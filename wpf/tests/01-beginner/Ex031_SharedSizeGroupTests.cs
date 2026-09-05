using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex031_SharedSizeGroupTests : WpfTestContext
{
    [WpfFact]
    public void With_The_Scope_Applied_Both_Rows_Measure_To_The_Larger_Height()
    {
        var outer = Ex031_SharedSizeGroup.BuildTwoRowsSharingSize("RowGroup", leftContentHeight: 20, rightContentHeight: 80, applySharedSizeScope: true);

        Assert.True(Grid.GetIsSharedSizeScope(outer));

        var left = Assert.IsType<Grid>(outer.Children[0]);
        var right = Assert.IsType<Grid>(outer.Children[1]);
        Assert.Equal(GridUnitType.Auto, left.RowDefinitions[0].Height.GridUnitType);
        Assert.Equal(GridUnitType.Auto, right.RowDefinitions[0].Height.GridUnitType);
        // The actual group name must be the one passed in, not a hard-coded literal shared
        // by both rows regardless of the parameter - a stub that ignores
        // sharedSizeGroupName entirely (writing the same literal on both rows) would still
        // share correctly below, so the name itself has to be checked here.
        Assert.Equal("RowGroup", left.RowDefinitions[0].SharedSizeGroup);
        Assert.Equal("RowGroup", right.RowDefinitions[0].SharedSizeGroup);

        // Defensive, not required: measured directly, both of these Grids already acquire a
        // RowDefinition and a child Border, which flips IsInitialized on its own (see the
        // IsInitialized/AddLogicalChild finding in README.md) - deleting this call changes
        // nothing here. Unlike ItemsControl in rows 032-034, a Grid has no equivalent gap.
        CompleteInitialization(outer);
        // A single Layout(...) call is enough - the shared-size negotiation resolves within
        // one measure/arrange pass once the tree is initialized; no Pump() needed.
        Layout(outer, new Size(400, 300));

        // The distinguishing check: BOTH rows end up at the LARGER of the two content
        // heights, even though they belong to two different Grids with no shared
        // RowDefinitions collection - that is what SharedSizeGroup + IsSharedSizeScope
        // actually do, as opposed to each Grid just sizing its own Auto row independently.
        Assert.Equal(80.0, left.RowDefinitions[0].ActualHeight);
        Assert.Equal(80.0, right.RowDefinitions[0].ActualHeight);
    }

    [WpfFact]
    public void A_Different_Group_Name_And_Heights_Still_Share_Correctly()
    {
        // Different group name, different (and reversed) heights than the test above - a
        // hard-coded 80.0 (or a hard-coded group name) cannot satisfy both.
        var outer = Ex031_SharedSizeGroup.BuildTwoRowsSharingSize("OtherGroup", leftContentHeight: 150, rightContentHeight: 45, applySharedSizeScope: true);

        var left = Assert.IsType<Grid>(outer.Children[0]);
        var right = Assert.IsType<Grid>(outer.Children[1]);
        Assert.Equal("OtherGroup", left.RowDefinitions[0].SharedSizeGroup);
        Assert.Equal("OtherGroup", right.RowDefinitions[0].SharedSizeGroup);

        CompleteInitialization(outer);
        Layout(outer, new Size(400, 300));

        Assert.Equal(150.0, left.RowDefinitions[0].ActualHeight);
        Assert.Equal(150.0, right.RowDefinitions[0].ActualHeight);
    }

    [WpfFact]
    public void Without_The_Scope_The_Same_Group_Name_Does_Not_Share_Anything()
    {
        var outer = Ex031_SharedSizeGroup.BuildTwoRowsSharingSize("NoScopeGroup", leftContentHeight: 30, rightContentHeight: 90, applySharedSizeScope: false);

        Assert.False(Grid.GetIsSharedSizeScope(outer));

        var left = Assert.IsType<Grid>(outer.Children[0]);
        var right = Assert.IsType<Grid>(outer.Children[1]);
        Assert.Equal("NoScopeGroup", left.RowDefinitions[0].SharedSizeGroup);
        Assert.Equal("NoScopeGroup", right.RowDefinitions[0].SharedSizeGroup);

        CompleteInitialization(outer);
        Layout(outer, new Size(400, 300));

        // With no scope ancestor, each row sizes to its OWN content - the group name alone,
        // with nothing to register it into, does nothing. This is the assertion a stub that
        // "does nothing" (never wires the ancestor flag at all, in either branch) cannot
        // dodge: it would make this pass but the two tests above fail instead.
        Assert.Equal(30.0, left.RowDefinitions[0].ActualHeight);
        Assert.Equal(90.0, right.RowDefinitions[0].ActualHeight);
    }
}
