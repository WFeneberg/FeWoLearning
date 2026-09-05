using FeWoLearning.Architecture.Exercises.Support;
using MQTTnet;

namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// The only facts in this track that pass in BOTH modes. They exist so a broken
/// harness fails loudly and first, instead of surfacing as sixty confusing exercise
/// failures. If one of these goes red after a package bump, fix it before reading
/// anything else in the run.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void ManualClock_Advances_Only_When_Told_To()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var clock = new ManualClock(start);

        Assert.Equal(start, clock.UtcNow);
        clock.Advance(TimeSpan.FromMinutes(5));
        Assert.Equal(start.AddMinutes(5), clock.UtcNow);
    }

    [Fact]
    public void SqliteScratch_Is_A_File_Database_That_Two_Connections_Share()
    {
        using var scratch = new SqliteScratch();

        scratch.Execute("CREATE TABLE t (v TEXT); INSERT INTO t VALUES ('x');");

        using var reader = scratch.OpenConnection();
        using var read = reader.CreateCommand();
        read.CommandText = "SELECT v FROM t";

        // The point of the fixture: a SECOND connection sees the first one's data.
        // ":memory:" would give each connection its own empty database.
        Assert.Equal("x", read.ExecuteScalar());
    }

    [Fact]
    public async Task MqttBroker_Round_Trips_A_Message_Between_Two_Real_Clients()
    {
        await using var broker = new MqttBrokerFixture();
        await broker.StartAsync();

        using var subscriber = await broker.ConnectClientAsync("smoke-sub");
        using var publisher = await broker.ConnectClientAsync("smoke-pub");

        var received = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        subscriber.ApplicationMessageReceivedAsync += e =>
        {
            received.TrySetResult(e.ApplicationMessage.ConvertPayloadToString() ?? "");
            return Task.CompletedTask;
        };

        await subscriber.SubscribeAsync("smoke/#");
        await publisher.PublishStringAsync("smoke/one", "hello");

        var payload = await received.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal("hello", payload);
    }

    [Fact]
    public void ContainerGate_Reflects_The_Build_Switch()
    {
        // Documents the default (off) without hard-coding false: with
        // -p:Containers=true this asserts the opposite, and still holds.
        var expected =
            Environment.GetEnvironmentVariable("FEWO_ARCH_CONTAINERS") == "1"
            || AppContext.GetData("FeWoLearning.Architecture.Containers") as string == "true";

        Assert.Equal(expected, ContainerGate.Enabled);
    }
}
