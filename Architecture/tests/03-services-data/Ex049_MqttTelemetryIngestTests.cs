using FeWoLearning.Architecture.Exercises.ServicesData.Ex049;
using FeWoLearning.Architecture.Tests.Harness;
using MQTTnet;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex049_MqttTelemetryIngestTests : IAsyncLifetime
{
    private readonly MqttBrokerFixture _broker = new();
    private IMqttClient _subscriber = null!;
    private IMqttClient _publisher = null!;

    public async ValueTask InitializeAsync()
    {
        await _broker.StartAsync();
        _subscriber = await _broker.ConnectClientAsync("ingest");
        _publisher = await _broker.ConnectClientAsync("sensors");
    }

    public async ValueTask DisposeAsync()
    {
        _subscriber.Dispose();
        _publisher.Dispose();
        await _broker.DisposeAsync();
    }

    /// <summary>
    /// Publishes, then waits until the ingest has seen everything it is going to. The
    /// broker is real and delivery is asynchronous, so "assert immediately after publish"
    /// would be a race - and one that passes on a fast machine.
    /// </summary>
    private static async Task<IReadOnlyList<(string Topic, string Payload)>> Settle(
        TelemetryIngest ingest, int expected)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);

        while (ingest.Received.Count < expected && DateTime.UtcNow < deadline)
            await Task.Delay(20);

        // A short grace period after the expected count, so a fact asserting that
        // something did NOT arrive cannot pass merely by looking too early.
        await Task.Delay(150);
        return ingest.Received;
    }

    [Fact]
    public async Task An_Exact_Filter_Receives_Its_Own_Topic_Only()
    {
        var ingest = new TelemetryIngest(_subscriber);
        await ingest.SubscribeAsync("sensors/kitchen/temperature");

        await _publisher.PublishStringAsync("sensors/kitchen/temperature", "21.5");
        await _publisher.PublishStringAsync("sensors/kitchen/humidity", "48");

        var received = await Settle(ingest, 1);

        Assert.Equal([("sensors/kitchen/temperature", "21.5")], received);
    }

    [Fact]
    public async Task Mechanism_A_Plus_Wildcard_Matches_Exactly_One_Level()
    {
        // The level people misread. Treating "+" as "everything below here" is the same
        // mistake as prefix matching in exercise 045, with the same consequence: a
        // subscriber that asked for one level starts receiving every sub-topic the day
        // somebody adds one. The broker decides this, not the exercise - which is why
        // these facts run against a real one.
        var ingest = new TelemetryIngest(_subscriber);
        await ingest.SubscribeAsync("sensors/+/temperature");

        await _publisher.PublishStringAsync("sensors/kitchen/temperature", "21.5");
        await _publisher.PublishStringAsync("sensors/attic/temperature", "9.0");
        await _publisher.PublishStringAsync("sensors/kitchen/oven/temperature", "220");

        var received = await Settle(ingest, 2);

        Assert.Equal(
            [("sensors/attic/temperature", "9.0"), ("sensors/kitchen/temperature", "21.5")],
            received.OrderBy(r => r.Topic));
    }

    [Fact]
    public async Task A_Hash_Wildcard_Matches_Every_Remaining_Level()
    {
        var ingest = new TelemetryIngest(_subscriber);
        await ingest.SubscribeAsync("sensors/#");

        await _publisher.PublishStringAsync("sensors/kitchen/temperature", "21.5");
        await _publisher.PublishStringAsync("sensors/kitchen/oven/temperature", "220");
        await _publisher.PublishStringAsync("actuators/kitchen/light", "on");

        var received = await Settle(ingest, 2);

        Assert.Equal(2, received.Count);
        Assert.DoesNotContain(received, r => r.Topic.StartsWith("actuators", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Payloads_Round_Trip_Unchanged()
    {
        var ingest = new TelemetryIngest(_subscriber);
        await ingest.SubscribeAsync("sensors/#");

        await _publisher.PublishStringAsync("sensors/kitchen/status", "{\"ok\":true,\"unit\":\"°C\"}");

        var received = await Settle(ingest, 1);

        Assert.Equal("{\"ok\":true,\"unit\":\"°C\"}", received[0].Payload);
    }
}
