using System.Text;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex051;
using FeWoLearning.Architecture.Tests.Harness;
using MQTTnet;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex051_MqttRequestReplyTests : IAsyncLifetime
{
    private const string RequestTopic = "rpc/echo";
    private const string ResponseTopic = "rpc/echo/reply/client-1";
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

    private readonly MqttBrokerFixture _broker = new();
    private IMqttClient _caller = null!;
    private IMqttClient _responder = null!;

    public async ValueTask InitializeAsync()
    {
        await _broker.StartAsync();
        _caller = await _broker.ConnectClientAsync("caller");
        _responder = await _broker.ConnectClientAsync("responder");
    }

    public async ValueTask DisposeAsync()
    {
        _caller.Dispose();
        _responder.Dispose();
        await _broker.DisposeAsync();
    }

    [Fact]
    public async Task A_Request_Gets_Its_Reply()
    {
        await MqttRequestReply.StartResponderAsync(_responder, RequestTopic, p => "echo:" + p);
        var rpc = new MqttRequestReply(_caller, ResponseTopic);
        await rpc.StartAsync();

        Assert.Equal("echo:hello", await rpc.RequestAsync(RequestTopic, "hello", Patience));
    }

    [Fact]
    public async Task Mechanism_Two_Concurrent_Requests_Each_Get_Their_Own_Reply()
    {
        // The whole exercise, and the case a sequential test cannot see. Both requests
        // share the response topic - a client subscribes once - so matching on the topic
        // alone works perfectly until two requests overlap, at which point each caller
        // gets whichever reply landed first.
        //
        // The responder here holds both requests and then answers them in REVERSE order,
        // so the reply for "two" reaches the caller before the reply for "one". A
        // topic-only matcher hands the first arrival to the first waiter and gets both
        // answers wrong; only correlation data sorts them out.
        //
        // Note what this fact does NOT do: block inside the responder's handler to force
        // overlap. Measured on MQTTnet 5.2 - a single client delivers to its
        // ApplicationMessageReceivedAsync handler SEQUENTIALLY, so blocking there stops
        // the second request ever arriving and the test deadlocks on its own gate.
        var pending = new List<(string ReplyTo, byte[]? Correlation, string Payload)>();
        var bothArrived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await _responder.SubscribeAsync(RequestTopic);
        _responder.ApplicationMessageReceivedAsync += e =>
        {
            lock (pending)
            {
                pending.Add((e.ApplicationMessage.ResponseTopic!,
                             e.ApplicationMessage.CorrelationData,
                             e.ApplicationMessage.ConvertPayloadToString() ?? ""));

                if (pending.Count == 2)
                    bothArrived.TrySetResult();
            }

            return Task.CompletedTask;
        };

        var rpc = new MqttRequestReply(_caller, ResponseTopic);
        await rpc.StartAsync();

        var first = rpc.RequestAsync(RequestTopic, "one", Patience);
        var second = rpc.RequestAsync(RequestTopic, "two", Patience);

        await bothArrived.Task.WaitAsync(Patience);

        List<(string ReplyTo, byte[]? Correlation, string Payload)> requests;
        lock (pending)
            requests = [.. pending];

        requests.Reverse(); // answer "two" first
        foreach (var request in requests)
        {
            await _responder.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(request.ReplyTo)
                .WithPayload("echo:" + request.Payload)
                .WithCorrelationData(request.Correlation)
                .Build());
        }

        Assert.Equal("echo:one", await first);
        Assert.Equal("echo:two", await second);
    }

    [Fact]
    public async Task Adversarial_A_Reply_With_Foreign_Correlation_Data_Is_Ignored()
    {
        // A responder that copies nothing, or copies the wrong thing, must not be able to
        // complete somebody's request by accident. The waiter times out instead - which
        // is the correct outcome: no answer arrived for THIS question.
        await _responder.SubscribeAsync(RequestTopic);
        _responder.ApplicationMessageReceivedAsync += async e =>
        {
            await _responder.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(e.ApplicationMessage.ResponseTopic!)
                .WithPayload("echo:impostor")
                .WithCorrelationData(Encoding.UTF8.GetBytes("somebody-elses-id"))
                .Build());
        };

        var rpc = new MqttRequestReply(_caller, ResponseTopic);
        await rpc.StartAsync();

        await Assert.ThrowsAsync<TimeoutException>(
            () => rpc.RequestAsync(RequestTopic, "hello", TimeSpan.FromMilliseconds(600)));
    }

    [Fact]
    public async Task No_Responder_At_All_Times_Out()
    {
        var rpc = new MqttRequestReply(_caller, ResponseTopic);
        await rpc.StartAsync();

        await Assert.ThrowsAsync<TimeoutException>(
            () => rpc.RequestAsync("rpc/nobody-is-listening", "hello", TimeSpan.FromMilliseconds(400)));
    }

    [Fact]
    public async Task A_Timed_Out_Request_Does_Not_Break_The_Next_One()
    {
        // Catches a waiter that is left behind on timeout: a late reply for the abandoned
        // request must not complete the following one, and the map must not grow forever.
        var rpc = new MqttRequestReply(_caller, ResponseTopic);
        await rpc.StartAsync();

        await Assert.ThrowsAsync<TimeoutException>(
            () => rpc.RequestAsync("rpc/nobody-is-listening", "hello", TimeSpan.FromMilliseconds(400)));

        await MqttRequestReply.StartResponderAsync(_responder, RequestTopic, p => "echo:" + p);

        Assert.Equal("echo:later", await rpc.RequestAsync(RequestTopic, "later", Patience));
    }
}
