using FeWoLearning.Uno.Exercises.Beginner;
using FeWoLearning.Uno.Support;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex019_DataTemplateBasicsTests : UnoTestContext
{
    [Fact]
    public void The_Template_Is_One_Shared_Instance()
    {
        // A template is a factory, so rebuilding it per call would be pure waste - and
        // would also break the "same template, many controls" test below.
        Assert.Same(Ex019_DataTemplateBasics.CaptionTemplate, Ex019_DataTemplateBasics.CaptionTemplate);
    }

    [Fact]
    public void The_Card_Uses_The_Shared_Template()
    {
        var card = Ex019_DataTemplateBasics.CreateCard(new CaptionSource { Caption = "one" });

        Assert.Same(Ex019_DataTemplateBasics.CaptionTemplate, card.ContentTemplate);
    }

    [Fact]
    public void The_Card_Holds_The_Item_Untouched()
    {
        var item = new CaptionSource { Caption = "one" };

        var card = Ex019_DataTemplateBasics.CreateCard(item);

        Assert.Same(item, card.Content);
    }

    [Fact]
    public void The_Template_Builds_The_TextBlock()
    {
        var card = Layout(Ex019_DataTemplateBasics.CreateCard(new CaptionSource { Caption = "one" }));

        // Nothing in the exercise creates a TextBlock: the template did.
        Assert.Equal("one", FindDescendant<TextBlock>(card).Text);
    }

    [Fact]
    public void Each_Control_Gets_Its_Own_Copy_Of_The_Tree()
    {
        var first = Layout(Ex019_DataTemplateBasics.CreateCard(new CaptionSource { Caption = "one" }));
        var second = Layout(Ex019_DataTemplateBasics.CreateCard(new CaptionSource { Caption = "two" }));

        var firstText = FindDescendant<TextBlock>(first);
        var secondText = FindDescendant<TextBlock>(second);

        Assert.NotSame(firstText, secondText);
        Assert.Equal("one", firstText.Text);
        Assert.Equal("two", secondText.Text);
    }

    [Fact]
    public void The_Binding_Inside_The_Template_Stays_Live()
    {
        var item = new CaptionSource { Caption = "one" };
        var card = Layout(Ex019_DataTemplateBasics.CreateCard(item));

        item.Caption = "changed";

        Assert.Equal("changed", FindDescendant<TextBlock>(card).Text);
    }

    [Fact]
    public void Swapping_The_Content_Re_Targets_The_Same_Template()
    {
        var card = Layout(Ex019_DataTemplateBasics.CreateCard(new CaptionSource { Caption = "one" }));

        card.Content = new CaptionSource { Caption = "second item" };
        Layout(card);

        Assert.Equal("second item", FindDescendant<TextBlock>(card).Text);
    }
}
