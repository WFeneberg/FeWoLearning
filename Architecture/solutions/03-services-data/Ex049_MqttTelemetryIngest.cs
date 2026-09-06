using MQTTnet;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex049;

// Exercise 049 — MqttTelemetryIngest (reference solution).
public sealed class TelemetryIngest(IMqttClient client)
{
    private readonly List<(string Topic, string Payload)> _received = [];
    private readonly Lock _gate = new();

    public IReadOnlyList<(string Topic, string Payload)> Received
    {
        get
        {
            // Copied under the lock. Messages arrive on the client's own receive loop,
            // so handing back the live list would let a test enumerate it mid-append.
            lock (_gate)
                return [.. _received];
        }
    }

    public async Task SubscribeAsync(string topicFilter)
    {
        // The handler is attached BEFORE the SUBSCRIBE goes out. The other order has a
        // window - small, real, and impossible to reproduce on demand - in which the
        // broker has already begun delivering to a client that is not listening yet.
        client.ApplicationMessageReceivedAsync += e =>
        {
            lock (_gate)
                _received.Add((e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString() ?? ""));

            return Task.CompletedTask;
        };

        await client.SubscribeAsync(topicFilter);
    }
}
