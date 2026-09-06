using MQTTnet;
using MQTTnet.Protocol;

namespace FeWoLearning.Architecture.Exercises.ServicesData.Ex050;

// Exercise 050 — MqttQosRetainedLastWill (reference solution).
public static class Ex050_MqttQosRetainedLastWill
{
    public static Task PublishRetainedAsync(IMqttClient client, string topic, string payload) =>
        client.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            // The one flag that matters here. The broker keeps this as the topic's
            // current value and hands it to every future subscriber at subscribe time.
            .WithRetainFlag()
            .Build());

    public static Task PublishAsync(IMqttClient client, string topic, string payload) =>
        client.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build());

    public static Task ConnectWithWillAsync(
        IMqttClient client, int port, string clientId, string willTopic, string willPayload) =>
        client.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", port)
            .WithClientId(clientId)
            // The will is registered at CONNECT time, with the broker, and held by the
            // broker. That is the whole trick: it is published by something that is
            // still running when this client is not.
            .WithWillTopic(willTopic)
            .WithWillPayload(willPayload)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build());

    public static Task DisconnectTriggeringWillAsync(IMqttClient client)
    {
        // Tear the connection down WITHOUT sending a DISCONNECT packet - which is what a
        // device that lost power or fell off the network actually does, and the only
        // thing that makes a broker publish the will.
        //
        // Measured on MQTTnet 5.2: DisconnectAsync with
        // MqttClientDisconnectOptionsReason.DisconnectWithWillMessage - the MQTT 5 reason
        // code that exists precisely for this - does NOT make MQTTnet's own broker
        // publish the will, with or without WithProtocolVersion(V500). Disposing the
        // client does.
        client.Dispose();
        return Task.CompletedTask;
    }

    public static Task DisconnectCleanlyAsync(IMqttClient client) =>
        client.DisconnectAsync(new MqttClientDisconnectOptions
        {
            // A will that fires on every disconnect is not an availability signal, it is
            // noise - every deployment and every restart publishes "device offline", and
            // the dashboard showing it gets ignored.
            Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
        });
}
