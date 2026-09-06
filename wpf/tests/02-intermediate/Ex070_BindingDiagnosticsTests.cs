using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex070_BindingDiagnosticsTests : WpfTestContext
{
    // Captures actual trace TEXT, not merely "was I called" - so a mutant that only flips some
    // flag on the listener rather than genuinely routing PresentationTraceSources output to it
    // cannot pass by faking the listener's own state.
    private sealed class CapturingListener : TraceListener
    {
        private readonly StringBuilder _sb = new();

        public string Captured => _sb.ToString();

        public override void Write(string? message) => _sb.Append(message);

        public override void WriteLine(string? message) => _sb.AppendLine(message);
    }

    private static TextBlock BindBrokenPath(string propertyName)
    {
        var target = new TextBlock();
        var source = new Ex070_DiagnosticsSource();
        target.DataContext = source;
        target.SetBinding(TextBlock.TextProperty, new Binding(propertyName));
        return target;
    }

    [WpfFact]
    public void A_Broken_Binding_Path_Is_Captured_Once_Diagnostics_Are_Enabled()
    {
        var listener = new CapturingListener();
        Ex070_BindingDiagnostics.EnableBindingErrorCapture(listener);
        try
        {
            var target = BindBrokenPath("NoSuchProperty");
            Layout(target);
            Pump();

            Assert.Contains("NoSuchProperty", listener.Captured);
        }
        finally
        {
            Ex070_BindingDiagnostics.DisableBindingErrorCapture(listener);
        }
    }

    [WpfFact]
    public void A_Different_Broken_Path_Is_Named_In_Its_Own_Captured_Message()
    {
        // Vary the input across call sites: a mutant that returns some fixed, hard-coded
        // diagnostic string regardless of what actually broke fails this against the test above.
        var listener = new CapturingListener();
        Ex070_BindingDiagnostics.EnableBindingErrorCapture(listener);
        try
        {
            var target = BindBrokenPath("AlsoMissing");
            Layout(target);
            Pump();

            Assert.Contains("AlsoMissing", listener.Captured);
            Assert.DoesNotContain("NoSuchProperty", listener.Captured);
        }
        finally
        {
            Ex070_BindingDiagnostics.DisableBindingErrorCapture(listener);
        }
    }

    [WpfFact]
    public void A_Correctly_Bound_Property_Produces_No_Diagnostic_Output_At_All()
    {
        var listener = new CapturingListener();
        Ex070_BindingDiagnostics.EnableBindingErrorCapture(listener);
        try
        {
            var target = BindBrokenPath(nameof(Ex070_DiagnosticsSource.RealProperty));
            Layout(target);
            Pump();

            Assert.Equal(string.Empty, listener.Captured);
            // Meanwhile the binding genuinely worked - not merely "no crash".
            Assert.Equal("actual value", target.Text);
        }
        finally
        {
            Ex070_BindingDiagnostics.DisableBindingErrorCapture(listener);
        }
    }

    [WpfFact]
    public void MarkForDetailedDiagnostics_Sets_The_Attached_TraceLevel_To_High_On_Only_That_Binding()
    {
        var marked = new Binding(nameof(Ex070_DiagnosticsSource.RealProperty));
        var untouched = new Binding(nameof(Ex070_DiagnosticsSource.RealProperty));

        Ex070_BindingDiagnostics.MarkForDetailedDiagnostics(marked);

        Assert.Equal(PresentationTraceLevel.High, PresentationTraceSources.GetTraceLevel(marked));
        // Proves this is a per-binding write, not some ambient/global level that would make every
        // binding read back High regardless of whether it was ever marked.
        Assert.Equal(PresentationTraceLevel.None, PresentationTraceSources.GetTraceLevel(untouched));
    }
}
