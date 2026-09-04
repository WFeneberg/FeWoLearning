using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex082_StoryboardLogicTests : UnoTestContext
{
    private static (Border Target, Storyboard Storyboard) Animation(double from = 10, double to = 50)
    {
        var target = Layout(new Border { Width = from, Height = 10 });
        return (target, Ex082_StoryboardLogic.CreateWidthAnimation(target, from, to));
    }

    [Fact]
    public void The_Storyboard_Holds_One_Animation()
    {
        var (_, storyboard) = Animation();

        Assert.Single(storyboard.Children);
        Assert.IsType<DoubleAnimation>(storyboard.Children[0]);
    }

    [Fact]
    public void The_Animation_Can_Move_A_Layout_Property()
    {
        var (_, storyboard) = Animation();

        var animation = (DoubleAnimation)storyboard.Children[0];

        // Width is a dependent animation. Without this flag it runs, completes, and moves
        // nothing at all - no exception, no warning.
        Assert.True(animation.EnableDependentAnimation);
    }

    [Fact]
    public void The_Animation_Names_Its_Target_Property()
    {
        var (_, storyboard) = Animation();

        // Storyboard.SetTarget has no getter in WinUI - the target is write-only attached
        // state - so only the property path can be read back. That the target is right is
        // proved further down, by the element actually moving.
        Assert.Equal("Width", Storyboard.GetTargetProperty(storyboard.Children[0]));
    }

    [Fact]
    public void The_Animation_Carries_Its_Endpoints()
    {
        var (_, storyboard) = Animation(from: 10, to: 50);

        var animation = (DoubleAnimation)storyboard.Children[0];

        Assert.Equal(10, animation.From);
        Assert.Equal(50, animation.To);
    }

    [Fact]
    public void The_Target_Does_Not_Move_Before_The_Storyboard_Starts()
    {
        var (target, _) = Animation();

        Assert.Equal(10, target.Width, 1);
    }

    [Fact]
    public void Running_To_The_End_Applies_The_Final_Value()
    {
        var (target, storyboard) = Animation();

        Assert.True(Ex082_StoryboardLogic.RunToEnd(storyboard));
        Assert.Equal(50, target.Width, 1);
    }

    [Fact]
    public void Running_To_The_End_Leaves_The_Storyboard_Finished()
    {
        var (_, storyboard) = Animation();

        Ex082_StoryboardLogic.RunToEnd(storyboard);

        Assert.NotEqual(ClockState.Active, storyboard.GetCurrentState());
    }

    [Fact]
    public void Completed_Is_Raised()
    {
        var (_, storyboard) = Animation();
        var completions = 0;
        storyboard.Completed += (_, _) => completions++;

        Ex082_StoryboardLogic.RunToEnd(storyboard);

        Assert.Equal(1, completions);
    }

    [Fact]
    public void A_Target_Property_Typo_Would_Move_Nothing()
    {
        var target = Layout(new Border { Width = 10, Height = 10 });
        var animation = new DoubleAnimation
        {
            From = 10,
            To = 50,
            Duration = new Duration(TimeSpan.FromSeconds(1)),
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, "Widht");
        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);

        Ex082_StoryboardLogic.RunToEnd(storyboard);

        // The property path is a string, so this is a silent no-op rather than a compile
        // error. Worth seeing once on purpose.
        Assert.Equal(10, target.Width, 1);
    }

    [Fact]
    public void Two_Storyboards_Over_One_Element_Both_Reach_Their_End()
    {
        var target = Layout(new Border { Width = 10, Height = 10 });

        Ex082_StoryboardLogic.RunToEnd(Ex082_StoryboardLogic.CreateWidthAnimation(target, 10, 30));
        Ex082_StoryboardLogic.RunToEnd(Ex082_StoryboardLogic.CreateWidthAnimation(target, 30, 70));

        Assert.Equal(70, target.Width, 1);
    }
}
