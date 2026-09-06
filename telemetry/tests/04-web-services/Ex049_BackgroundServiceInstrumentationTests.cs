using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex049_BackgroundServiceInstrumentationTests
{
    private static async Task<List<Activity>> Run(IEnumerable<string> items, Func<string, Task>? work = null)
    {
        var exported = new List<Activity>();

        using var provider = Ex049_BackgroundServiceInstrumentation.Build(exported);
        await Ex049_BackgroundServiceInstrumentation.RunAsync(items, work ?? (_ => Task.CompletedTask));
        provider.ForceFlush();

        return exported;
    }

    [Fact]
    public async Task Every_item_gets_its_own_span_naming_the_item()
    {
        using var ctx = new TelemetryContext();

        var spans = await Run(["a", "b", "c"]);

        Assert.Equal(3, spans.Count);
        Assert.All(spans, s => Assert.Equal(
            Ex049_BackgroundServiceInstrumentation.IterationSpanName, s.DisplayName));
        Assert.Equal(
            ["a", "b", "c"],
            spans.Select(s => s.GetTagItem(Ex049_BackgroundServiceInstrumentation.ItemAttribute)?.ToString()));
    }

    [Fact]
    public async Task Adversarial_A_Every_iteration_is_its_own_root_on_its_own_trace()
    {
        // The row. The obvious thing to do in a worker is open a span when the service
        // starts, and it gets worse the longer the process lives: one span still running
        // after a week, one trace holding every item the worker has ever touched, and a
        // backend that will not show you any of it because the trace has no end and no
        // reasonable size. Nothing errors. It simply never appears.
        //
        // A unit of work is a trace. For a worker that unit is one item.
        //
        // The loop runs INSIDE an ambient span deliberately, and that is what gives this
        // fact teeth. Measured 2026-09-06: with nothing ambient, an implementation that
        // simply omits the explicit `parentContext: default` produces roots anyway, so a
        // test that runs on a bare thread cannot tell the two apart. Under an ambient
        // span - a worker with an outer "service started" activity, or an iteration that
        // leaked its own - only the explicit form still roots.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using (var provider = Ex049_BackgroundServiceInstrumentation.Build(exported))
        using (var ambient = Ex049_BackgroundServiceInstrumentation.Source.StartActivity("worker"))
        {
            Assert.NotNull(ambient);
            await Ex049_BackgroundServiceInstrumentation.RunAsync(["a", "b", "c"], _ => Task.CompletedTask);
            provider.ForceFlush();
        }

        var iterations = exported
            .Where(s => s.DisplayName == Ex049_BackgroundServiceInstrumentation.IterationSpanName)
            .ToArray();

        Assert.Equal(3, iterations.Length);
        Assert.All(iterations, s => Assert.Equal(default, s.ParentSpanId));
        Assert.Equal(3, iterations.Select(s => s.TraceId).Distinct().Count());
    }

    [Fact]
    public async Task Adversarial_B_A_failing_item_is_recorded_and_the_loop_carries_on()
    {
        // The difference between instrumentation and control flow, which row 018 made
        // about rethrowing and this one makes about surviving. A worker that dies on the
        // first bad item is a worker that stops; one that swallows the failure silently is
        // worse. Record it on that item's span, and go on.
        using var ctx = new TelemetryContext();

        var spans = await Run(
            ["a", "boom", "c"],
            item => item == "boom"
                ? Task.FromException(new InvalidOperationException("item is poison"))
                : Task.CompletedTask);

        Assert.Equal(3, spans.Count);

        var failed = Assert.Single(spans, s =>
            s.GetTagItem(Ex049_BackgroundServiceInstrumentation.ItemAttribute)?.ToString() == "boom");
        Assert.Equal(ActivityStatusCode.Error, failed.Status);
        Assert.Equal("item is poison", failed.StatusDescription);

        Assert.All(
            spans.Where(s => s != failed),
            s => Assert.NotEqual(ActivityStatusCode.Error, s.Status));
    }

    [Fact]
    public async Task Adversarial_C_No_ambient_activity_survives_an_iteration()
    {
        // The leak that would make the other facts lie. Activity.Current is ambient and
        // AsyncLocal: an iteration that leaves its span open makes the NEXT one a child of
        // it, and the roots quietly turn into the single growing trace this row exists to
        // prevent.
        using var ctx = new TelemetryContext();
        var seenInsideWork = new List<string?>();

        var spans = await Run(
            ["a", "b"],
            _ =>
            {
                seenInsideWork.Add(Activity.Current?.DisplayName);
                return Task.CompletedTask;
            });

        // Inside the work there IS a current activity - the iteration's own.
        Assert.Equal(
            [Ex049_BackgroundServiceInstrumentation.IterationSpanName,
             Ex049_BackgroundServiceInstrumentation.IterationSpanName],
            seenInsideWork);

        // And none of it is left over afterwards.
        Assert.Null(Activity.Current);
        Assert.Equal(2, spans.Count);
    }
}
