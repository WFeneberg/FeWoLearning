using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex032_ValuePrecedenceTests : UnoTestContext
{
    private static Style WidthStyle(double width) => new(typeof(Border))
    {
        Setters = { new Setter(FrameworkElement.WidthProperty, width) },
    };

    private static string Source(FrameworkElement element) =>
        Ex032_ValuePrecedence.DescribeSource(element, FrameworkElement.WidthProperty);

    [Fact]
    public void An_Untouched_Element_Reports_The_Default()
    {
        Assert.Equal("default", Source(new Border()));
    }

    [Fact]
    public void A_Styled_Element_Reports_The_Style()
    {
        Assert.Equal("style", Source(new Border { Style = WidthStyle(50) }));
    }

    [Fact]
    public void A_Locally_Set_Element_Reports_Local()
    {
        Assert.Equal("local", Source(new Border { Width = 70 }));
    }

    [Fact]
    public void A_Local_Value_Outranks_The_Style()
    {
        var border = new Border { Style = WidthStyle(50), Width = 70 };

        Assert.Equal("local", Source(border));

        Layout(border);
        Assert.Equal(70, border.ActualWidth, 1);
    }

    [Fact]
    public void A_Style_That_Does_Not_Set_The_Property_Is_Not_The_Source()
    {
        var styleWithoutWidth = new Style(typeof(Border))
        {
            Setters = { new Setter(FrameworkElement.HeightProperty, 10d) },
        };

        Assert.Equal("default", Source(new Border { Style = styleWithoutWidth }));
    }

    [Fact]
    public void An_Inherited_Setter_Still_Counts_As_The_Style()
    {
        var derived = new Style(typeof(Border))
        {
            BasedOn = WidthStyle(50),
            Setters = { new Setter(FrameworkElement.HeightProperty, 10d) },
        };

        // The setter lives on the base style, and the element only knows the derived one.
        Assert.Equal("style", Source(new Border { Style = derived }));
    }

    [Fact]
    public void The_Effective_Value_Alone_Cannot_Tell_You_The_Source()
    {
        var styled = new Border { Style = WidthStyle(50) };
        var local = new Border { Width = 50 };

        Layout(styled);
        Layout(local);

        // Same number, different provenance. This is why "the style is not applying" needs
        // ReadLocalValue and not GetValue to diagnose.
        Assert.Equal(styled.ActualWidth, local.ActualWidth, 1);
        Assert.NotEqual(Source(styled), Source(local));
    }

    [Fact]
    public void Works_For_Other_Properties_Too()
    {
        var border = new Border { Height = 12 };

        Assert.Equal("local", Ex032_ValuePrecedence.DescribeSource(border, FrameworkElement.HeightProperty));
        Assert.Equal("default", Ex032_ValuePrecedence.DescribeSource(border, FrameworkElement.WidthProperty));
    }
}
