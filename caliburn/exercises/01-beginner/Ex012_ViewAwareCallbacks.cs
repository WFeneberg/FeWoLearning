// Exercise 012 - View Aware Callbacks (beginner).
// Goal:   Learn the two callbacks a Screen gets about its own view, and that they fire at two
//         genuinely different moments.
// Drills: OnViewAttached firing immediately and synchronously from AttachView, versus
//         OnViewLoaded firing only once the view has really been loaded - plus views being
//         stored keyed by context.
// Passes: dotnet test --filter FullyQualifiedName~Ex012_
//
// AttachView(view, context) - reached through the IViewAware interface, never directly off a
// Screen-typed reference - fires OnViewAttached(view, context) IMMEDIATELY and synchronously,
// with no window involved at all. OnViewLoaded(view) is a completely different moment: it does
// NOT fire on attach, and Measure/Arrange (this track's Layout helper) is not enough either - it
// needs the view's real Loaded event, which this harness supplies two ways: CaliburnViewContext's
// Show(view) (a real window) or Load(view) (raises Loaded by hand). Views are stored keyed by
// context: GetView(context) returns only the view attached under that exact context - attach
// under "Edit" and GetView() (which means GetView(null)) comes back null.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex012_ViewAwareCallbacks : Screen
{
    /// <summary>How many times OnViewAttached actually ran.</summary>
    public int AttachCount { get; private set; }

    /// <summary>How many times OnViewLoaded actually ran.</summary>
    public int LoadCount { get; private set; }

    /// <summary>The view passed to the most recent OnViewAttached call, if any.</summary>
    public object? LastAttachedView { get; private set; }

    /// <summary>The context passed to the most recent OnViewAttached call, if any.</summary>
    public object? LastAttachedContext { get; private set; }

    /// <summary>The view passed to the most recent OnViewLoaded call, if any.</summary>
    public object? LastLoadedView { get; private set; }

    protected override void OnViewAttached(object view, object context) =>
        throw new NotImplementedException("TODO: Ex012 - increment AttachCount and record view/context");

    protected override void OnViewLoaded(object view) =>
        throw new NotImplementedException("TODO: Ex012 - increment LoadCount and record view");
}
