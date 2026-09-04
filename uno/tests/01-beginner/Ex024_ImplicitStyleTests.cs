using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex024_ImplicitStyleTests : UnoTestContext
{
    [Fact]
    public void The_Style_Targets_Borders()
    {
        Assert.Equal(typeof(Border), Ex024_ImplicitStyle.CreateBorderStyle().TargetType);
    }

    [Fact]
    public void The_Style_Carries_Both_Setters()
    {
        var style = Ex024_ImplicitStyle.CreateBorderStyle();

        Assert.Equal(2, style.Setters.Count);
    }

    [Fact]
    public void Every_Border_In_The_Scope_Picks_It_Up()
    {
        var first = new Border();
        var second = new Border();

        Layout(Ex024_ImplicitStyle.CreateStyledScope(first, second));

        // Neither Border was touched: no Style assignment, no Width, no Height.
        Assert.Equal(77, first.ActualWidth, 1);
        Assert.Equal(33, first.ActualHeight, 1);
        Assert.Equal(77, second.ActualWidth, 1);
    }

    [Fact]
    public void The_Style_Is_Registered_Under_The_Target_Type()
    {
        var panel = Ex024_ImplicitStyle.CreateStyledScope();

        // That key is what "implicit" means: the lookup from a Border asks for
        // typeof(Border), the same walk as any other resource.
        Assert.IsType<Style>(panel.Resources[typeof(Border)]);
    }

    [Fact]
    public void Elements_Of_Other_Types_Are_Untouched()
    {
        var text = new TextBlock { Text = "x" };

        Layout(Ex024_ImplicitStyle.CreateStyledScope(text));

        Assert.NotEqual(77, text.ActualWidth);
    }

    [Fact]
    public void A_Local_Value_Still_Wins()
    {
        var explicitly = new Border { Width = 10 };

        Layout(Ex024_ImplicitStyle.CreateStyledScope(explicitly));

        // Style setters sit below local values in the precedence order - which is why a
        // style can never "fix" a value somebody hard-coded on the element.
        Assert.Equal(10, explicitly.ActualWidth, 1);
        Assert.Equal(33, explicitly.ActualHeight, 1);
    }

    [Fact]
    public void A_Border_Outside_The_Scope_Gets_Nothing()
    {
        var outside = new Border();

        Ex024_ImplicitStyle.CreateStyledScope(new Border());
        Layout(outside);

        Assert.NotEqual(77, outside.ActualWidth);
    }

    [Fact]
    public void The_Children_End_Up_In_The_Panel()
    {
        var child = new Border();

        var panel = Ex024_ImplicitStyle.CreateStyledScope(child);

        Assert.Same(child, Assert.Single(panel.Children));
    }
}
