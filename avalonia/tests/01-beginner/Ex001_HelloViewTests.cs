using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex001_HelloViewTests
{
    private static (Ex001_HelloView View, Ex001_HelloViewModel Vm) Arrange()
    {
        var vm = new Ex001_HelloViewModel { Title = "Avalonia", Subtitle = "desktop UI" };
        var view = ViewHarness.Show(new Ex001_HelloView { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_Both_ViewModel_Properties_Into_Named_TextBlocks()
    {
        var (view, _) = Arrange();

        Assert.Equal("Avalonia", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("desktop UI", view.FindControl<TextBlock>("SubtitleText")!.Text);
    }

    [AvaloniaFact]
    public void Both_TextBlocks_Are_Laid_Out_And_Stacked_Vertically()
    {
        var (view, _) = Arrange();

        var title = view.FindControl<TextBlock>("TitleText")!;
        var subtitle = view.FindControl<TextBlock>("SubtitleText")!;

        Assert.True(title.Bounds.Height > 0, "TitleText was never laid out");
        Assert.True(subtitle.Bounds.Y >= title.Bounds.Bottom,
            "SubtitleText must sit below TitleText, so the panel must stack vertically");
    }

    // The anti-literal check: a view that hard-codes the strings passes the first
    // test but not this one, because only a real binding re-renders.
    [AvaloniaFact]
    public void Text_Follows_Later_ViewModel_Changes()
    {
        var (view, vm) = Arrange();

        vm.Title = "Changed";
        vm.Subtitle = "also changed";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Changed", view.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("also changed", view.FindControl<TextBlock>("SubtitleText")!.Text);
    }
}
