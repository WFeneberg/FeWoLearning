using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex060_TemplatePartLookupTests
{
    private static Ex060_TemplatePartLookup Arrange() =>
        ViewHarness.Show(new Ex060_TemplatePartLookup(), 200, 80);

    // Named lookups rather than OfType<TextBlock>().Single(): the Button's own
    // "+" content renders through an auto-generated TextBlock of its own, so a
    // plain type-based query would see two and prove nothing.
    private static Button? RealButton(Ex060_TemplatePartLookup control) =>
        control.GetVisualDescendants().OfType<Button>().SingleOrDefault(b => b.Name == "PART_Increment");

    private static TextBlock? RealDisplay(Ex060_TemplatePartLookup control) =>
        control.GetVisualDescendants().OfType<TextBlock>().SingleOrDefault(t => t.Name == "PART_Display");

    // Mechanism check: IncrementPart/DisplayPart can only be populated from
    // inside OnApplyTemplate, since e.NameScope does not exist anywhere else. A
    // handler wired in the constructor that catches the Button's Click event
    // bubbling up (with no part lookup at all) can still make Count and the
    // rendered text behave correctly below, without ever touching these two
    // properties - which is exactly why this check exists independently of the
    // behavioural one.
    [AvaloniaFact]
    public void IncrementPart_And_DisplayPart_Are_The_Real_Template_Children()
    {
        var control = Arrange();
        Dispatcher.UIThread.RunJobs();

        Assert.NotNull(control.IncrementPart);
        Assert.NotNull(control.DisplayPart);
        Assert.Same(RealButton(control), control.IncrementPart);
        Assert.Same(RealDisplay(control), control.DisplayPart);
    }

    [AvaloniaFact]
    public void Display_Starts_At_Zero()
    {
        var control = Arrange();
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(0, control.Count);
        Assert.Equal("0", control.DisplayPart!.Text);
    }

    [AvaloniaFact]
    public void Clicking_Increment_Twice_Updates_Count_And_The_Display()
    {
        var control = Arrange();
        Dispatcher.UIThread.RunJobs();

        control.IncrementPart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        control.IncrementPart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(2, control.Count);
        Assert.Equal("2", control.DisplayPart!.Text);
    }
}
