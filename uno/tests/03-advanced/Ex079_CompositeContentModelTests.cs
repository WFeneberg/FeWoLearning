using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex079_CompositeContentModelTests : UnoTestContext
{
    private static Ex079_CompositeContentModel Card(object? header = null, object? body = null, object? footer = null) =>
        Layout(new Ex079_CompositeContentModel
        {
            Template = Ex079_CompositeContentModel.CardTemplate,
            Header = header,
            Body = body,
            Footer = footer,
        });

    private static Visibility HostVisibility(Ex079_CompositeContentModel card, string part) =>
        FindDescendant<Border>(card, part).Visibility;

    [Fact]
    public void A_Filled_Slot_Is_Shown()
    {
        var card = Card(header: "Title", body: "Text");

        Assert.Equal(Visibility.Visible, HostVisibility(card, "PART_HeaderHost"));
        Assert.Equal(Visibility.Visible, HostVisibility(card, "PART_BodyHost"));
    }

    [Fact]
    public void An_Empty_Slot_Is_Collapsed()
    {
        var card = Card(header: "Title", body: "Text");

        // Not merely invisible: a collapsed host leaves the layout, so its padding goes
        // with it. An Opacity of 0 would leave a gap nobody can explain from the markup.
        Assert.Equal(Visibility.Collapsed, HostVisibility(card, "PART_FooterHost"));
    }

    [Fact]
    public void Filling_A_Slot_Later_Shows_It()
    {
        var card = Card(body: "Text");

        card.Footer = "Signed";

        Assert.Equal(Visibility.Visible, HostVisibility(card, "PART_FooterHost"));
    }

    [Fact]
    public void Clearing_A_Slot_Collapses_It_Again()
    {
        var card = Card(header: "Title", body: "Text");

        card.Header = null;

        Assert.Equal(Visibility.Collapsed, HostVisibility(card, "PART_HeaderHost"));
    }

    [Fact]
    public void An_Empty_String_Is_Content()
    {
        var card = Card(header: "");

        // Empty is not absent. A control that treats "" as "no header" cannot show a blank
        // header row on purpose.
        Assert.Equal(Visibility.Visible, HostVisibility(card, "PART_HeaderHost"));
    }

    [Fact]
    public void A_Zero_Is_Content()
    {
        var card = Card(body: 0);

        Assert.Equal(Visibility.Visible, HostVisibility(card, "PART_BodyHost"));
    }

    [Fact]
    public void A_Slot_Takes_An_Element_As_Readily_As_A_String()
    {
        var element = new Border { Width = 30, Height = 10 };

        var card = Card(body: element);

        Assert.Same(element, FindDescendant<Border>(card, "PART_BodyHost").Child is ContentPresenter presenter
            ? presenter.Content
            : null);
    }

    [Fact]
    public void The_Slots_Render_Their_Content()
    {
        var card = Card(header: "Title", body: "Text", footer: "Signed");

        var texts = Descendants(card).OfType<TextBlock>().Select(block => block.Text).ToList();

        Assert.Contains("Title", texts);
        Assert.Contains("Text", texts);
        Assert.Contains("Signed", texts);
    }

    [Fact]
    public void An_Empty_Card_Collapses_Everything()
    {
        var card = Card();

        Assert.Equal(Visibility.Collapsed, HostVisibility(card, "PART_HeaderHost"));
        Assert.Equal(Visibility.Collapsed, HostVisibility(card, "PART_BodyHost"));
        Assert.Equal(Visibility.Collapsed, HostVisibility(card, "PART_FooterHost"));
        Assert.Equal(0, card.DesiredSize.Height, 1);
    }
}
