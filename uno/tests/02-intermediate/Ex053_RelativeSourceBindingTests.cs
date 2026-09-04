using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex053_RelativeSourceBindingTests : UnoTestContext
{
    [Fact]
    public void A_Self_Bound_Label_Reads_Its_Own_Property()
    {
        var label = Ex053_RelativeSourceBinding.CreateSelfBoundLabel("mine");

        Assert.Equal("mine", label.Text);
    }

    [Fact]
    public void A_Self_Bound_Label_Follows_Its_Own_Property()
    {
        var label = Ex053_RelativeSourceBinding.CreateSelfBoundLabel("mine");

        label.Tag = "changed";

        // Self is a live binding, not a copy - Tag is a dependency property, so the
        // property system raises the change the binding hears.
        Assert.Equal("changed", label.Text);
    }

    [Fact]
    public void A_Self_Bound_Label_Needs_No_Tree()
    {
        var label = Ex053_RelativeSourceBinding.CreateSelfBoundLabel("mine");

        // No parent, no DataContext, no name scope. That is what makes Self the right tool
        // for a converter over one of the element's own properties.
        Assert.Null(label.Parent);
        Assert.Equal("mine", label.Text);
    }

    [Fact]
    public void Template_Binding_Reaches_The_Templated_Parent()
    {
        var card = Layout(Ex053_RelativeSourceBinding.CreateTemplatedCard("owner"));

        Assert.Equal("owner", FindDescendant<TextBlock>(card, "PART_Short").Text);
    }

    [Fact]
    public void The_Long_Form_Reaches_The_Same_Parent()
    {
        var card = Layout(Ex053_RelativeSourceBinding.CreateTemplatedCard("owner"));

        Assert.Equal("owner", FindDescendant<TextBlock>(card, "PART_Long").Text);
    }

    [Fact]
    public void Both_Forms_Follow_A_Later_Change()
    {
        var card = Layout(Ex053_RelativeSourceBinding.CreateTemplatedCard("owner"));

        card.Tag = "new owner";

        Assert.Equal("new owner", FindDescendant<TextBlock>(card, "PART_Short").Text);
        Assert.Equal("new owner", FindDescendant<TextBlock>(card, "PART_Long").Text);
    }

    [Fact]
    public void The_Template_Is_Reused_Per_Card()
    {
        var first = Layout(Ex053_RelativeSourceBinding.CreateTemplatedCard("one"));
        var second = Layout(Ex053_RelativeSourceBinding.CreateTemplatedCard("two"));

        // TemplatedParent means "the control this copy of the template belongs to", which
        // is why one template serves every instance.
        Assert.Equal("one", FindDescendant<TextBlock>(first, "PART_Short").Text);
        Assert.Equal("two", FindDescendant<TextBlock>(second, "PART_Short").Text);
    }
}
