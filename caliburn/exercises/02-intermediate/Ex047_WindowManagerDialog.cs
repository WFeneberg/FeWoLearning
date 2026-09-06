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
//
// Measured on this machine (Caliburn.Micro 5.0.258): IWindowManager has exactly three methods -
// ShowDialogAsync(object, object, IDictionary<string,object>), ShowWindowAsync(...),
// ShowPopupAsync(...) - all declared directly on the interface, no extension-method surface adds
// to it (checked both the interface and every static class in the assembly before writing this).

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

    /// <summary>The TODO: increment TimesShown, then show rootModel via the INJECTED
    /// _windowManager.ShowDialogAsync(rootModel, null, settings), returning whatever it
    /// resolves to unchanged.</summary>
    public Task<bool?> ShowAsync(object rootModel, IDictionary<string, object>? settings = null) =>
        throw new NotImplementedException("TODO: Ex047 - _windowManager.ShowDialogAsync(rootModel, null, settings), counting TimesShown");
}
