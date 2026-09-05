using System.Net;
using System.Net.Sockets;
using MQTTnet;
using MQTTnet.Server;

namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// A real MQTT broker, in this process, on a loopback port.
///
/// This is why the MQTT rows are graded in the default run rather than gated behind
/// Docker: MQTTnet 5 ships its server in the main package, so the facts get real
/// protocol frames, real QoS 1 redelivery, real retained-message delivery to a late
/// subscriber and a real last will on an ungraceful disconnect - none of which a
/// hand-written fake can honestly produce.
/// </summary>
public sealed class MqttBrokerFixture : IAsyncDisposable
{
    private MqttServer? _server;

    public int Port { get; private set; }

    public MqttServer Server =>
        _server ?? throw new InvalidOperationException("StartAsync has not run yet.");

    public async Task StartAsync()
    {
        Port = FreePort();

        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Loopback)
            .WithDefaultEndpointPort(Port)
            .Build();

        _server = new MqttServerFactory().CreateMqttServer(options);
        await _server.StartAsync();
    }

    /// <summary>Connects a client to this broker and waits for CONNACK.</summary>
    public async Task<IMqttClient> ConnectClientAsync(
        string clientId,
        Action<MqttClientOptionsBuilder>? configure = null)
    {
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", Port)
            .WithClientId(clientId);

        configure?.Invoke(builder);

        var client = new MqttClientFactory().CreateMqttClient();
        await client.ConnectAsync(builder.Build());
        return client;
    }

    public async ValueTask DisposeAsync()
    {
        if (_server is null) return;
        await _server.StopAsync();
        _server.Dispose();
        _server = null;
    }

    /// <summary>
    /// The broker needs a port before it starts, and MqttServerOptionsBuilder has no
    /// "any free port" mode, so ask the OS for one and hand it back immediately.
    /// </summary>
    private static int FreePort()
    {
        var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }
}
