using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex018_StatusAndExceptionTests
{
    private static TraceProbe Probe() => new(Ex018_StatusAndException.SourceName);

    [Fact]
    public void Work_that_succeeds_leaves_the_activity_ok_and_unmarked()
    {
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Ex018_StatusAndException.Execute(() => { });

        var activity = probe.Single();
        Assert.Equal(ActivityStatusCode.Ok, activity.Status);
        Assert.Empty(activity.Events);
    }

    [Fact]
    public void Adversarial_A_The_exception_is_rethrown()
    {
        // The bug this row exists for, and it is a bad one. The natural shape for
        // "record the failure" is try/catch, and a catch block that records and then
        // falls off the end has turned an exception into a silent success: the caller
        // carries on with a null it did not expect, while the trace looks perfect - a
        // red span sitting neatly under a green one.
        //
        // Observability code must be transparent to control flow.
        using var ctx = new TelemetryContext();
        using var probe = Probe();
        var boom = new InvalidOperationException("ledger is closed");

        var thrown = Assert.Throws<InvalidOperationException>(
            () => Ex018_StatusAndException.Execute(() => throw boom));

        Assert.Same(boom, thrown);
    }

    [Fact]
    public void A_failure_sets_the_error_status_and_its_description()
    {
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Assert.Throws<InvalidOperationException>(
            () => Ex018_StatusAndException.Execute(() => throw new InvalidOperationException("ledger is closed")));

        var activity = probe.Single();
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal("ledger is closed", activity.StatusDescription);
    }

    [Fact]
    public void Adversarial_B_The_failure_is_recorded_under_the_conventional_names()
    {
        // About names, not behaviour. Every backend, dashboard and alert rule keys on
        // exception.type and exception.message inside an event literally called
        // "exception". Invent your own and the data is technically all there and
        // practically invisible - nothing will find it, and nothing will tell you so.
        //
        // Activity.AddException writes exactly these.
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Assert.Throws<InvalidOperationException>(
            () => Ex018_StatusAndException.Execute(() => throw new InvalidOperationException("ledger is closed")));

        var recorded = Assert.Single(probe.Single().Events);
        Assert.Equal(Ex018_StatusAndException.ExceptionEventName, recorded.Name);

        var tags = recorded.Tags.ToDictionary(t => t.Key, t => t.Value?.ToString());
        Assert.Equal(
            typeof(InvalidOperationException).FullName,
            tags[Ex018_StatusAndException.ExceptionTypeTag]);
        Assert.Equal("ledger is closed", tags[Ex018_StatusAndException.ExceptionMessageTag]);
    }

    [Fact]
    public void The_activity_is_stopped_either_way()
    {
        // A `using` that only covers the happy path leaves the failing activity
        // running - which is how a trace ends up with a span whose duration is however
        // long the process lived.
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Ex018_StatusAndException.Execute(() => { });
        Assert.Throws<InvalidOperationException>(
            () => Ex018_StatusAndException.Execute(() => throw new InvalidOperationException("ledger is closed")));

        Assert.Equal(2, probe.Stopped.Count);
        Assert.Null(Activity.Current);
    }
}
