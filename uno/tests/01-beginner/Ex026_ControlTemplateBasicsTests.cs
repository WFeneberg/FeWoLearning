using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex026_ControlTemplateBasicsTests : UnoTestContext
{
    [Fact]
    public void The_Template_Is_One_Shared_Instance()
    {
        Assert.Same(Ex026_ControlTemplateBasics.CardTemplate, Ex026_ControlTemplateBasics.CardTemplate);
    }

    [Fact]
    public void The_Card_Uses_The_Template()
    {
        var card = Ex026_ControlTemplateBasics.CreateCard("hello");

        Assert.Same(Ex026_ControlTemplateBasics.CardTemplate, card.Template);
    }

    [Fact]
    public void The_Template_Supplies_The_Named_Root()
    {
        var card = Layout(Ex026_ControlTemplateBasics.CreateCard("hello"));

        Assert.NotNull(FindDescendant<Border>(card, "PART_Root"));
    }

    [Fact]
    public void The_Content_Reaches_The_Presenter()
    {
        var card = Layout(Ex026_ControlTemplateBasics.CreateCard("hello"));

        Assert.Equal("hello", FindDescendant<TextBlock>(card).Text);
    }

    [Fact]
    public void Template_Binding_Pulls_The_Background_From_The_Control()
    {
        var card = Ex026_ControlTemplateBasics.CreateCard("hello");
        card.Background = new SolidColorBrush(Colors.Red);

        Layout(card);

        var root = FindDescendant<Border>(card, "PART_Root");
        Assert.Equal(Colors.Red, ((SolidColorBrush)root.Background).Color);
    }

    [Fact]
    public void Template_Binding_Keeps_Following_The_Control()
    {
        var card = Ex026_ControlTemplateBasics.CreateCard("hello");
        card.Background = new SolidColorBrush(Colors.Red);
        Layout(card);
        var root = FindDescendant<Border>(card, "PART_Root");

        card.Background = new SolidColorBrush(Colors.Blue);
        Layout(card);

        // TemplateBinding is a binding, not a copy taken while the template was expanded.
        Assert.Equal(Colors.Blue, ((SolidColorBrush)root.Background).Color);
    }

    [Fact]
    public void Padding_Set_On_The_Control_Is_Applied_By_The_Template()
    {
        var card = Ex026_ControlTemplateBasics.CreateCard(new Border { Width = 30, Height = 20 });
        card.Padding = new Thickness(5);

        Layout(card);

        // The control has no idea what a padding looks like - the template decided that
        // it belongs on the Border around the presenter.
        Assert.Equal(40, card.DesiredSize.Width, 1);
        Assert.Equal(30, card.DesiredSize.Height, 1);
    }

    [Fact]
    public void Each_Card_Expands_Its_Own_Copy_Of_The_Template()
    {
        var first = Layout(Ex026_ControlTemplateBasics.CreateCard("one"));
        var second = Layout(Ex026_ControlTemplateBasics.CreateCard("two"));

        Assert.NotSame(
            FindDescendant<Border>(first, "PART_Root"),
            FindDescendant<Border>(second, "PART_Root"));
    }
}
