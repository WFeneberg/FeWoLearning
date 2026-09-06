// Exercise 070 - PresentationTraceSources and catching a silent binding failure (intermediate).
// Goal:   A Binding whose Path is simply wrong never throws - it fails silently, leaving the
//         target showing nothing useful, and the only way to notice is to actually listen for the
//         diagnostic WPF already emits internally. Measured directly building this row: adding a
//         TraceListener alone changes nothing - PresentationTraceSources.Refresh() has to be
//         called to make the trace configuration live, AND it has to be called BEFORE the switch
//         level below is raised, not after - calling Refresh() a second time (or first) resets the
//         level straight back down, undoing a raise that came before it.
// Drills: PresentationTraceSources.DataBindingSource's Listeners/Switch plus Refresh() actually
//         producing captured diagnostic text for a broken binding (not merely leaving a listener
//         "wired up" with nothing ever reaching it), and PresentationTraceSources.SetTraceLevel/
//         GetTraceLevel - the attached property that singles out one binding's own diagnostics.
// Passes: dotnet test --filter FullyQualifiedName~Ex070_

using System.Diagnostics;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex070_BindingDiagnostics
{
    /// <summary>
    /// Turns on PresentationTraceSources' binding-error diagnostics and adds <paramref name="listener"/>
    /// so a subsequent binding failure becomes observable instead of silent.
    /// </summary>
    public static void EnableBindingErrorCapture(TraceListener listener)
        => throw new NotImplementedException("TODO: Ex070 - three things need to happen, in this order and no other: PresentationTraceSources must reload its trace configuration (doing this AFTER the next step instead resets what the next step just raised), the DataBindingSource switch's level needs raising high enough that Error-level diagnostics actually get through, and `listener` needs to end up in that same source's collection of listeners");

    /// <summary>
    /// Ready to use - teardown for <see cref="EnableBindingErrorCapture"/>. Deliberately does not
    /// lower Switch.Level back down: PresentationTraceSources is process-global and shared with
    /// every other test in this run, so lowering it here could silence a DIFFERENT test's
    /// diagnostics if this one happens to run first - removing this one listener is all teardown
    /// actually needs to do.
    /// </summary>
    public static void DisableBindingErrorCapture(TraceListener listener)
        => PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);

    /// <summary>
    /// Marks <paramref name="binding"/> for its own detailed diagnostics via
    /// PresentationTraceSources' attached TraceLevel property - the mechanism that lets a caller
    /// single out ONE binding among many once the source-level switch above is already flooding a
    /// listener with everyone's.
    /// </summary>
    public static void MarkForDetailedDiagnostics(BindingBase binding)
        => throw new NotImplementedException("TODO: Ex070 - write PresentationTraceLevel.High onto `binding`'s own attached TraceLevel property (the same property PresentationTraceSources exposes a getter for elsewhere in this class) - not onto a different Binding instance, and not onto the source-level switch above");
}

/// <summary>
/// The model behind the label. Ready to use - no INotifyPropertyChanged here, deliberately: this
/// row's tests only ever read RealProperty once, right after binding, so there is nothing for a
/// change notification to do.
/// </summary>
public sealed class Ex070_DiagnosticsSource
{
    public string RealProperty => "actual value";
}
