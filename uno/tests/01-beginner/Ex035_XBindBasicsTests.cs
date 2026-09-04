using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex035_XBindBasicsTests : UnoTestContext
{
    private static string TextOf(Ex035_XBindBasics control, string name) =>
        FindDescendant<TextBlock>(control, name).Text;

    [Fact]
    public void All_Three_Bindings_Resolve_On_First_Build()
    {
        var control = Layout(new Ex035_XBindBasics());

        Assert.Equal("hello", TextOf(control, "Once"));
        Assert.Equal("hello", TextOf(control, "Live"));
        Assert.Equal("HELLO", TextOf(control, "Computed"));
    }

    [Fact]
    public void The_One_Way_Binding_Follows_The_Property()
    {
        var control = Layout(new Ex035_XBindBasics());

        control.Caption = "changed";

        Assert.Equal("changed", TextOf(control, "Live"));
    }

    [Fact]
    public void The_Default_Binding_Does_Not_Follow_The_Property()
    {
        var control = Layout(new Ex035_XBindBasics());

        control.Caption = "changed";

        // x:Bind defaults to OneTime, unlike {Binding}, which defaults to OneWay. Nothing
        // reports this: the label simply stops being right.
        Assert.Equal("hello", TextOf(control, "Once"));
    }

    [Fact]
    public void A_Method_Binding_Is_Evaluated_Once_Too()
    {
        var control = Layout(new Ex035_XBindBasics());

        control.Caption = "changed";

        Assert.Equal("HELLO", TextOf(control, "Computed"));
    }

    [Fact]
    public void The_Property_Announces_Itself()
    {
        var control = new Ex035_XBindBasics();
        var names = new List<string?>();
        control.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        control.Caption = "changed";

        Assert.Contains(nameof(Ex035_XBindBasics.Caption), names);
    }

    [Fact]
    public void The_Method_Reflects_The_Current_Caption_When_Called()
    {
        var control = new Ex035_XBindBasics { Caption = "quiet" };

        // The method itself is not stale - only the binding that called it once is.
        Assert.Equal("QUIET", control.Shout());
    }

    [Fact]
    public void The_Markup_Builds_The_Named_Root()
    {
        var control = Layout(new Ex035_XBindBasics());

        var root = Assert.IsType<StackPanel>(control.FindName("Root"));

        Assert.Equal(3, root.Children.Count);
    }
}
