using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex042_DialogServiceAbstractionTests : WpfTestContext
{
    // A pure test double - no window, no MessageBox, nothing that can block the dispatcher.
    // This is the entire point of the row: Ex042_ItemViewModel is asserted without a window
    // because it depends on Ex042_IDialogService, never on Ex042_MessageBoxDialogService.
    private sealed class FakeDialogService(bool confirmResult) : Ex042_IDialogService
    {
        public List<string> ConfirmMessages { get; } = [];
        public List<string> NotifyMessages { get; } = [];

        public bool Confirm(string message)
        {
            ConfirmMessages.Add(message);
            return confirmResult;
        }

        public void Notify(string message) => NotifyMessages.Add(message);
    }

    [WpfTheory]
    [InlineData("Report")]
    [InlineData("Invoice-77")]
    public void Delete_Asks_For_Confirmation_Naming_The_Item(string itemName)
    {
        var dialog = new FakeDialogService(confirmResult: true);
        var item = new Ex042_ItemViewModel(itemName, dialog);

        item.Delete();

        Assert.Equal(new[] { $"Delete '{itemName}'?" }, dialog.ConfirmMessages);
    }

    [WpfFact]
    public void Delete_Returns_True_Marks_Deleted_And_Notifies_When_Confirmed()
    {
        var dialog = new FakeDialogService(confirmResult: true);
        var item = new Ex042_ItemViewModel("Ledger", dialog);

        var result = item.Delete();

        Assert.True(result);
        Assert.True(item.IsDeleted);
        Assert.Equal(new[] { "'Ledger' deleted." }, dialog.NotifyMessages);
    }

    [WpfFact]
    public void Delete_Returns_False_Leaves_The_Item_And_Does_Not_Notify_When_Declined()
    {
        var dialog = new FakeDialogService(confirmResult: false);
        var item = new Ex042_ItemViewModel("Ledger", dialog);

        var result = item.Delete();

        // Load-bearing against a Delete() that ignores Confirm's answer and deletes anyway,
        // or that notifies regardless of the outcome.
        Assert.False(result);
        Assert.False(item.IsDeleted);
        Assert.Empty(dialog.NotifyMessages);
    }
}
