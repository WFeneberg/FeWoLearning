using System.Windows.Threading;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex060_UnhandledExceptionCaptureTests
{
    [WpfFact]
    public async Task An_exception_escaping_a_dispatcher_callback_is_captured()
    {
        using var logs = new LogProbe();
        var dispatcher = Dispatcher.CurrentDispatcher;

        Ex060_UnhandledExceptionCapture.AttachTo(
            dispatcher, logs.For(Ex060_UnhandledExceptionCapture.CategoryName));

        await dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            // A block body, not `() => throw`: an expression-bodied throw cannot be
            // inferred as an Action here and fails with CS8917.
            () => { throw new InvalidOperationException("the view model exploded"); });

        var record = Assert.Single(logs.Records);
        Assert.Equal(LogLevel.Critical, record.Level);
        Assert.Equal(
            Ex060_UnhandledExceptionCapture.DispatcherSource,
            LogProbe.Field(record, Ex060_UnhandledExceptionCapture.SourceField));
        Assert.Equal("the view model exploded", record.Exception?.Message);
    }

    [WpfFact]
    public async Task Adversarial_A_The_dispatcher_survives_and_keeps_working()
    {
        // The clause that decides whether the application lives. An unhandled dispatcher
        // exception tears the process down; marking it handled turns a crash into a
        // recorded fault - and is also a decision to keep running with state you no longer
        // trust, which is why it belongs at the top level and nowhere else.
        using var logs = new LogProbe();
        var dispatcher = Dispatcher.CurrentDispatcher;

        Ex060_UnhandledExceptionCapture.AttachTo(
            dispatcher, logs.For(Ex060_UnhandledExceptionCapture.CategoryName));

        await dispatcher.BeginInvoke(
            DispatcherPriority.Normal, () => { throw new InvalidOperationException("boom"); });

        var ranAfterwards = false;
        await dispatcher.BeginInvoke(DispatcherPriority.Normal, () => ranAfterwards = true);

        Assert.True(ranAfterwards, "the dispatcher has to still be usable");
        Assert.Single(logs.Records);
    }

    [WpfFact]
    public void Adversarial_B_The_same_instance_reaching_a_second_hook_records_nothing_more()
    {
        // The title's "one record, not three or none". A desktop application has several
        // places an escaping exception can surface, and one failure often reaches more
        // than one of them. Three reports of one crash is not three times the information;
        // it is a support engineer counting an incident three times, and a deduplication
        // rule somebody has to invent later, in a query, from worse data.
        using var logs = new LogProbe();
        var logger = logs.For(Ex060_UnhandledExceptionCapture.CategoryName);
        var failure = new InvalidOperationException("one failure");

        Ex060_UnhandledExceptionCapture.Capture(
            logger, failure, Ex060_UnhandledExceptionCapture.DispatcherSource);
        Ex060_UnhandledExceptionCapture.Capture(
            logger, failure, Ex060_UnhandledExceptionCapture.TaskSource);
        Ex060_UnhandledExceptionCapture.Capture(
            logger, failure, Ex060_UnhandledExceptionCapture.DomainSource);

        var record = Assert.Single(logs.Records);
        Assert.Equal(
            Ex060_UnhandledExceptionCapture.DispatcherSource,
            LogProbe.Field(record, Ex060_UnhandledExceptionCapture.SourceField));
    }

    [WpfFact]
    public void Adversarial_C_A_different_instance_is_recorded_normally()
    {
        // What makes deduplicating by INSTANCE the right rule. "Report only the first
        // exception" loses every later failure; "report only one per message" merges
        // genuinely separate incidents that happen to say the same thing.
        //
        // These two say exactly the same thing and are two different failures.
        using var logs = new LogProbe();
        var logger = logs.For(Ex060_UnhandledExceptionCapture.CategoryName);

        Ex060_UnhandledExceptionCapture.Capture(
            logger, new InvalidOperationException("the same words"),
            Ex060_UnhandledExceptionCapture.DispatcherSource);
        Ex060_UnhandledExceptionCapture.Capture(
            logger, new InvalidOperationException("the same words"),
            Ex060_UnhandledExceptionCapture.DispatcherSource);

        Assert.Equal(2, logs.Records.Count);
    }

    [WpfFact]
    public void The_report_uses_a_constant_template()
    {
        // Row 001, at the point where it matters most: crash reports are the records you
        // most want to group, count and alert on.
        using var logs = new LogProbe();
        var logger = logs.For(Ex060_UnhandledExceptionCapture.CategoryName);

        Ex060_UnhandledExceptionCapture.Capture(
            logger, new InvalidOperationException("a"), Ex060_UnhandledExceptionCapture.DispatcherSource);
        Ex060_UnhandledExceptionCapture.Capture(
            logger, new InvalidOperationException("b"), Ex060_UnhandledExceptionCapture.TaskSource);

        Assert.Equal(
            [Ex060_UnhandledExceptionCapture.Template, Ex060_UnhandledExceptionCapture.Template],
            logs.Records.Select(LogProbe.OriginalFormat));
    }
}
