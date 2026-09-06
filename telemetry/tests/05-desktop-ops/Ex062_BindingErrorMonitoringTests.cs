using System.Windows;
using System.Windows.Data;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex062_BindingErrorMonitoringTests
{
    /// <summary>
    /// A binding target. Measured 2026-09-06: a plain DependencyObject reports binding
    /// failures exactly as a FrameworkElement does - no Window and no visual tree needed.
    /// </summary>
    private sealed class Target : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(Target));

        public string? Value
        {
            get => (string?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }

    private sealed record Source(string Real);

    private static void Bind(string path)
    {
        var target = new Target();

        BindingOperations.SetBinding(
            target, Target.ValueProperty, new Binding(path) { Source = new Source("here") });

        // Touching the value is what makes the engine resolve the path.
        _ = target.Value;
    }

    [WpfFact]
    public void A_failing_binding_produces_one_record_carrying_the_engines_message()
    {
        using var logs = new LogProbe();

        using (Ex062_BindingErrorMonitoring.CaptureBindingErrors(
            logs.For(Ex062_BindingErrorMonitoring.CategoryName)))
        {
            Bind("ThisPropertyDoesNotExist");
        }

        var record = Assert.Single(logs.Records);
        Assert.Equal(LogLevel.Error, record.Level);

        var message = LogProbe.Field(record, Ex062_BindingErrorMonitoring.ErrorField);
        Assert.NotNull(message);
        Assert.Contains("ThisPropertyDoesNotExist", message);
    }

    [WpfFact]
    public void Adversarial_A_A_binding_that_resolves_produces_nothing()
    {
        // The paired half. A listener that logs everything the trace source says - or that
        // logs unconditionally - turns a quiet application into a noisy one and teaches
        // everyone to filter the category out.
        using var logs = new LogProbe();

        using (Ex062_BindingErrorMonitoring.CaptureBindingErrors(
            logs.For(Ex062_BindingErrorMonitoring.CategoryName)))
        {
            Bind(nameof(Source.Real));
        }

        Assert.Empty(logs.Records);
    }

    [WpfFact]
    public void Adversarial_B_Every_failure_is_reported_not_just_the_first()
    {
        // This is per failure, not once ever. A view with twelve broken bindings has
        // twelve bugs, and a capture that reports one of them has hidden eleven.
        using var logs = new LogProbe();

        using (Ex062_BindingErrorMonitoring.CaptureBindingErrors(
            logs.For(Ex062_BindingErrorMonitoring.CategoryName)))
        {
            Bind("MissingOne");
            Bind("MissingTwo");
        }

        Assert.Equal(2, logs.Records.Count);
    }

    [WpfFact]
    public void Adversarial_C_Disposing_the_capture_stops_it()
    {
        // The trace source is process-global. A capture that never detaches leaves a
        // listener attached to a static for the life of the process - which in a test
        // suite means every later test reports into whichever logger the first one used.
        using var logs = new LogProbe();

        using (Ex062_BindingErrorMonitoring.CaptureBindingErrors(
            logs.For(Ex062_BindingErrorMonitoring.CategoryName)))
        {
            Bind("MissingWhileWatching");
        }

        Bind("MissingAfterwards");

        var record = Assert.Single(logs.Records);
        Assert.Contains(
            "MissingWhileWatching",
            LogProbe.Field(record, Ex062_BindingErrorMonitoring.ErrorField)!);
    }

    [WpfFact]
    public void The_report_uses_a_constant_template()
    {
        using var logs = new LogProbe();

        using (Ex062_BindingErrorMonitoring.CaptureBindingErrors(
            logs.For(Ex062_BindingErrorMonitoring.CategoryName)))
        {
            Bind("MissingOne");
            Bind("MissingTwo");
        }

        Assert.Equal(
            [Ex062_BindingErrorMonitoring.Template, Ex062_BindingErrorMonitoring.Template],
            logs.Records.Select(LogProbe.OriginalFormat));
    }
}
