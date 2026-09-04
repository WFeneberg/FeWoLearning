using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex013_MarginPaddingTests : UnoTestContext
{
    private static (Border Card, Border Content) Card()
    {
        var content = new Border { Width = 30, Height = 20 };
        return (Ex013_MarginPadding.CreateCard(content), content);
    }

    [Fact]
    public void Holds_The_Content()
    {
        var (card, content) = Card();

        Assert.Same(content, card.Child);
    }

    [Fact]
    public void Pads_In_Left_Top_Right_Bottom_Order()
    {
        var (card, _) = Card();

        Assert.Equal(12, card.Padding.Left);
        Assert.Equal(8, card.Padding.Top);
        Assert.Equal(4, card.Padding.Right);
        Assert.Equal(2, card.Padding.Bottom);
    }

    [Fact]
    public void Gives_The_Content_A_Uniform_Margin()
    {
        var (_, content) = Card();

        Assert.Equal(4, content.Margin.Left);
        Assert.Equal(4, content.Margin.Top);
        Assert.Equal(4, content.Margin.Right);
        Assert.Equal(4, content.Margin.Bottom);
    }

    [Fact]
    public void Asks_For_The_Content_Plus_Both_Kinds_Of_Space()
    {
        var (card, _) = Card();

        Layout(card);

        // 30 + (4+4) margin + (12+4) padding, and 20 + (4+4) + (8+2).
        Assert.Equal(54, card.DesiredSize.Width, 1);
        Assert.Equal(38, card.DesiredSize.Height, 1);
    }

    [Fact]
    public void Offsets_The_Content_By_Padding_And_Margin_Together()
    {
        var (card, content) = Card();

        // Arranged at exactly its desired size. Given a bigger slot the card would stretch
        // and then centre the fixed-width content inside it, which is Ex014's lesson, not
        // this one.
        Layout(card, width: 54, height: 38);

        Assert.Equal(16, Offset(content).X, 1);
        Assert.Equal(12, Offset(content).Y, 1);
    }

    [Fact]
    public void Leaves_The_Content_Its_Own_Size()
    {
        var (card, content) = Card();

        Layout(card);

        // Space around the child, never taken out of it.
        Assert.Equal(30, content.ActualWidth, 1);
        Assert.Equal(20, content.ActualHeight, 1);
    }
}
