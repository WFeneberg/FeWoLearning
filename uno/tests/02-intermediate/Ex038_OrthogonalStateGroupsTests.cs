using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex038_OrthogonalStateGroupsTests : UnoTestContext
{
    private static Ex038_OrthogonalStateGroups Control(bool available = true, bool checkedState = false) =>
        Layout(new Ex038_OrthogonalStateGroups
        {
            Template = Ex038_OrthogonalStateGroups.TwoGroupTemplate,
            IsAvailable = available,
            IsChecked = checkedState,
        });

    private static Border Fill(Ex038_OrthogonalStateGroups control) =>
        FindDescendant<Border>(control, "PART_Fill");

    [Fact]
    public void Starts_Available_And_Unchecked()
    {
        var fill = Fill(Control());

        Assert.Equal(1, fill.Opacity, 2);
        Assert.Equal(20, fill.Width, 1);
    }

    [Fact]
    public void Availability_Alone_Changes_Only_The_Opacity()
    {
        var control = Control();

        control.IsAvailable = false;

        Assert.Equal(0.4, Fill(control).Opacity, 2);
        Assert.Equal(20, Fill(control).Width, 1);
    }

    [Fact]
    public void Checking_Alone_Changes_Only_The_Width()
    {
        var control = Control();

        control.IsChecked = true;

        Assert.Equal(1, Fill(control).Opacity, 2);
        Assert.Equal(60, Fill(control).Width, 1);
    }

    [Fact]
    public void Both_States_Apply_At_The_Same_Time()
    {
        var control = Control();

        control.IsAvailable = false;
        control.IsChecked = true;

        // One state per group, both groups active. Neither setter undoes the other.
        Assert.Equal(0.4, Fill(control).Opacity, 2);
        Assert.Equal(60, Fill(control).Width, 1);
    }

    [Fact]
    public void Changing_One_Group_Does_Not_Reset_The_Other()
    {
        var control = Control();
        control.IsChecked = true;

        control.IsAvailable = false;
        control.IsAvailable = true;

        // The classic symptom of updating only the group whose property moved: the other
        // group's state is dropped the next time the states are re-applied.
        Assert.Equal(60, Fill(control).Width, 1);
    }

    [Fact]
    public void Requests_Both_Groups_On_Every_Update()
    {
        var control = Control();
        var before = control.LastRequestedStates.Count;

        control.IsChecked = true;

        Assert.Equal(before + 2, control.LastRequestedStates.Count);
        Assert.Equal("Available", control.LastRequestedStates[^2]);
        Assert.Equal("Checked", control.LastRequestedStates[^1]);
    }

    [Fact]
    public void A_Control_Templated_Late_Comes_Up_In_Both_States()
    {
        var control = new Ex038_OrthogonalStateGroups { IsAvailable = false, IsChecked = true };

        control.Template = Ex038_OrthogonalStateGroups.TwoGroupTemplate;
        Layout(control);

        Assert.Equal(0.4, Fill(control).Opacity, 2);
        Assert.Equal(60, Fill(control).Width, 1);
    }

    [Fact]
    public void Leaving_A_State_Undoes_Only_Its_Own_Setter()
    {
        var control = Control(available: false, checkedState: true);

        control.IsChecked = false;

        Assert.Equal(0.4, Fill(control).Opacity, 2);
        Assert.Equal(20, Fill(control).Width, 1);
    }
}
