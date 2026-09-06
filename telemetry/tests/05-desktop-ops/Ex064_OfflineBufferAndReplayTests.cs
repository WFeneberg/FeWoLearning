using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex064_OfflineBufferAndReplayTests
{
    private const string SourceName = "fewolearning.telemetry.ex064.driver";

    /// <summary>
    /// Drives the exporter through a simple export processor, so every finished span
    /// reaches it immediately and the buffering under test is the only buffering there is.
    /// </summary>
    private sealed class Rig : IDisposable
    {
        private readonly ActivitySource _source = new(SourceName);
        private readonly TracerProvider _provider;

        public Rig(Ex064_OfflineBufferAndReplay exporter)
        {
            Exporter = exporter;
            _provider = Sdk.CreateTracerProviderBuilder()
                .AddSource(SourceName)
                .AddProcessor(new SimpleActivityExportProcessor(exporter))
                .Build();
        }

        public Ex064_OfflineBufferAndReplay Exporter { get; }

        public void Emit(string name)
        {
            using var activity = _source.StartActivity(name);
        }

        public void Dispose()
        {
            _provider.Dispose();
            _source.Dispose();
        }
    }

    [Fact]
    public void While_online_spans_are_delivered_straight_away()
    {
        using var ctx = new TelemetryContext();
        using var rig = new Rig(new Ex064_OfflineBufferAndReplay());

        rig.Emit("a");
        rig.Emit("b");

        Assert.Equal(["a", "b"], rig.Exporter.Delivered);
        Assert.Equal(0, rig.Exporter.Buffered);
    }

    [Fact]
    public void Adversarial_A_While_offline_nothing_is_delivered_and_nothing_is_lost()
    {
        // A laptop on a train. It will be back in forty minutes with everything that
        // happened in between - which is the interesting part, since whatever the user is
        // about to complain about happened while they were offline.
        using var ctx = new TelemetryContext();
        using var rig = new Rig(new Ex064_OfflineBufferAndReplay { IsOnline = false });

        rig.Emit("a");
        rig.Emit("b");

        Assert.Empty(rig.Exporter.Delivered);
        Assert.Equal(2, rig.Exporter.Buffered);
        Assert.Equal(0, rig.Exporter.Dropped);
    }

    [Fact]
    public void Adversarial_B_Coming_back_online_replays_the_buffer_in_order()
    {
        // In order, and before the new one. A replay that appends after whatever is
        // happening now produces a trace where the past arrives after the present, which
        // no backend will reorder for you.
        using var ctx = new TelemetryContext();
        using var rig = new Rig(new Ex064_OfflineBufferAndReplay { IsOnline = false });

        rig.Emit("a");
        rig.Emit("b");
        rig.Exporter.IsOnline = true;
        rig.Emit("c");

        Assert.Equal(["a", "b", "c"], rig.Exporter.Delivered);
        Assert.Equal(0, rig.Exporter.Buffered);
    }

    [Fact]
    public void Adversarial_C_Past_the_capacity_the_OLDEST_is_dropped()
    {
        // A real decision, and it goes the way most instincts do not. When the buffer is
        // full something has to go, and the newest records are the ones nearest to
        // whatever is going wrong NOW - so the oldest go. That is the opposite of a
        // queue's natural behaviour, and writing it down as a policy is the difference
        // between choosing it and inheriting it.
        using var ctx = new TelemetryContext();
        using var rig = new Rig(new Ex064_OfflineBufferAndReplay { IsOnline = false });

        for (var i = 1; i <= Ex064_OfflineBufferAndReplay.Capacity + 2; i++) rig.Emit($"s{i}");

        rig.Exporter.IsOnline = true;
        rig.Emit("last");

        // s1 and s2 fell out; the five newest survived.
        Assert.Equal(["s3", "s4", "s5", "s6", "s7", "last"], rig.Exporter.Delivered);
    }

    [Fact]
    public void Adversarial_D_The_number_dropped_is_reported()
    {
        // What stops the whole thing being a lie. A buffer that silently drops produces
        // telemetry with invisible holes: a gap that looks like nothing happened, in a
        // period where quite a lot did. Counting turns an unknown unknown into a number.
        using var ctx = new TelemetryContext();
        using var rig = new Rig(new Ex064_OfflineBufferAndReplay { IsOnline = false });

        for (var i = 1; i <= Ex064_OfflineBufferAndReplay.Capacity + 2; i++) rig.Emit($"s{i}");

        Assert.Equal(2, rig.Exporter.Dropped);
        Assert.Equal(Ex064_OfflineBufferAndReplay.Capacity, rig.Exporter.Buffered);
    }
}
