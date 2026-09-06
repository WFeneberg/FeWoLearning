using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex059_VisualStateManagerTests : WpfTestContext
{
    [WpfFact]
    public void BuildGroup_Attaches_The_Group_So_It_Is_Actually_Reachable()
    {
        var root = new Grid();

        var group = Ex059_VisualStateManager.BuildGroup(root, "CommonStates", "Normal", "Highlighted");

        Assert.Equal("CommonStates", group.Name);
        Assert.Equal(2, group.States.Count);
        Assert.Contains(group.States.Cast<VisualState>(), s => s.Name == "Normal");
        Assert.Contains(group.States.Cast<VisualState>(), s => s.Name == "Highlighted");

        // Against a bypass that builds the group but never attaches it to root: the group would
        // be unreachable through the mechanism a real templated control uses to find it.
        var groups = VisualStateManager.GetVisualStateGroups(root).Cast<VisualStateGroup>();
        Assert.Contains(group, groups);
    }

    [WpfFact]
    public void RequestState_Moves_CurrentState_And_Reports_Success()
    {
        var root = new Grid();
        var group = Ex059_VisualStateManager.BuildGroup(root, "CommonStates", "Normal", "Highlighted");

        var result = Ex059_VisualStateManager.RequestState(root, "Highlighted");

        Assert.True(result);
        Assert.Equal("Highlighted", group.CurrentState?.Name);
    }

    [WpfFact]
    public void RequestState_With_An_Unknown_Name_Reports_Failure_And_Leaves_CurrentState_Unchanged()
    {
        var root = new Grid();
        var group = Ex059_VisualStateManager.BuildGroup(root, "CommonStates", "Normal", "Highlighted");
        Ex059_VisualStateManager.RequestState(root, "Highlighted");

        var result = Ex059_VisualStateManager.RequestState(root, "NoSuchState");

        // Against a bypass that hardcodes a true return: an unknown name must come back false.
        Assert.False(result);
        Assert.Equal("Highlighted", group.CurrentState?.Name);
    }

    [WpfFact]
    public void A_Different_Group_And_State_Names_Also_Work()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance - a hardcoded
        // "Highlighted" from the tests above cannot satisfy this one too.
        var root = new Grid();
        var group = Ex059_VisualStateManager.BuildGroup(root, "VisibilityStates", "Visible", "Hidden", "Collapsed");

        var result = Ex059_VisualStateManager.RequestState(root, "Hidden");

        Assert.True(result);
        Assert.Equal("Hidden", group.CurrentState?.Name);

        var second = Ex059_VisualStateManager.RequestState(root, "Collapsed");
        Assert.True(second);
        Assert.Equal("Collapsed", group.CurrentState?.Name);
    }
}
