using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex025_StyleInheritanceTests : UnoTestContext
{
    [Fact]
    public void The_Base_Style_Applies_Its_Own_Setters()
    {
        var border = Layout(Ex025_StyleInheritance.CreateStyled(Ex025_StyleInheritance.CreateBaseStyle()));

        Assert.Equal(100, border.ActualWidth, 1);
        Assert.Equal(40, border.ActualHeight, 1);
    }

    [Fact]
    public void The_Derived_Style_Points_At_The_Base()
    {
        var baseStyle = Ex025_StyleInheritance.CreateBaseStyle();

        var wide = Ex025_StyleInheritance.CreateWideStyle(baseStyle);

        Assert.Same(baseStyle, wide.BasedOn);
    }

    [Fact]
    public void The_Derived_Style_Inherits_What_It_Does_Not_Override()
    {
        var wide = Ex025_StyleInheritance.CreateWideStyle(Ex025_StyleInheritance.CreateBaseStyle());

        var border = Layout(Ex025_StyleInheritance.CreateStyled(wide));

        Assert.Equal(100, border.ActualWidth, 1);
    }

    [Fact]
    public void The_Derived_Style_Overrides_What_It_Declares()
    {
        var wide = Ex025_StyleInheritance.CreateWideStyle(Ex025_StyleInheritance.CreateBaseStyle());

        var border = Layout(Ex025_StyleInheritance.CreateStyled(wide));

        Assert.Equal(80, border.ActualHeight, 1);
    }

    [Fact]
    public void The_Derived_Style_Does_Not_Repeat_The_Inherited_Setter()
    {
        var wide = Ex025_StyleInheritance.CreateWideStyle(Ex025_StyleInheritance.CreateBaseStyle());

        // Copying the Width setter across would work today and drift tomorrow: the point
        // of BasedOn is that the base stays the single place Width is decided.
        Assert.Single(wide.Setters);
    }

    [Fact]
    public void The_Base_Style_Is_Unaffected_By_The_Derived_One()
    {
        var baseStyle = Ex025_StyleInheritance.CreateBaseStyle();
        Ex025_StyleInheritance.CreateWideStyle(baseStyle);

        var border = Layout(Ex025_StyleInheritance.CreateStyled(baseStyle));

        Assert.Equal(40, border.ActualHeight, 1);
    }

    [Fact]
    public void One_Base_Can_Serve_Several_Derived_Styles()
    {
        var baseStyle = Ex025_StyleInheritance.CreateBaseStyle();

        var first = Layout(Ex025_StyleInheritance.CreateStyled(Ex025_StyleInheritance.CreateWideStyle(baseStyle)));
        var second = Layout(Ex025_StyleInheritance.CreateStyled(Ex025_StyleInheritance.CreateWideStyle(baseStyle)));

        Assert.Equal(80, first.ActualHeight, 1);
        Assert.Equal(80, second.ActualHeight, 1);
    }

    [Fact]
    public void The_Styled_Element_Actually_Carries_The_Style()
    {
        var wide = Ex025_StyleInheritance.CreateWideStyle(Ex025_StyleInheritance.CreateBaseStyle());

        var border = Ex025_StyleInheritance.CreateStyled(wide);

        Assert.Same(wide, border.Style);
        Assert.IsType<Border>(border);
    }
}
