using System.Collections.Concurrent;
using System.Text;
using MQTTnet;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex051;

// Exercise 051 — MqttRequestReply (reference solution).
public sealed class MqttRequestReply(IMqttClient client, string responseTopic)
{
    // Keyed by correlation id, NOT by response topic. Both requests share the response
    // topic - a client subscribes once - so "the reply arrived on my topic" identifies
    // nothing the moment two requests overlap.
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _waiting = new();

    public async Task StartAsync()
    {
        client.ApplicationMessageReceivedAsync += e =>
        {
            var correlation = Decode(e.ApplicationMessage.CorrelationData);

            // TryRemove, so a duplicate reply cannot complete an already-finished waiter,
            // and an unknown correlation id is simply dropped rather than handed to
            // whoever happens to be waiting.
            if (correlation is not null && _waiting.TryRemove(correlation, out var waiter))
                waiter.TrySetResult(e.ApplicationMessage.ConvertPayloadToString() ?? "");

            return Task.CompletedTask;
        };

        await client.SubscribeAsync(responseTopic);
    }

    public async Task<string> RequestAsync(string requestTopic, string payload, TimeSpan timeout)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        var waiter = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _waiting[correlationId] = waiter;

        try
        {
            await client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(requestTopic)
                .WithPayload(payload)
                .WithResponseTopic(responseTopic)
                .WithCorrelationData(Encoding.UTF8.GetBytes(correlationId))
                .Build());

            return await waiter.Task.WaitAsync(timeout);
        }
        catch (TimeoutException)
        {
            // Clean up, or a request that timed out leaks its waiter for the lifetime of
            // the process - and a late reply would complete a Task nobody is holding.
            _waiting.TryRemove(correlationId, out _);
            throw;
        }
    }

    public static async Task StartResponderAsync(IMqttClient client, string requestTopic, Func<string, string> handle)
    {
        client.ApplicationMessageReceivedAsync += async e =>
        {
            // The reply goes to the REQUEST'S response topic, not to a topic the
            // responder chose. That is what lets one responder serve many callers, each
            // listening somewhere different.
            if (e.ApplicationMessage.ResponseTopic is not { } replyTo)
                return;

            await client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(replyTo)
                .WithPayload(handle(e.ApplicationMessage.ConvertPayloadToString() ?? ""))
                .WithCorrelationData(e.ApplicationMessage.CorrelationData)
                .Build());
        };

        await client.SubscribeAsync(requestTopic);
    }

    private static string? Decode(byte[]? correlationData) =>
        correlationData is null or { Length: 0 } ? null : Encoding.UTF8.GetString(correlationData);
}
