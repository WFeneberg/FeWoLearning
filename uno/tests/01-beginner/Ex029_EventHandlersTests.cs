using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex029_EventHandlersTests : UnoTestContext
{
    private static (StackPanel Panel, Button Increment, TextBlock Count) Counter()
    {
        var panel = Layout(Ex029_EventHandlers.CreateCounter());
        return (panel, FindDescendant<Button>(panel, "Increment"), FindDescendant<TextBlock>(panel, "Count"));
    }

    /// <summary>
    /// Presses the button the way a screen reader would. There is no pointer to synthesise
    /// in a windowless tree, and a control worth shipping is reachable this way anyway.
    /// </summary>
    private static void Press(Button button) => new ButtonAutomationPeer(button).Invoke();

    [Fact]
    public void Starts_At_Zero()
    {
        var (_, _, count) = Counter();

        Assert.Equal("0", count.Text);
    }

    [Fact]
    public void Labels_The_Button()
    {
        var (_, increment, _) = Counter();

        Assert.Equal("+", increment.Content);
    }

    [Fact]
    public void One_Press_Counts_One()
    {
        var (_, increment, count) = Counter();

        Press(increment);

        Assert.Equal("1", count.Text);
    }

    [Fact]
    public void Presses_Accumulate()
    {
        var (_, increment, count) = Counter();

        Press(increment);
        Press(increment);
        Press(increment);

        Assert.Equal("3", count.Text);
    }

    [Fact]
    public void Two_Counters_Count_Separately()
    {
        var (_, firstButton, firstCount) = Counter();
        var (_, _, secondCount) = Counter();

        Press(firstButton);
        Press(firstButton);

        // A static field would make this read "2" as well - and every counter in the app
        // would share one number.
        Assert.Equal("2", firstCount.Text);
        Assert.Equal("0", secondCount.Text);
    }

    [Fact]
    public void The_Panel_Holds_Both_Elements()
    {
        var (panel, increment, count) = Counter();

        Assert.Equal(2, panel.Children.Count);
        Assert.Contains(increment, panel.Children);
        Assert.Contains(count, panel.Children);
    }
}
