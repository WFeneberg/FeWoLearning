// Exercise 047 - Window Manager Dialog (intermediate).
// Goal:   Showing a dialog through an injected IWindowManager and awaiting its outcome.
//         ShowDialogAsync is MODAL - it parks the calling thread in a nested dispatcher frame
//         until the dialog closes, so awaiting it genuinely means waiting for a real close, not
//         just forwarding a Task that happens to already be finished.
// Drills: calling windowManager.ShowDialogAsync on the INJECTED instance (not a fresh one built
//         inside the method - constructor injection only means something if the injected object
//         is actually the one used) and returning exactly what it resolves to, while counting how
//         many times a dialog has been shown.
// Passes: dotnet test --filter FullyQualifiedName~Ex047_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>Shows a root model as a modal dialog through whichever IWindowManager was injected,
/// tracking how many dialogs it has shown.</summary>
public class Ex047_DialogHost
{
    private readonly IWindowManager _windowManager;

    public Ex047_DialogHost(IWindowManager windowManager) => _windowManager = windowManager;

    /// <summary>How many times ShowAsync has been called on this instance.</summary>
    public int TimesShown { get; private set; }

    public Task<bool?> ShowAsync(object rootModel, IDictionary<string, object>? settings = null)
    {
        TimesShown++;
        return _windowManager.ShowDialogAsync(rootModel, null, settings);
    }
}
