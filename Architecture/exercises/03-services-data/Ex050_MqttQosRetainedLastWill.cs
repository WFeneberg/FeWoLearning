using MQTTnet;
using MQTTnet.Protocol;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex050;

// Exercise 050 — MqttQosRetainedLastWill (services-data).
// Goal:   Use the three MQTT features that exist because devices are not servers: they
//         are offline more often than not, they join late, and they vanish without
//         saying goodbye.
// Drills: retained messages, last will and testament, QoS on publish.
// Passes: retained     - PublishRetained puts the LAST value on the topic, and a
//                        subscriber that connects AFTERWARDS receives it immediately.
//         not retained - an ordinary publish leaves nothing behind for a late subscriber.
//         retained is last-value-wins, not a log: two retained publishes leave one message.
//         the will     - ConnectWithWillAsync arranges a message the BROKER publishes if
//                        that client disappears without a proper disconnect.
//         THE ONE       - a client that disconnects PROPERLY does NOT trigger its will.
//
// The last clause is what makes the will worth anything. A will that fires on every
// disconnect is not an availability signal, it is noise: every deployment, every restart,
// every scale-down publishes "device offline", and the dashboard that shows it is
// promptly ignored by everyone.
//
// Retained is a per-topic slot the broker overwrites, not a queue. It is how a device
// that publishes its temperature once a minute can still answer "what is it now" for a
// dashboard that opened thirty seconds ago - and it is why a retained topic must hold
// state and never an event.
public static class Ex050_MqttQosRetainedLastWill
{
    /// <summary>Publish so the broker keeps it as the topic's current value.</summary>
    public static Task PublishRetainedAsync(IMqttClient client, string topic, string payload) =>
        throw new NotImplementedException(
            "TODO: Ex050 - publish with the retain flag set and QoS AtLeastOnce");

    /// <summary>Publish ordinarily: delivered to whoever is listening now, kept by nobody.</summary>
    public static Task PublishAsync(IMqttClient client, string topic, string payload) =>
        throw new NotImplementedException(
            "TODO: Ex050 - publish without the retain flag, QoS AtLeastOnce");

    /// <summary>
    /// Connect <paramref name="client"/> to 127.0.0.1:<paramref name="port"/> with a will
    /// on <paramref name="willTopic"/> carrying <paramref name="willPayload"/>.
    /// </summary>
    public static Task ConnectWithWillAsync(
        IMqttClient client, int port, string clientId, string willTopic, string willPayload) =>
        throw new NotImplementedException(
            "TODO: Ex050 - build client options with WithWillTopic/WithWillPayload/WithWillQualityOfServiceLevel and connect");

    /// <summary>
    /// Go away the way a device that lost power goes away: tear the connection down
    /// without sending a DISCONNECT packet.
    ///
    /// Measured on MQTTnet 5.2, and worth knowing before you reach for it:
    /// DisconnectAsync with MqttClientDisconnectOptionsReason.DisconnectWithWillMessage -
    /// the MQTT 5 reason code that exists for exactly this - does NOT make MQTTnet's own
    /// broker publish the will, with or without WithProtocolVersion(V500).
    /// </summary>
    public static Task DisconnectTriggeringWillAsync(IMqttClient client) =>
        throw new NotImplementedException(
            "TODO: Ex050 - end the connection without a DISCONNECT packet");

    /// <summary>Disconnect properly. The will must NOT be published.</summary>
    public static Task DisconnectCleanlyAsync(IMqttClient client) =>
        throw new NotImplementedException(
            "TODO: Ex050 - disconnect with reason NormalDisconnection");
}
