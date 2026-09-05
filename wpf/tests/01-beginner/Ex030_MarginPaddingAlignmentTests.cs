using System.Windows;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex030_MarginPaddingAlignmentTests : WpfTestContext
{
    [WpfFact]
    public void BuildBorder_Carries_All_Four_Values_As_Given()
    {
        var margin = new Thickness(5, 6, 7, 8);
        var padding = new Thickness(3);

        var border = Ex030_MarginPaddingAlignment.BuildBorder(margin, padding, HorizontalAlignment.Left, VerticalAlignment.Top);

        Assert.Equal(margin, border.Margin);
        Assert.Equal(padding, border.Padding);
        Assert.Equal(HorizontalAlignment.Left, border.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Top, border.VerticalAlignment);
    }

    [WpfFact]
    public void Stretch_Alignment_Fills_The_Available_Space_Minus_Margin()
    {
        var border = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(5), new Thickness(3), HorizontalAlignment.Stretch, VerticalAlignment.Stretch);

        Layout(border, new Size(300, 200));

        Assert.Equal(new Size(290, 190), border.RenderSize); // 300/200 minus 5+5 margin each way
    }

    [WpfFact]
    public void NonStretch_Alignment_Shrinks_To_The_Borders_Own_Natural_Size()
    {
        // Different available size, margin and padding than the Stretch test above - a
        // hard-coded RenderSize cannot satisfy both.
        var border = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(10), new Thickness(4), HorizontalAlignment.Left, VerticalAlignment.Top);

        Layout(border, new Size(400, 250));

        // Natural size: the 40x20 child plus Padding (4 each side). Margin positions the box
        // but plays no part in RenderSize itself.
        Assert.Equal(new Size(48, 28), border.RenderSize);
    }

    [WpfFact]
    public void Left_Top_And_Right_Bottom_Alignment_Place_The_Box_At_Opposite_Corners()
    {
        var leftTop = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(5), new Thickness(3), HorizontalAlignment.Left, VerticalAlignment.Top);
        Layout(leftTop, new Size(300, 200));

        var rightBottom = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(5), new Thickness(3), HorizontalAlignment.Right, VerticalAlignment.Bottom);
        Layout(rightBottom, new Size(300, 200));

        // Same margin, same padding, same available size, opposite alignment - only the real
        // mechanism moves the box from one corner of the available space to the other.
        Assert.Equal(new Vector(5, 5), VisualTreeHelper.GetOffset(leftTop));
        Assert.Equal(new Vector(249, 169), VisualTreeHelper.GetOffset(rightBottom));
    }

    [WpfFact]
    public void Padding_Grows_The_Borders_Own_Size_Beyond_Its_Childs_Fixed_Size()
    {
        var noPadding = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(0), new Thickness(0), HorizontalAlignment.Left, VerticalAlignment.Top);
        Layout(noPadding, new Size(300, 200));

        var withPadding = Ex030_MarginPaddingAlignment.BuildBorder(
            new Thickness(0), new Thickness(12), HorizontalAlignment.Left, VerticalAlignment.Top);
        Layout(withPadding, new Size(300, 200));

        // Same child (40x20), same alignment, same available size - only Padding explains
        // the 24-pixel-per-axis difference (12 on each side).
        Assert.Equal(new Size(40, 20), noPadding.RenderSize);
        Assert.Equal(new Size(64, 44), withPadding.RenderSize);
    }
}
