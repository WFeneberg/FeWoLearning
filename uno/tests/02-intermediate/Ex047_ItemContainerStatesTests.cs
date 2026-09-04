using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex047_ItemContainerStatesTests : UnoTestContext
{
    private static Ex047_SelectionGroup Group()
    {
        var group = new Ex047_SelectionGroup("one", "two", "three");

        foreach (var container in group.Containers)
        {
            Layout(container);
        }

        return group;
    }

    private static double Highlight(Ex047_ItemContainerStates container) =>
        FindDescendant<Border>(container, "PART_Highlight").Opacity;

    [Fact]
    public void Builds_One_Container_Per_Item()
    {
        var group = Group();

        Assert.Equal(3, group.Containers.Count);
        Assert.Equal("one", group.Containers[0].Content);
        Assert.Equal("three", group.Containers[2].Content);
    }

    [Fact]
    public void Each_Container_Comes_Templated()
    {
        var group = Group();

        Assert.All(
            group.Containers,
            container => Assert.Same(Ex047_ItemContainerStates.ContainerTemplate, container.Template));
    }

    [Fact]
    public void Nothing_Is_Selected_To_Begin_With()
    {
        var group = Group();

        Assert.Null(group.SelectedIndex);
        Assert.All(group.Containers, container => Assert.Equal(0, Highlight(container), 2));
    }

    [Fact]
    public void Selecting_Highlights_That_Container()
    {
        var group = Group();

        group.Select(1);

        Assert.Equal(1, group.SelectedIndex);
        Assert.Equal(1, Highlight(group.Containers[1]), 2);
    }

    [Fact]
    public void Selecting_Leaves_The_Others_Alone()
    {
        var group = Group();

        group.Select(1);

        Assert.Equal(0, Highlight(group.Containers[0]), 2);
        Assert.Equal(0, Highlight(group.Containers[2]), 2);
    }

    [Fact]
    public void Selecting_Another_One_Moves_The_Selection()
    {
        var group = Group();

        group.Select(1);
        group.Select(2);

        // The container itself knows nothing about its siblings - the group is the only
        // thing that enforces "one at a time".
        Assert.Equal(2, group.SelectedIndex);
        Assert.Equal(0, Highlight(group.Containers[1]), 2);
        Assert.Equal(1, Highlight(group.Containers[2]), 2);
    }

    [Fact]
    public void Selecting_Out_Of_Range_Clears_The_Selection()
    {
        var group = Group();
        group.Select(1);

        group.Select(-1);

        Assert.Null(group.SelectedIndex);
        Assert.All(group.Containers, container => Assert.Equal(0, Highlight(container), 2));
    }

    [Fact]
    public void Selecting_The_Same_One_Twice_Keeps_It_Selected()
    {
        var group = Group();

        group.Select(1);
        group.Select(1);

        Assert.Equal(1, group.SelectedIndex);
        Assert.Equal(1, Highlight(group.Containers[1]), 2);
    }

    [Fact]
    public void Setting_IsSelected_Directly_Still_Updates_The_Visual_State()
    {
        var group = Group();

        group.Containers[0].IsSelected = true;

        // The container reacts to its own property, whoever sets it. A policy that only
        // works through the group would break every keyboard and automation path.
        Assert.Equal(1, Highlight(group.Containers[0]), 2);
        Assert.Equal(0, group.SelectedIndex);
    }

    [Fact]
    public void A_Container_Selected_Before_Its_Template_Comes_Up_Highlighted()
    {
        var container = new Ex047_ItemContainerStates
        {
            Content = "late",
            IsSelected = true,
            Template = Ex047_ItemContainerStates.ContainerTemplate,
        };

        Layout(container);

        Assert.Equal(1, Highlight(container), 2);
    }
}
