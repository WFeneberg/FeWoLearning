using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex036_CustomTemplatedControlTests : UnoTestContext
{
    private static Ex036_CustomTemplatedControl Badge(string caption = "Inbox", int count = 3) =>
        new() { Caption = caption, Count = count };

    [Fact]
    public void Declares_Which_Default_Style_To_Look_For()
    {
        var badge = Badge();

        // Without this the framework looks for Control's default style and the control
        // comes up with no template at all.
        Assert.Equal(typeof(Ex036_CustomTemplatedControl), badge.DeclaredStyleKey);
    }

    [Fact]
    public void The_Default_Style_Supplies_Only_The_Template()
    {
        var style = Ex036_CustomTemplatedControl.CreateDefaultStyle();

        var setter = Assert.IsType<Setter>(Assert.Single(style.Setters));
        Assert.Equal(Control.TemplateProperty, setter.Property);
        Assert.Same(Ex036_CustomTemplatedControl.BadgeTemplate, setter.Value);
    }

    [Fact]
    public void A_Consumer_Only_Writes_The_Element()
    {
        var badge = Badge();

        Layout(Ex036_CustomTemplatedControl.CreateHost(badge));

        // Nothing set Template on the badge: the implicit style in the host did.
        Assert.Same(Ex036_CustomTemplatedControl.BadgeTemplate, badge.Template);
    }

    [Fact]
    public void The_Parts_Show_The_Properties()
    {
        var badge = Badge();

        Layout(Ex036_CustomTemplatedControl.CreateHost(badge));

        Assert.Equal("Inbox", FindDescendant<TextBlock>(badge, "PART_Caption").Text);
        Assert.Equal("3", FindDescendant<TextBlock>(badge, "PART_Count").Text);
    }

    [Fact]
    public void The_Parts_Follow_Later_Property_Changes()
    {
        var badge = Badge();
        Layout(Ex036_CustomTemplatedControl.CreateHost(badge));

        badge.Caption = "Archive";
        badge.Count = 12;

        // The template binds back to the templated parent, so the control never has to
        // push anything into its parts.
        Assert.Equal("Archive", FindDescendant<TextBlock>(badge, "PART_Caption").Text);
        Assert.Equal("12", FindDescendant<TextBlock>(badge, "PART_Count").Text);
    }

    [Fact]
    public void Several_Badges_In_One_Host_Are_Independent()
    {
        var first = Badge("Inbox", 3);
        var second = Badge("Spam", 99);

        Layout(Ex036_CustomTemplatedControl.CreateHost(first, second));

        Assert.Equal("Inbox", FindDescendant<TextBlock>(first, "PART_Caption").Text);
        Assert.Equal("99", FindDescendant<TextBlock>(second, "PART_Count").Text);
    }

    [Fact]
    public void Outside_The_Host_It_Has_No_Look_And_Does_Not_Break()
    {
        var badge = Badge();

        Layout(badge);

        // No style in scope, so no template. A control library that ships a type without
        // its style dictionary produces exactly this: an element that measures to nothing.
        Assert.Empty(Descendants(badge).OfType<TextBlock>());
        Assert.Equal(0, badge.DesiredSize.Height, 1);
    }

    [Fact]
    public void The_Host_Registers_The_Style_Under_The_Control_Type()
    {
        var host = Ex036_CustomTemplatedControl.CreateHost();

        Assert.IsType<Style>(host.Resources[typeof(Ex036_CustomTemplatedControl)]);
    }
}
