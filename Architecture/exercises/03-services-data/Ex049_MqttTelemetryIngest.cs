using MQTTnet;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex049;

// Exercise 049 — MqttTelemetryIngest (services-data).
// Goal:   Subscribe to a telemetry topic hierarchy and get the wildcard levels right,
//         against a real broker.
// Drills: MQTT topic filters, "+" versus "#", subscription lifetime.
// Passes: exact filter - "sensors/kitchen/temperature" receives that topic and no other.
//         "+"          - matches EXACTLY ONE level: "sensors/+/temperature" receives
//                        "sensors/kitchen/temperature" and does NOT receive
//                        "sensors/kitchen/oven/temperature".
//         "#"          - matches the remaining levels: "sensors/#" receives everything
//                        under sensors, at any depth.
//         payloads round-trip unchanged.
//
// These facts run against a REAL MQTT broker, started in-process by the test harness -
// real CONNECT, SUBSCRIBE and PUBLISH packets, and the broker's own filter matching. A
// hand-written matcher would let this exercise assert whatever the author believed "+"
// means; the broker is the thing that decides, so the broker is what the exercise talks
// to.
//
// "+" is the level that gets misread. Treating it as "everything below here" is the same
// mistake as prefix-matching in exercise 045, and it has the same consequence: a
// subscriber that asked for one level starts receiving every sub-topic the day somebody
// adds one.
public sealed class TelemetryIngest(IMqttClient client)
{
    /// <summary>Everything this ingest has received, in arrival order.</summary>
    public IReadOnlyList<(string Topic, string Payload)> Received =>
        throw new NotImplementedException("TODO: Ex049 - what arrived, in order");

    /// <summary>Start recording messages, then subscribe to <paramref name="topicFilter"/>.</summary>
    public Task SubscribeAsync(string topicFilter) =>
        throw new NotImplementedException(
            "TODO: Ex049 - hook ApplicationMessageReceivedAsync to record topic and payload, then subscribe to the filter");
}
