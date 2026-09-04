using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex042_AttachedBehaviorTests : UnoTestContext
{
    private static void Press(Button button) => new ButtonAutomationPeer(button).Invoke();

    [Fact]
    public void Switching_It_On_Starts_Counting()
    {
        var button = new Button();

        // Nothing counts a click nobody asked to have counted.
        Press(button);
        Assert.Equal(0, Ex042_AttachedBehavior.GetClickCount(button));

        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Press(button);
        Press(button);

        Assert.Equal(2, Ex042_AttachedBehavior.GetClickCount(button));
    }

    [Fact]
    public void Switching_It_Off_Stops_Counting()
    {
        var button = new Button();
        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Press(button);

        Ex042_AttachedBehavior.SetIsEnabled(button, false);
        Press(button);

        // The false branch is the one that gets forgotten, and forgetting it means the
        // handler - and everything it captured - outlives the behaviour.
        Assert.Equal(1, Ex042_AttachedBehavior.GetClickCount(button));
    }

    [Fact]
    public void Switching_It_On_Again_Resumes_Counting()
    {
        var button = new Button();
        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Ex042_AttachedBehavior.SetIsEnabled(button, false);

        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Press(button);

        Assert.Equal(1, Ex042_AttachedBehavior.GetClickCount(button));
    }

    [Fact]
    public void A_Press_Is_Counted_Once()
    {
        var button = new Button();

        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Ex042_AttachedBehavior.SetIsEnabled(button, false);
        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Press(button);

        // Two "+=" against one "-=" that removed nothing shows up here as 2. It happens
        // when the handler is written as a lambda in each direction: two delegates, and
        // only one of them is ever attached.
        Assert.Equal(1, Ex042_AttachedBehavior.GetClickCount(button));
    }

    [Fact]
    public void Each_Button_Counts_Its_Own_Clicks()
    {
        var first = new Button();
        var second = new Button();
        Ex042_AttachedBehavior.SetIsEnabled(first, true);
        Ex042_AttachedBehavior.SetIsEnabled(second, true);

        Press(first);
        Press(first);
        Press(second);

        // The count lives on the element, so the behaviour needs no dictionary and no
        // lifetime management of its own.
        Assert.Equal(2, Ex042_AttachedBehavior.GetClickCount(first));
        Assert.Equal(1, Ex042_AttachedBehavior.GetClickCount(second));
    }

    [Fact]
    public void Attaching_It_To_Something_That_Is_Not_A_Button_Is_Ignored()
    {
        var border = new Border();

        Ex042_AttachedBehavior.SetIsEnabled(border, true);

        // Markup can attach this to any element, and a cast would take the app down at
        // parse time for a typo in a style.
        Assert.True(Ex042_AttachedBehavior.GetIsEnabled(border));
        Assert.Equal(0, Ex042_AttachedBehavior.GetClickCount(border));
    }

    [Fact]
    public void The_Button_Is_Not_Modified_By_The_Behaviour()
    {
        var button = new Button { Content = "go" };

        Ex042_AttachedBehavior.SetIsEnabled(button, true);
        Press(button);

        // Nothing was subclassed and no property of the button itself was touched - that is
        // the point of a behaviour over a derived control.
        Assert.Equal("go", button.Content);
        Assert.True(button.IsEnabled);
    }
}
