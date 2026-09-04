using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex046_ElementFactoryContractTests : UnoTestContext
{
    [Fact]
    public void Builds_An_Element_Showing_The_Item()
    {
        var factory = new Ex046_ElementFactoryContract();

        var element = Assert.IsType<TextBlock>(factory.GetElement("apple"));

        Assert.Equal("apple", element.Text);
        Assert.Equal(1, factory.Constructed);
    }

    [Fact]
    public void Builds_A_New_One_While_The_Pool_Is_Empty()
    {
        var factory = new Ex046_ElementFactoryContract();

        var first = factory.GetElement("apple");
        var second = factory.GetElement("pear");

        Assert.NotSame(first, second);
        Assert.Equal(2, factory.Constructed);
    }

    [Fact]
    public void A_Recycled_Element_Goes_Into_The_Pool()
    {
        var factory = new Ex046_ElementFactoryContract();
        var element = factory.GetElement("apple");

        factory.RecycleElement(element);

        Assert.Equal(1, factory.Pooled);
    }

    [Fact]
    public void The_Pooled_Element_Is_Handed_Out_Again()
    {
        var factory = new Ex046_ElementFactoryContract();
        var element = factory.GetElement("apple");
        factory.RecycleElement(element);

        var reused = factory.GetElement("pear");

        // The same instance, re-pointed. This is the whole reason the contract has two
        // halves instead of one.
        Assert.Same(element, reused);
        Assert.Equal(1, factory.Constructed);
        Assert.Equal(0, factory.Pooled);
    }

    [Fact]
    public void A_Reused_Element_Shows_The_New_Item()
    {
        var factory = new Ex046_ElementFactoryContract();
        var element = (TextBlock)factory.GetElement("apple");
        factory.RecycleElement(element);

        var reused = (TextBlock)factory.GetElement("pear");

        Assert.Equal("pear", reused.Text);
    }

    [Fact]
    public void An_Element_In_The_Pool_Carries_No_Leftover_State()
    {
        var factory = new Ex046_ElementFactoryContract();
        var element = (TextBlock)factory.GetElement("apple");

        factory.RecycleElement(element);

        // Cleared on the way in, not on the way out. An element that keeps the old text
        // while it sits in the pool flashes the previous item when it is re-attached.
        Assert.Equal("", element.Text);
    }

    [Fact]
    public void A_Null_Item_Produces_An_Empty_Element()
    {
        var factory = new Ex046_ElementFactoryContract();

        var element = (TextBlock)factory.GetElement(null);

        Assert.Equal("", element.Text);
    }

    [Fact]
    public void Something_That_Is_Not_A_TextBlock_Is_Not_Pooled()
    {
        var factory = new Ex046_ElementFactoryContract();

        factory.RecycleElement(new Border());

        Assert.Equal(0, factory.Pooled);
    }

    [Fact]
    public void Reuse_Beats_Construction_Across_A_Whole_Pass()
    {
        var factory = new Ex046_ElementFactoryContract();
        var items = new[] { "apple", "pear", "plum", "fig", "sloe" };
        UIElement? previous = null;

        foreach (var item in items)
        {
            if (previous is not null)
            {
                factory.RecycleElement(previous);
            }

            previous = factory.GetElement(item);
        }

        // Five items, one element: exactly the trade a long list is built on.
        Assert.Equal(1, factory.Constructed);
    }
}
