using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex032_UserControlCompositionTests
{
    private static Ex032_UserControlComposition Show() =>
        ViewHarness.Show(new Ex032_UserControlComposition(), 300, 200);

    // Structural: the composition must actually nest two INSTANCES of the
    // given Ex032_Badge control type - two plain TextBlocks with the right
    // literal text would satisfy every assertion below without ever nesting
    // a UserControl at all.
    [AvaloniaFact]
    public void Composition_Nests_Two_Badge_Instances()
    {
        var view = Show();
        var badges = view.GetSelfAndVisualDescendants().OfType<Ex032_Badge>().ToList();

        Assert.Equal(2, badges.Count);
    }

    [AvaloniaFact]
    public void Each_Nested_Badge_Has_Its_Own_Caption_Set_From_The_Host()
    {
        var view = Show();
        var first = view.FindControl<Ex032_Badge>("FirstBadge")!;
        var second = view.FindControl<Ex032_Badge>("SecondBadge")!;

        Assert.Equal("Alpha", first.Caption);
        Assert.Equal("Beta", second.Caption);
        Assert.NotEqual(first.Caption, second.Caption);
    }

    // The plain CLR property has to actually reach the badge's own rendered
    // content - nesting a Badge but forgetting to set Caption would leave
    // its default ("") showing instead of the literal the host declared.
    [AvaloniaFact]
    public void Each_Badges_Caption_Reaches_Its_Own_Rendered_Text()
    {
        var view = Show();
        var first = view.FindControl<Ex032_Badge>("FirstBadge")!;
        var second = view.FindControl<Ex032_Badge>("SecondBadge")!;

        var firstText = first.GetSelfAndVisualDescendants().OfType<TextBlock>()
            .Single(t => t.Name == "CaptionText");
        var secondText = second.GetSelfAndVisualDescendants().OfType<TextBlock>()
            .Single(t => t.Name == "CaptionText");

        Assert.Equal("Alpha", firstText.Text);
        Assert.Equal("Beta", secondText.Text);
    }
}
