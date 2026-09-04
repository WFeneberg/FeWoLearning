using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex031_StaticAndDynamicResourceTests
{
    private static Ex031_StaticAndDynamicResource Show() =>
        ViewHarness.Show(new Ex031_StaticAndDynamicResource(), 300, 160);

    [AvaloniaFact]
    public void Both_Consumers_Start_At_The_Resources_Initial_Value()
    {
        var view = Show();
        var staticText = view.FindControl<TextBlock>("StaticText")!;
        var dynamicText = view.FindControl<TextBlock>("DynamicText")!;

        Assert.Equal(17, staticText.FontSize);
        Assert.Equal(17, dynamicText.FontSize);
    }

    // The whole exercise: a runtime resource swap reaches ONLY the
    // DynamicResource consumer. A hard-coded FontSize="17" literal on both
    // elements (no resource lookup at all) matches the test above but stays
    // frozen at 17 for both here - and swapping which markup extension is on
    // which named element fails this too, because each assertion is pinned
    // to a specific Name, not merely "one of the two textblocks".
    [AvaloniaFact]
    public void Only_The_DynamicResource_Consumer_Follows_A_Runtime_Resource_Swap()
    {
        var view = Show();
        var staticText = view.FindControl<TextBlock>("StaticText")!;
        var dynamicText = view.FindControl<TextBlock>("DynamicText")!;

        view.Resources["Size"] = 29d;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(17, staticText.FontSize);
        Assert.Equal(29, dynamicText.FontSize);
    }
}
