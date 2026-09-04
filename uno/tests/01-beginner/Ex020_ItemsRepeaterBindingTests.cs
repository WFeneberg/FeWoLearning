using FeWoLearning.Uno.Exercises.Beginner;
using FeWoLearning.Uno.Support;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex020_ItemsRepeaterBindingTests : UnoTestContext
{
    private static readonly string[] Fruit = ["apple", "pear", "plum"];

    private static List<string> Texts(ItemsRepeater repeater) =>
        Descendants(repeater).OfType<TextBlock>().Select(t => t.Text).ToList();

    [Fact]
    public void The_Item_Template_Is_One_Shared_Instance()
    {
        Assert.Same(Ex020_ItemsRepeaterBinding.ItemTemplate, Ex020_ItemsRepeaterBinding.ItemTemplate);
    }

    [Fact]
    public void Wires_Up_Source_Template_And_Layout()
    {
        var repeater = Ex020_ItemsRepeaterBinding.CreateList(Fruit);

        Assert.Same(Fruit, repeater.ItemsSource);
        Assert.Same(Ex020_ItemsRepeaterBinding.ItemTemplate, repeater.ItemTemplate);
        Assert.IsType<StackEverythingLayout>(repeater.Layout);
    }

    [Fact]
    public void Views_The_Collection_Through_ItemsSourceView()
    {
        var repeater = Ex020_ItemsRepeaterBinding.CreateList(Fruit);

        // Whatever collection type goes in, the repeater works against this one view.
        Assert.Equal(3, repeater.ItemsSourceView.Count);
    }

    [Fact]
    public void Builds_One_Element_Per_Item()
    {
        var repeater = Layout(Ex020_ItemsRepeaterBinding.CreateList(Fruit));

        Assert.Equal(3, Texts(repeater).Count);
    }

    [Fact]
    public void Keeps_The_Items_In_Source_Order()
    {
        var repeater = Layout(Ex020_ItemsRepeaterBinding.CreateList(Fruit));

        Assert.Equal(Fruit, Texts(repeater));
    }

    [Fact]
    public void An_Empty_Path_Binds_To_The_Item_Itself()
    {
        var repeater = Layout(Ex020_ItemsRepeaterBinding.CreateList(new[] { "only" }));

        // The items are strings: there is no Caption to reach for, only {Binding}.
        Assert.Equal(["only"], Texts(repeater));
    }

    [Fact]
    public void Stacks_The_Elements_Vertically()
    {
        var repeater = Layout(Ex020_ItemsRepeaterBinding.CreateList(Fruit));

        var blocks = Descendants(repeater).OfType<TextBlock>().ToList();

        Assert.Equal(0, Offset(blocks[0]).Y, 1);
        Assert.True(
            Offset(blocks[1]).Y >= blocks[0].DesiredSize.Height,
            $"second element at {Offset(blocks[1]).Y}, first is {blocks[0].DesiredSize.Height} tall");
    }

    [Fact]
    public void An_Empty_Source_Builds_Nothing()
    {
        var repeater = Layout(Ex020_ItemsRepeaterBinding.CreateList(Array.Empty<string>()));

        Assert.Empty(Texts(repeater));
        Assert.Equal(0, repeater.ItemsSourceView.Count);
    }
}
