using FeWoLearning.Uno.Exercises.Beginner;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex005_OneWayBindingTests : UnoTestContext
{
    [Fact]
    public void Shows_The_Caption_The_Source_Already_Has()
    {
        var source = new CaptionSource { Caption = "ready" };

        var label = Ex005_OneWayBinding.CreateCaptionLabel(source);

        Assert.Equal("ready", label.Text);
    }

    [Fact]
    public void Follows_The_Source_Afterwards()
    {
        var source = new CaptionSource { Caption = "ready" };
        var label = Ex005_OneWayBinding.CreateCaptionLabel(source);

        source.Caption = "running";

        // Copying Text once in the factory passes the first test and fails this one.
        Assert.Equal("running", label.Text);
    }

    [Fact]
    public void Keeps_Following_Across_Several_Changes()
    {
        var source = new CaptionSource();
        var label = Ex005_OneWayBinding.CreateCaptionLabel(source);

        source.Caption = "one";
        source.Caption = "two";
        source.Caption = "three";

        Assert.Equal("three", label.Text);
    }

    [Fact]
    public void Does_Not_Write_Back_To_The_Source()
    {
        var source = new CaptionSource { Caption = "ready" };
        var label = Ex005_OneWayBinding.CreateCaptionLabel(source);

        label.Text = "typed by the user";

        // OneWay, not TwoWay: the source is nobody's output here.
        Assert.Equal("ready", source.Caption);
    }

    [Fact]
    public void Renders_The_Bound_Text_At_A_Real_Size()
    {
        var source = new CaptionSource { Caption = "ready" };

        var label = Layout(Ex005_OneWayBinding.CreateCaptionLabel(source));

        // The value reached the element before layout, not just the property system.
        Assert.True(label.DesiredSize.Width > 0);
    }
}
