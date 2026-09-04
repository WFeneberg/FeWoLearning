using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex087_ControlLibraryStylesTests : UnoTestContext
{
    [Fact]
    public void The_Control_Claims_Its_Own_Default_Style()
    {
        var control = new Ex087_LibraryControl();

        Assert.Equal(typeof(Ex087_LibraryControl), control.DeclaredStyleKey);
    }

    [Fact]
    public void Without_The_Dictionary_There_Is_No_Look()
    {
        var control = Layout(new Ex087_LibraryControl());

        // Which is the honest state of an Uno control library whose dictionary the app
        // never merged - and the reason libraries document that one line of App.xaml.
        Assert.Null(control.Template);
        Assert.Equal(0, control.DesiredSize.Width, 1);
    }

    [Fact]
    public void A_Consumer_Scope_Supplies_The_Look()
    {
        var control = new Ex087_LibraryControl();

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(control), width: 200, height: 200);

        Assert.NotNull(control.Template);
    }

    [Fact]
    public void The_Style_Is_Implicit()
    {
        var control = new Ex087_LibraryControl();

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(control), width: 200, height: 200);

        // Nothing set Style on the control: a consumer writes the element and nothing else.
        Assert.Null(control.ReadLocalValue(FrameworkElement.StyleProperty) as Style);
        Assert.NotNull(control.Template);
    }

    [Fact]
    public void The_Template_Supplies_The_Part()
    {
        var control = new Ex087_LibraryControl();

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(control), width: 200, height: 200);

        var part = FindDescendant<Border>(control, "PART_Root");

        Assert.Equal(42, part.ActualWidth, 1);
        Assert.Equal(17, part.ActualHeight, 1);
    }

    [Fact]
    public void The_Template_Binds_Back_To_The_Control()
    {
        var control = new Ex087_LibraryControl { Background = new SolidColorBrush(Colors.Red) };

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(control), width: 200, height: 200);

        var part = FindDescendant<Border>(control, "PART_Root");

        Assert.Equal(Colors.Red, ((SolidColorBrush)part.Background).Color);
    }

    [Fact]
    public void Merging_Into_A_Scope_Is_All_A_Consumer_Does()
    {
        var scope = new StackPanel();
        var control = new Ex087_LibraryControl();
        scope.Children.Add(control);

        Ex087_ControlLibraryStyles.MergeInto(scope);
        Layout(scope, width: 200, height: 200);

        Assert.NotNull(control.Template);
    }

    [Fact]
    public void Several_Controls_In_One_Scope_All_Get_The_Look()
    {
        var first = new Ex087_LibraryControl();
        var second = new Ex087_LibraryControl();

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(first, second), width: 200, height: 200);

        Assert.NotNull(first.Template);
        Assert.NotNull(second.Template);
    }

    [Fact]
    public void A_Control_Outside_The_Scope_Stays_Unstyled()
    {
        var inside = new Ex087_LibraryControl();
        var outside = new Ex087_LibraryControl();

        Layout(Ex087_ControlLibraryStyles.CreateConsumerScope(inside), width: 200, height: 200);
        Layout(outside);

        Assert.NotNull(inside.Template);
        Assert.Null(outside.Template);
    }
}
