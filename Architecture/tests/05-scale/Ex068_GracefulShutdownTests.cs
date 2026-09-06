using FeWoLearning.Architecture.Exercises.Scale.Ex068;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex068_GracefulShutdownTests
{
    private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(30);

    private static (RequestHost Host, ManualClock Clock) Build()
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        return (new RequestHost(clock), clock);
    }

    [Fact]
    public void Requests_Are_Admitted_While_Running()
    {
        var (host, _) = Build();

        Assert.True(host.TryBegin("r-1"));
        Assert.Equal(1, host.InFlight);
        Assert.False(host.IsShuttingDown);
    }

    [Fact]
    public void Mechanism_New_Work_Is_Refused_As_Soon_As_Shutdown_Begins()
    {
        // The half people leave out, and the reason the order matters: draining while
        // still accepting is not a drain. Under load the in-flight count never reaches
        // zero, the deadline expires, and everything is abandoned - the same outcome as
        // no graceful shutdown at all, after a delay.
        var (host, clock) = Build();
        host.TryBegin("r-1");

        var admittedDuringDrain = true;

        var report = host.Shutdown(Deadline, () =>
        {
            admittedDuringDrain = host.TryBegin("late-arrival");
            host.Complete("r-1");
            clock.Advance(TimeSpan.FromSeconds(1));
        });

        Assert.False(admittedDuringDrain);
        Assert.Equal(new ShutdownReport(1, 0), report);
    }

    [Fact]
    public void In_Flight_Work_Is_Waited_For_And_Counted()
    {
        var (host, clock) = Build();
        host.TryBegin("r-1");
        host.TryBegin("r-2");
        host.TryBegin("r-3");

        var remaining = new Queue<string>(["r-1", "r-2", "r-3"]);

        var report = host.Shutdown(Deadline, () =>
        {
            host.Complete(remaining.Dequeue());
            clock.Advance(TimeSpan.FromSeconds(1));
        });

        Assert.Equal(new ShutdownReport(3, 0), report);
        Assert.Equal(0, host.InFlight);
    }

    [Fact]
    public void Mechanism_The_Deadline_Ends_The_Drain_And_The_Rest_Is_Reported()
    {
        // Every orchestrator has its own patience - Kubernetes sends SIGTERM and then
        // SIGKILL after terminationGracePeriodSeconds - and a process that waits longer
        // gets killed mid-drain, losing both the drain and any chance of saying what was
        // in flight. Finishing early with an honest count beats being killed with none.
        var (host, clock) = Build();
        host.TryBegin("finishes");
        host.TryBegin("never-finishes");

        var completed = false;

        var report = host.Shutdown(Deadline, () =>
        {
            if (!completed)
            {
                host.Complete("finishes");
                completed = true;
            }

            clock.Advance(TimeSpan.FromSeconds(10));
        });

        Assert.Equal(new ShutdownReport(1, 1), report);
    }

    [Fact]
    public void Adversarial_A_Drain_With_Nothing_In_Flight_Returns_Without_Waiting()
    {
        // An implementation that always runs its wait loop at least once burns the
        // orchestrator's patience on an idle instance - and multiplied across a rolling
        // deploy, that is the difference between a two-minute release and a twenty-minute
        // one.
        var (host, _) = Build();
        var waits = 0;

        var report = host.Shutdown(Deadline, () => waits++);

        Assert.Equal(0, waits);
        Assert.Equal(new ShutdownReport(0, 0), report);
    }

    [Fact]
    public void Shutting_Down_Twice_Is_Harmless()
    {
        // SIGTERM arriving twice, or a health check and a signal handler both calling it,
        // is ordinary. A second drain that resets the counters or waits again reports
        // fiction.
        var (host, clock) = Build();
        host.TryBegin("r-1");

        var first = host.Shutdown(Deadline, () =>
        {
            host.Complete("r-1");
            clock.Advance(TimeSpan.FromSeconds(1));
        });

        var second = host.Shutdown(Deadline, () => Assert.Fail("the second shutdown should not drain again"));

        Assert.Equal(new ShutdownReport(1, 0), first);
        Assert.Equal(0, second.Abandoned);
    }
}
