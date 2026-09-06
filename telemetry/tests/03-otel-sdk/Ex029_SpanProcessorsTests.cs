using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using OpenTelemetry;
using OpenTelemetry.Trace;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex029_SpanProcessorsTests
{
    private static Activity DoWork() =>
        Ex029_SpanProcessors.Source.StartActivity(Ex029_SpanProcessors.WorkSpanName)
        ?? throw new InvalidOperationException("no provider is listening - the test is set up wrong");

    [Fact]
    public void One_processor_sees_the_start_before_the_work_and_the_end_after_it()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();
        var log = new List<string>();

        using (var provider = Ex029_SpanProcessors.Build(
            exported, Ex029_SpanProcessors.CreateEnrichingProcessor("p", log)))
        {
            var activity = DoWork();
            log.Add("body");
            activity.Dispose();
            provider.ForceFlush();
        }

        Assert.Equal(["p:start", "body", "p:end"], log);
    }

    [Fact]
    public void Both_of_the_processors_tags_reach_the_exported_span()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();
        var log = new List<string>();

        using (var provider = Ex029_SpanProcessors.Build(
            exported, Ex029_SpanProcessors.CreateEnrichingProcessor("p", log)))
        {
            DoWork().Dispose();
            provider.ForceFlush();
        }

        var span = Assert.Single(exported);
        Assert.Equal("yes", span.GetTagItem(Ex029_SpanProcessors.StartedTagPrefix + "p")?.ToString());
        Assert.Equal("yes", span.GetTagItem(Ex029_SpanProcessors.EndedTagPrefix + "p")?.ToString());
    }

    [Fact]
    public void Adversarial_A_OnStart_runs_on_an_unfinished_span_and_OnEnd_on_a_finished_one()
    {
        // Proves the hooks are where the names claim. A processor that did all its work
        // in OnEnd would satisfy the tag facts above and see a duration on both hooks;
        // one wired to the wrong event would see none on either.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();
        var log = new List<string>();
        var durations = new List<TimeSpan>();

        using (var provider = Ex029_SpanProcessors.Build(
            exported, new DurationSpy(durations)))
        {
            DoWork().Dispose();
            provider.ForceFlush();
        }

        Assert.Equal(2, durations.Count);
        Assert.Equal(TimeSpan.Zero, durations[0]);
        Assert.NotEqual(TimeSpan.Zero, durations[1]);
        Assert.Empty(log);
    }

    [Fact]
    public void Adversarial_B_Both_hooks_run_in_registration_order_the_chain_does_not_unwind()
    {
        // The clause that surprises everyone who has written middleware. An ASP.NET
        // pipeline nests - outer-in, inner-in, inner-out, outer-out. A processor chain
        // does not: OTel composes processors into a list and walks it head to tail for
        // BOTH hooks.
        //
        // So "the last processor gets the final say" is only true for OnEnd because it
        // is last in the list, not because anything unwound - and a processor added
        // after the exporter runs after the export rather than around it.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();
        var log = new List<string>();

        using (var provider = Ex029_SpanProcessors.Build(
            exported,
            Ex029_SpanProcessors.CreateEnrichingProcessor("first", log),
            Ex029_SpanProcessors.CreateEnrichingProcessor("second", log)))
        {
            DoWork().Dispose();
            provider.ForceFlush();
        }

        Assert.Equal(["first:start", "second:start", "first:end", "second:end"], log);
    }

    /// <summary>
    /// Records the span's duration as each hook sees it. Deliberately not the
    /// exercise's own processor: this fact has to be able to fail independently of it.
    /// </summary>
    private sealed class DurationSpy(IList<TimeSpan> durations) : OpenTelemetry.BaseProcessor<Activity>
    {
        public override void OnStart(Activity data) => durations.Add(data.Duration);

        public override void OnEnd(Activity data) => durations.Add(data.Duration);
    }
}
