using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FeWoLearning.Architecture.Exercises.ServicesData.Ex050;
using FeWoLearning.Architecture.Tests.Harness;
using MQTTnet;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex050_MqttQosRetainedLastWillTests : IAsyncLifetime
{
    private readonly MqttBrokerFixture _broker = new();

    public ValueTask InitializeAsync() => new(_broker.StartAsync());

    public ValueTask DisposeAsync() => _broker.DisposeAsync();

    /// <summary>Collects everything a client receives, and lets a fact wait for it.</summary>
    private sealed class Collector
    {
        public List<(string Topic, string Payload)> Received { get; } = [];

        public void Attach(IMqttClient client) =>
            client.ApplicationMessageReceivedAsync += e =>
            {
                lock (Received)
                    Received.Add((e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString() ?? ""));
                return Task.CompletedTask;
            };

        public async Task<IReadOnlyList<(string Topic, string Payload)>> Settle(int expected)
        {
            var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);

            while (DateTime.UtcNow < deadline)
            {
                lock (Received)
                    if (Received.Count >= expected) break;
                await Task.Delay(20);
            }

            // A grace period, so a fact asserting nothing arrived cannot pass by looking
            // too early.
            await Task.Delay(200);
            lock (Received)
                return [.. Received];
        }
    }

    [Fact]
    public async Task Mechanism_A_Retained_Message_Reaches_A_Subscriber_That_Arrives_Later()
    {
        // Why retained exists: a device publishing once a minute can still answer "what
        // is it now" for a dashboard that opened thirty seconds ago.
        using var device = await _broker.ConnectClientAsync("device");
        await Ex050_MqttQosRetainedLastWill.PublishRetainedAsync(device, "state/kitchen/temperature", "21.5");

        using var dashboard = await _broker.ConnectClientAsync("dashboard");
        var collector = new Collector();
        collector.Attach(dashboard);
        await dashboard.SubscribeAsync("state/#");

        Assert.Equal([("state/kitchen/temperature", "21.5")], await collector.Settle(1));
    }

    [Fact]
    public async Task Adversarial_An_Ordinary_Message_Leaves_Nothing_For_A_Late_Subscriber()
    {
        // Retaining everything is the easy way to pass the fact above, and it turns the
        // broker into a database where every event ever published is replayed to every
        // new subscriber.
        using var device = await _broker.ConnectClientAsync("device");
        await Ex050_MqttQosRetainedLastWill.PublishAsync(device, "events/kitchen/door-opened", "1");

        using var dashboard = await _broker.ConnectClientAsync("dashboard");
        var collector = new Collector();
        collector.Attach(dashboard);
        await dashboard.SubscribeAsync("events/#");

        Assert.Empty(await collector.Settle(0));
    }

    [Fact]
    public async Task Retained_Is_Last_Value_Wins_Not_A_Log()
    {
        using var device = await _broker.ConnectClientAsync("device");
        await Ex050_MqttQosRetainedLastWill.PublishRetainedAsync(device, "state/kitchen/temperature", "21.5");
        await Ex050_MqttQosRetainedLastWill.PublishRetainedAsync(device, "state/kitchen/temperature", "22.0");

        using var dashboard = await _broker.ConnectClientAsync("dashboard");
        var collector = new Collector();
        collector.Attach(dashboard);
        await dashboard.SubscribeAsync("state/#");

        Assert.Equal([("state/kitchen/temperature", "22.0")], await collector.Settle(1));
    }

    [Fact]
    public async Task Mechanism_The_Will_Is_Published_When_A_Client_Vanishes()
    {
        using var monitor = await _broker.ConnectClientAsync("monitor");
        var collector = new Collector();
        collector.Attach(monitor);
        await monitor.SubscribeAsync("status/#");

        using var device = new MqttClientFactory().CreateMqttClient();
        await Ex050_MqttQosRetainedLastWill.ConnectWithWillAsync(
            device, _broker.Port, "device", "status/device", "offline");

        await Ex050_MqttQosRetainedLastWill.DisconnectTriggeringWillAsync(device);

        Assert.Equal([("status/device", "offline")], await collector.Settle(1));
    }

    [Fact]
    public async Task Adversarial_A_Clean_Disconnect_Does_Not_Publish_The_Will()
    {
        // What makes a will worth anything. One that fires on every disconnect is not an
        // availability signal, it is noise: every deployment, every restart and every
        // scale-down publishes "offline", and the dashboard showing it gets ignored.
        using var monitor = await _broker.ConnectClientAsync("monitor");
        var collector = new Collector();
        collector.Attach(monitor);
        await monitor.SubscribeAsync("status/#");

        using var device = new MqttClientFactory().CreateMqttClient();
        await Ex050_MqttQosRetainedLastWill.ConnectWithWillAsync(
            device, _broker.Port, "device", "status/device", "offline");

        await Ex050_MqttQosRetainedLastWill.DisconnectCleanlyAsync(device);

        Assert.Empty(await collector.Settle(0));
    }

    [Fact]
    public async Task Container_The_Same_Client_Code_Behaves_On_Real_Mosquitto()
    {
        // The in-process broker is MQTTnet's own. This one runs the exercise's client
        // code against Mosquitto, so the retained and will behaviour being graded is the
        // protocol's rather than one implementation's. Skipped unless -p:Containers=true.
        ContainerGate.SkipUnlessEnabled();

        const string config = "listener 1883\nallow_anonymous true\n";

        // The image goes to the constructor: the parameterless ContainerBuilder is
        // obsolete in Testcontainers 4.14. Mosquitto 2 refuses anonymous clients unless
        // told otherwise, hence the mapped config.
        IContainer mosquitto = new ContainerBuilder("eclipse-mosquitto:2")
            .WithPortBinding(1883, assignRandomHostPort: true)
            .WithResourceMapping(Encoding.UTF8.GetBytes(config), "/mosquitto/config/mosquitto.conf")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("mosquitto version 2"))
            .Build();

        await mosquitto.StartAsync();
        var port = mosquitto.GetMappedPublicPort(1883);

        var factory = new MqttClientFactory();

        using (var device = factory.CreateMqttClient())
        {
            await device.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", port).WithClientId("device").Build());
            await Ex050_MqttQosRetainedLastWill.PublishRetainedAsync(device, "state/kitchen/temperature", "21.5");
            await Ex050_MqttQosRetainedLastWill.DisconnectCleanlyAsync(device);
        }

        using var dashboard = factory.CreateMqttClient();
        var collector = new Collector();
        collector.Attach(dashboard);
        await dashboard.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", port).WithClientId("dashboard").Build());
        await dashboard.SubscribeAsync("state/#");

        Assert.Equal([("state/kitchen/temperature", "21.5")], await collector.Settle(1));

        await mosquitto.DisposeAsync();
    }
}
