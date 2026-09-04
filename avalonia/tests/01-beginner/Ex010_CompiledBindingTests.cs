using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex010_CompiledBindingTests
{
    private static (Ex010_CompiledBinding View, Ex010_BookViewModel Vm) Arrange()
    {
        var vm = new Ex010_BookViewModel
        {
            Title = "Design Patterns",
            Author = new Ex010_AuthorViewModel { Name = "Erich Gamma" },
        };
        var view = ViewHarness.Show(new Ex010_CompiledBinding { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_The_Direct_And_The_Nested_Path()
    {
        var (view, _) = Arrange();

        Assert.Equal("Design Patterns", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("Erich Gamma", view.FindControl<TextBlock>("AuthorText")!.Text);
    }

    [AvaloniaFact]
    public void Direct_Path_Follows_A_Later_Change()
    {
        var (view, vm) = Arrange();

        vm.Title = "Refactoring";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Refactoring", view.FindControl<TextBlock>("TitleText")!.Text);
    }

    // The nested-path discriminator: a binding to Author.Name must re-resolve when the
    // intermediate Author object itself is swapped, not only when its Name changes.
    [AvaloniaFact]
    public void Nested_Path_Re_Resolves_When_The_Intermediate_Object_Is_Replaced()
    {
        var (view, vm) = Arrange();
        var author = view.FindControl<TextBlock>("AuthorText")!;

        vm.Author.Name = "Richard Helm";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Richard Helm", author.Text);

        vm.Author = new Ex010_AuthorViewModel { Name = "Ralph Johnson" };
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Ralph Johnson", author.Text);
    }
}
