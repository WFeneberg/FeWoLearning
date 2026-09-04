using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex055_DataTemplateSelectorTests : UnoTestContext
{
    private static readonly DataTemplate Fallback = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <TextBlock x:Name="Fallback" Text="fallback" />
        </DataTemplate>
        """);

    private static Ex055_DataTemplateSelector Selector(int threshold = 5) => new() { Threshold = threshold };

    private static string ChosenName(ContentControl card) => FindDescendant<TextBlock>(card).Name;

    [Fact]
    public void A_Long_Item_Gets_The_Long_Template()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("a long caption", Selector()));

        Assert.Equal("Long", ChosenName(card));
    }

    [Fact]
    public void A_Short_Item_Gets_The_Short_Template()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("hi", Selector()));

        Assert.Equal("Short", ChosenName(card));
    }

    [Fact]
    public void The_Threshold_Is_Inclusive()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("12345", Selector(threshold: 5)));

        Assert.Equal("Long", ChosenName(card));
    }

    [Fact]
    public void The_Threshold_Is_Configurable()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("hi", Selector(threshold: 2)));

        Assert.Equal("Long", ChosenName(card));
    }

    [Fact]
    public void The_Chosen_Template_Renders_The_Item()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("hi", Selector()));

        Assert.Equal("hi", FindDescendant<TextBlock>(card).Text);
    }

    [Fact]
    public void Abstaining_Falls_Back_To_The_Content_Template()
    {
        var card = Layout(Ex055_DataTemplateSelector.CreateCard(42, Selector(), Fallback));

        // Returning null means "no opinion", not "show nothing" - the host then uses its
        // own ContentTemplate.
        Assert.Equal("Fallback", ChosenName(card));
    }

    [Fact]
    public void The_Card_Carries_The_Selector()
    {
        var selector = Selector();

        var card = Ex055_DataTemplateSelector.CreateCard("hi", selector);

        Assert.Same(selector, card.ContentTemplateSelector);
    }

    [Fact]
    public void The_Selector_Is_Asked_Per_Item()
    {
        var selector = Selector();
        var card = Layout(Ex055_DataTemplateSelector.CreateCard("a long caption", selector));

        card.Content = "hi";
        Layout(card);

        // The choice is re-made for the new item, which a property on the host could never
        // do.
        Assert.Equal("Short", ChosenName(card));
    }
}
