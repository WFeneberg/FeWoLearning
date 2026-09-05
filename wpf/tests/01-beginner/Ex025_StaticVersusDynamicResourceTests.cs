using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex025_StaticVersusDynamicResourceTests : WpfTestContext
{
    [WpfFact]
    public void ApplyOnce_Writes_The_Resolved_Value_As_A_Plain_Literal()
    {
        var root = new StackPanel();
        root.Resources["Greeting"] = "Hello";
        var target = new Button();
        root.Children.Add(target);
        Layout(root);

        Ex025_StaticVersusDynamicResource.ApplyOnce(target, FrameworkElement.TagProperty, "Greeting");

        Assert.Equal("Hello", target.Tag);
        // Mechanism: a plain SetValue result is never an expression - this is what tells
        // ApplyOnce apart from a learner who called SetResourceReference here instead.
        Assert.False(DependencyPropertyHelper.GetValueSource(target, FrameworkElement.TagProperty).IsExpression);
    }

    [WpfFact]
    public void ApplyFollowing_Writes_The_Value_As_A_Resource_Reference_Expression()
    {
        var root = new StackPanel();
        root.Resources["Greeting"] = "Hello";
        var target = new Button();
        root.Children.Add(target);
        Layout(root);

        Ex025_StaticVersusDynamicResource.ApplyFollowing(target, FrameworkElement.TagProperty, "Greeting");
        Pump();

        Assert.Equal("Hello", target.Tag);
        // Mechanism: only a genuine resource reference reports IsExpression - a learner
        // who instead called FindResource+SetValue here would pass every assertion above
        // but fail this one.
        Assert.True(DependencyPropertyHelper.GetValueSource(target, FrameworkElement.TagProperty).IsExpression);
    }

    [WpfFact]
    public void Only_ApplyFollowing_Picks_Up_A_Later_Resource_Swap()
    {
        var root = new StackPanel();
        root.Resources["Greeting"] = "Hello";
        var once = new Button();
        var following = new Button();
        root.Children.Add(once);
        root.Children.Add(following);
        Layout(root);

        Ex025_StaticVersusDynamicResource.ApplyOnce(once, FrameworkElement.TagProperty, "Greeting");
        Ex025_StaticVersusDynamicResource.ApplyFollowing(following, FrameworkElement.TagProperty, "Greeting");
        Pump();

        Assert.Equal("Hello", once.Tag);
        Assert.Equal("Hello", following.Tag);

        root.Resources["Greeting"] = "Ciao";
        Pump();

        // Same swap, same dictionary, same key - only the DynamicResource-equivalent
        // reference actually follows it.
        Assert.Equal("Hello", once.Tag);
        Assert.Equal("Ciao", following.Tag);
    }
}
