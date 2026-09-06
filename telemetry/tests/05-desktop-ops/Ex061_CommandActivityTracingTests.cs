using System.Diagnostics;
using System.Windows.Threading;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex061_CommandActivityTracingTests
{
    private static async Task<(List<Activity> Spans, string Result, string? CurrentInsideSave)> Run(
        Dispatcher dispatcher)
    {
        var exported = new List<Activity>();
        string? insideSave = null;

        using (var provider = Ex061_CommandActivityTracing.Build(exported))
        {
            var result = await Ex061_CommandActivityTracing.ExecuteSaveAsync(
                dispatcher,
                async () =>
                {
                    // Deliberately on a thread-pool thread, exactly as a persistence layer
                    // would be. Nothing here was told about the command.
                    await Task.Run(() => insideSave = Activity.Current?.DisplayName);

                    return await Ex061_CommandActivityTracing.PersistAsync();
                });

            provider.ForceFlush();

            return (exported, result, insideSave);
        }
    }

    [WpfFact]
    public async Task The_command_produces_one_span_covering_all_of_it()
    {
        using var ctx = new TelemetryContext();

        var (spans, result, _) = await Run(Dispatcher.CurrentDispatcher);

        Assert.Equal("saved", result);
        Assert.Single(spans, s => s.DisplayName == Ex061_CommandActivityTracing.CommandSpanName);
    }

    [WpfFact]
    public async Task Adversarial_A_The_span_is_current_on_the_thread_pool_thread_too()
    {
        // Better news than most people expect, and measured rather than assumed:
        // Activity.Current is AsyncLocal and rides on the ExecutionContext, which every
        // hop here captures. A command that starts on the UI thread, works on a pool
        // thread and comes back stays ONE trace, with no correlation id threaded through
        // any signature.
        using var ctx = new TelemetryContext();

        var (_, _, insideSave) = await Run(Dispatcher.CurrentDispatcher);

        Assert.Equal(Ex061_CommandActivityTracing.CommandSpanName, insideSave);
    }

    [WpfFact]
    public async Task Adversarial_B_A_span_started_by_the_work_is_a_child_not_a_root()
    {
        // The proof that the context really flowed rather than being re-established. A
        // persistence layer that knows nothing about the command still ends up inside it.
        using var ctx = new TelemetryContext();

        var (spans, _, _) = await Run(Dispatcher.CurrentDispatcher);

        var command = Assert.Single(spans, s => s.DisplayName == Ex061_CommandActivityTracing.CommandSpanName);
        var work = Assert.Single(spans, s => s.DisplayName == Ex061_CommandActivityTracing.WorkSpanName);

        Assert.Equal(command.SpanId, work.ParentSpanId);
        Assert.Equal(command.TraceId, work.TraceId);
    }

    [WpfFact]
    public async Task Adversarial_C_The_command_really_finishes_on_the_ui_thread()
    {
        // Without this the row would pass just as well for a command that never comes
        // back - and a command that never comes back cannot update the view, which is the
        // entire reason a desktop application has a dispatcher.
        using var ctx = new TelemetryContext();

        var (spans, _, _) = await Run(Dispatcher.CurrentDispatcher);

        var command = Assert.Single(spans, s => s.DisplayName == Ex061_CommandActivityTracing.CommandSpanName);
        Assert.Equal(
            "True",
            command.GetTagItem(Ex061_CommandActivityTracing.CompletedOnUiThreadTag)?.ToString());
    }

    [WpfFact]
    public async Task Adversarial_D_Nothing_is_left_current_afterwards()
    {
        // Row 016's discipline where it matters most. A UI thread lives for hours and runs
        // thousands of commands; one that leaks its span makes every later command a child
        // of it, and the trace grows until the backend refuses it.
        using var ctx = new TelemetryContext();

        await Run(Dispatcher.CurrentDispatcher);

        Assert.Null(Activity.Current);
    }
}
