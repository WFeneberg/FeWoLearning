using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex016_ActivityParentChildTests
{
    private static readonly string[] Steps = ["fetch", "transform", "store"];

    private static TraceProbe Probe() => new(Ex016_ActivityParentChild.SourceName);

    [Fact]
    public void The_steps_finish_inside_the_pipeline()
    {
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Ex016_ActivityParentChild.RunPipeline(Steps);

        // Children stop before their parent, so the pipeline is last.
        Assert.Equal(
            ["step", "step", "step", "pipeline"],
            probe.Stopped.Select(a => a.DisplayName));
        Assert.Equal(
            ["fetch", "transform", "store"],
            probe.Stopped.Take(3).Select(a => a.GetTagItem(Ex016_ActivityParentChild.StepTag)?.ToString()));
    }

    [Fact]
    public void Every_activity_shares_the_pipelines_trace_id()
    {
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Ex016_ActivityParentChild.RunPipeline(Steps);

        var traceIds = probe.Stopped.Select(a => a.TraceId).Distinct().ToArray();
        Assert.Single(traceIds);
    }

    [Fact]
    public void Adversarial_A_The_steps_are_siblings_not_a_staircase()
    {
        // The shape bug, and it is invisible in every summary view. A `using` that is
        // not scoped to one iteration leaves step 1 current while step 2 starts, so
        // step 2 becomes step 1's child and a flat fan-out renders as a staircase.
        // Durations still add up, nothing errors, and the waterfall claims the steps
        // depend on each other when they do not.
        using var ctx = new TelemetryContext();
        using var probe = Probe();

        Ex016_ActivityParentChild.RunPipeline(Steps);

        var pipeline = probe.Stopped.Single(a => a.DisplayName == Ex016_ActivityParentChild.PipelineName);
        var steps = probe.Stopped.Where(a => a.DisplayName == Ex016_ActivityParentChild.StepName).ToArray();

        Assert.Equal(3, steps.Length);
        Assert.All(steps, s => Assert.Equal(pipeline.SpanId, s.ParentSpanId));
    }

    [Fact]
    public void Adversarial_B_The_ambient_activity_is_left_exactly_as_it_was_found()
    {
        // Activity.Current is ambient state on the thread. A method that leaves it
        // changed has quietly reparented everything its caller does next - and a method
        // that ignores what was already there has orphaned its own work from the
        // caller's trace.
        //
        // `new Activity(...).Start()` needs no listener at all, which is itself worth
        // knowing: it is ActivitySource.StartActivity that returns null unheard.
        using var ctx = new TelemetryContext();
        using var probe = Probe();
        var outer = new Activity("outer").Start();

        Ex016_ActivityParentChild.RunPipeline(Steps);

        Assert.Same(outer, Activity.Current);

        var pipeline = probe.Stopped.Single(a => a.DisplayName == Ex016_ActivityParentChild.PipelineName);
        Assert.Equal(outer.SpanId, pipeline.ParentSpanId);
        Assert.Equal(outer.TraceId, pipeline.TraceId);

        outer.Stop();
    }
}
