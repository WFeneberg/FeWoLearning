// Exercise 012 - View Aware Callbacks (beginner).
// Goal:   Learn the two callbacks a Screen gets about its own view, and that they fire at two
//         genuinely different moments.
// Passes: dotnet test --filter FullyQualifiedName~Ex012_

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

    protected override void OnViewAttached(object view, object context)
    {
        AttachCount++;
        LastAttachedView = view;
        LastAttachedContext = context;
    }

    protected override void OnViewLoaded(object view)
    {
        LoadCount++;
        LastLoadedView = view;
    }
}
