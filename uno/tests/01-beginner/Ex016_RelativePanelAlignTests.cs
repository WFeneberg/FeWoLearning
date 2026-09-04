using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex016_RelativePanelAlignTests : UnoTestContext
{
    private static (RelativePanel Panel, Border Icon, Border Title, Border Subtitle) Badge()
    {
        var icon = new Border { Width = 30, Height = 30 };
        var title = new Border { Width = 50, Height = 20 };
        var subtitle = new Border { Width = 40, Height = 16 };
        return (Ex016_RelativePanelAlign.CreateBadge(icon, title, subtitle), icon, title, subtitle);
    }

    [Fact]
    public void Holds_All_Three_Children_In_Order()
    {
        var (panel, icon, title, subtitle) = Badge();

        Assert.Equal(3, panel.Children.Count);
        Assert.Same(icon, panel.Children[0]);
        Assert.Same(title, panel.Children[1]);
        Assert.Same(subtitle, panel.Children[2]);
    }

    [Fact]
    public void An_Unconstrained_Child_Anchors_At_The_Top_Left()
    {
        var (panel, icon, _, _) = Badge();

        Layout(panel, width: 300, height: 200);

        Assert.Equal(0, Offset(icon).X, 1);
        Assert.Equal(0, Offset(icon).Y, 1);
    }

    [Fact]
    public void Right_Of_Puts_The_Title_After_The_Icon()
    {
        var (panel, _, title, _) = Badge();

        Layout(panel, width: 300, height: 200);

        Assert.Equal(30, Offset(title).X, 1);
        Assert.Equal(0, Offset(title).Y, 1);
    }

    [Fact]
    public void Below_And_Align_Left_Stack_The_Subtitle_Under_The_Title()
    {
        var (panel, _, _, subtitle) = Badge();

        Layout(panel, width: 300, height: 200);

        // Left-aligned with the title, so it inherits the icon's width as an indent
        // without anybody writing 30 anywhere.
        Assert.Equal(30, Offset(subtitle).X, 1);
        Assert.Equal(20, Offset(subtitle).Y, 1);
    }

    [Fact]
    public void The_Panel_Asks_For_The_Union_Of_The_Solved_Positions()
    {
        var (panel, _, _, _) = Badge();

        Layout(panel, width: 300, height: 200);

        // Widest chain is icon + title, tallest is title + subtitle.
        Assert.Equal(80, panel.DesiredSize.Width, 1);
        Assert.Equal(36, panel.DesiredSize.Height, 1);
    }

    [Fact]
    public void Records_The_Relationships_As_Attached_References()
    {
        var (_, icon, title, subtitle) = Badge();

        // The attached values point at the sibling elements themselves - that is what
        // makes the layout survive a size change without any recalculation here.
        Assert.Same(icon, RelativePanel.GetRightOf(title));
        Assert.Same(icon, RelativePanel.GetAlignTopWith(title));
        Assert.Same(title, RelativePanel.GetBelow(subtitle));
        Assert.Same(title, RelativePanel.GetAlignLeftWith(subtitle));
    }

    [Fact]
    public void Follows_A_Bigger_Icon_Without_Any_Coordinates_Changing()
    {
        var icon = new Border { Width = 64, Height = 30 };
        var title = new Border { Width = 50, Height = 20 };
        var subtitle = new Border { Width = 40, Height = 16 };

        Layout(Ex016_RelativePanelAlign.CreateBadge(icon, title, subtitle), width: 300, height: 200);

        Assert.Equal(64, Offset(title).X, 1);
        Assert.Equal(64, Offset(subtitle).X, 1);
    }
}
