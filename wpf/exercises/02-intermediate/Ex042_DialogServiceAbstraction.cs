// Exercise 042 - The dialog service seam (intermediate).
// Goal:   Get a real user-facing decision (confirm or cancel) out of a view model without the
//         view model ever touching MessageBox itself, so the view model's logic can be
//         asserted headlessly - no window, no modal loop, nothing that can hang a test run.
// Drills: an application-defined IDialogService interface standing between a view model and
//         the concrete dialog mechanism, and a view model that depends on that interface
//         (never on MessageBox) so a test double can stand in for it.
// Passes: dotnet test --filter FullyQualifiedName~Ex042_

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

/// <summary>
/// The seam: anything a view model needs to ask the user, expressed without any concrete
/// dialog technology in sight. Ready to use.
/// </summary>
public interface Ex042_IDialogService
{
    /// <summary>Asks a yes/no question; true means the user confirmed.</summary>
    bool Confirm(string message);

    /// <summary>Tells the user something; no answer expected.</summary>
    void Notify(string message);
}

/// <summary>
/// The real, production implementation - ready to use, and deliberately NOT exercised by any
/// test in this row: MessageBox.Show pumps its own modal message loop on the calling thread,
/// so a test invoking this on the harness's single STA dispatcher would hang the run rather
/// than fail it. Testing Ex042_ItemViewModel through Ex042_IDialogService instead - never
/// through this class - is exactly what the abstraction buys: full coverage of the decision
/// logic below with no window ever opening.
/// </summary>
public sealed class Ex042_MessageBoxDialogService : Ex042_IDialogService
{
    public bool Confirm(string message)
        => MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

    public void Notify(string message)
        => MessageBox.Show(message, "Notice", MessageBoxButton.OK);
}

/// <summary>
/// Deletes an item, but only after the user confirms - the everyday shape a dialog
/// abstraction exists for.
/// </summary>
public sealed class Ex042_ItemViewModel
{
    private readonly Ex042_IDialogService _dialogService;

    public Ex042_ItemViewModel(string name, Ex042_IDialogService dialogService)
    {
        Name = name;
        _dialogService = dialogService;
    }

    public string Name { get; }

    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Asks dialogService.Confirm($"Delete '{Name}'?"). If confirmed: mark IsDeleted true,
    /// call dialogService.Notify($"'{Name}' deleted.") and return true. If declined: leave
    /// IsDeleted false, call Notify nothing, and return false.
    /// </summary>
    public bool Delete()
        => throw new NotImplementedException("TODO: Ex042 - call _dialogService.Confirm($\"Delete '{Name}'?\"); if true, set IsDeleted = true, call _dialogService.Notify($\"'{Name}' deleted.\") and return true; if false, leave IsDeleted false, call Notify nothing, and return false");
}
