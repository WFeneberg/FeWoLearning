// Exercise 027 - Action Special Values (beginner).
// Goal:   Learn the small set of magic tokens an attach expression can pass instead of a literal
//         or an element: $eventArgs, $dataContext, $source, $view - four of Caliburn's five
//         documented SpecialValues - plus $this, which is not one of those five keys yet still
//         resolves.
// Drills: cal:Message.Attach passing several special values to one method in a single call;
//         MessageBinder.SpecialValues being a lowercase-keyed, process-global dictionary this
//         exercise only READS, never mutates (ex069 owns adding to it); that $source is the
//         clicked element itself, $dataContext is the view model, and $view - measured below -
//         is NOT simply "the containing view".
// Passes: dotnet test --filter FullyQualifiedName~Ex027_
//
// Measured on this machine (Caliburn.Micro 5.0.258): MessageBinder.SpecialValues holds exactly
// five keys, stored lowercase - $datacontext, $eventargs, $executioncontext, $source, $view.
// $executioncontext is out of scope here - it was not measured on this machine, so nothing is
// claimed about what it yields.
//
// The $view result is the sharp one, and it is NOT what a plain DataContext assignment would
// give you. Wiring the view model the way ex025/ex026 do - view.DataContext = viewModel - leaves
// $view identical to $source (both the clicked button): ActionMessage's target-resolution walk
// (Action.HasTargetSet) finds no ancestor with an explicitly-set target anywhere, falls back to
// "resolve against source.DataContext", and in that fallback path View is set to source itself.
// $view only becomes a DIFFERENT element once something up the tree has set an explicit target -
// this exercise uses Caliburn.Micro.Action.SetTarget(view, viewModel) (ex028's own mechanism) on
// the view's ROOT specifically because that is what makes $view resolve to that root: clicking a
// button inside it, with cal:Message.Attach="CaptureAll($eventArgs, $dataContext, $source, $view,
// $this)", measured $eventArgs as the Click's own RoutedEventArgs, $dataContext and $this both as
// the view model instance, $source as the Button that was clicked, and $view as the root - a
// DIFFERENT object from $source. $this is not one of the five SpecialValues keys (verified
// against the dictionary above), so this exercise only reports that it resolves, not why.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex027_ActionSpecialValues
{
    /// <summary>
    /// Builds a view with a single button whose cal:Message.Attach passes $eventArgs,
    /// $dataContext, $source, $view and $this - in that order - to CaptureAll, and wires the
    /// supplied viewModel onto the view's root via Caliburn.Micro.Action.SetTarget (not a plain
    /// DataContext assignment - that is what makes $view resolve to the root instead of merely
    /// repeating $source).
    /// </summary>
    public (FrameworkElement View, System.Windows.Controls.Button Button) BuildView(object viewModel) =>
        throw new NotImplementedException("TODO: Ex027 - XamlReader.Parse a Button attached to CaptureAll($eventArgs, $dataContext, $source, $view, $this); Caliburn.Micro.Action.SetTarget(view, viewModel)");
}

/// <summary>A view model whose one method records every special value it was handed, in argument order.</summary>
public class Ex027_Vm : PropertyChangedBase
{
    public RoutedEventArgs? LastEventArgs { get; private set; }
    public object? LastDataContext { get; private set; }
    public object? LastSource { get; private set; }
    public object? LastView { get; private set; }
    public object? LastThis { get; private set; }
    public int CallCount { get; private set; }

    public void CaptureAll(RoutedEventArgs eventArgs, object dataContext, object source, object view, object self)
    {
        LastEventArgs = eventArgs;
        LastDataContext = dataContext;
        LastSource = source;
        LastView = view;
        LastThis = self;
        CallCount++;
    }
}
