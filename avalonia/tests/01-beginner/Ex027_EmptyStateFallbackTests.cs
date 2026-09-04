using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex027_EmptyStateFallbackTests
{
    private static (Ex027_EmptyStateFallback View, Ex027_EmptyStateFallbackViewModel Vm) Arrange()
    {
        var vm = new Ex027_EmptyStateFallbackViewModel();
        var view = ViewHarness.Show(new Ex027_EmptyStateFallback { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Both_Elements_Exist()
    {
        var (view, _) = Arrange();

        Assert.NotNull(view.FindControl<TextBlock>("EmptyMessage"));
        Assert.NotNull(view.FindControl<ItemsControl>("ItemsPanel"));
    }

    // Discriminator: a static IsVisible="True"/"False" on either element
    // matches exactly one of these three states. Driving the collection
    // empty -> non-empty -> empty again means only a live binding to Count
    // can pass all three, and the two elements must flip in OPPOSITE
    // directions every time.
    [AvaloniaFact]
    public void Visibility_Flips_Oppositely_As_The_Collection_Empties_And_Fills()
    {
        var (view, vm) = Arrange();
        var emptyMessage = view.FindControl<TextBlock>("EmptyMessage")!;
        var itemsPanel = view.FindControl<ItemsControl>("ItemsPanel")!;

        // Starting empty.
        Assert.True(emptyMessage.IsVisible);
        Assert.False(itemsPanel.IsVisible);

        // Add an item: non-empty.
        vm.Items.Add("Milk");
        Dispatcher.UIThread.RunJobs();
        Assert.False(emptyMessage.IsVisible);
        Assert.True(itemsPanel.IsVisible);

        // Remove it again: back to empty.
        vm.Items.Remove("Milk");
        Dispatcher.UIThread.RunJobs();
        Assert.True(emptyMessage.IsVisible);
        Assert.False(itemsPanel.IsVisible);
    }
}
