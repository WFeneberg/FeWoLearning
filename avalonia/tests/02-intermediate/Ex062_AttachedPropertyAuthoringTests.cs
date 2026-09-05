using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex062_AttachedPropertyAuthoringTests
{
    // Mechanism check: only AvaloniaProperty.RegisterAttached produces an
    // AttachedProperty whose IsAttached is true and whose OwnerType is the
    // decorating class rather than the decorated control. A styled property
    // declared on a Control subclass, or a plain static CLR property, fails
    // this even when every behavioural assertion below passes.
    [AvaloniaFact]
    public void Registers_A_Real_Attached_Property()
    {
        var property = Ex062_AttachedPropertyAuthoring.BadgeCountProperty;

        Assert.True(property.IsAttached);
        Assert.Equal("BadgeCount", property.Name);
        Assert.Equal(typeof(Ex062_AttachedPropertyAuthoring), property.OwnerType);
        Assert.Equal(typeof(int), property.PropertyType);
    }

    // Crossing the two APIs on purpose: written through the static accessor,
    // read straight out of the property system. A static Dictionary keyed by
    // control - the answer that otherwise behaves identically - stores nothing
    // the property system can see, so this comes back 0.
    [AvaloniaFact]
    public void The_Set_Accessor_Writes_Into_The_Property_System()
    {
        var target = new Border();

        Ex062_AttachedPropertyAuthoring.SetBadgeCount(target, 3);

        Assert.Equal(3, target.GetValue(Ex062_AttachedPropertyAuthoring.BadgeCountProperty));
    }

    // The mirror of the above, so neither accessor can be the side table's
    // private half.
    [AvaloniaFact]
    public void The_Get_Accessor_Reads_From_The_Property_System()
    {
        var target = new Border();

        target.SetValue(Ex062_AttachedPropertyAuthoring.BadgeCountProperty, 5);

        Assert.Equal(5, Ex062_AttachedPropertyAuthoring.GetBadgeCount(target));
    }

    // Deliberately does NOT go through SetBadgeCount. A tooltip written inside
    // that accessor is bypassed here, so this passes only if the work lives in
    // a change handler subscribed to BadgeCountProperty.Changed - which is also
    // the only version that survives the property being set from XAML, a style
    // or a binding.
    [AvaloniaFact]
    public void A_Change_Handler_Not_The_Set_Accessor_Updates_The_Tooltip()
    {
        var target = new Border();

        target.SetValue(Ex062_AttachedPropertyAuthoring.BadgeCountProperty, 7);

        Assert.Equal("7 items", ToolTip.GetTip(target));
    }

    [AvaloniaFact]
    public void A_Count_Of_Zero_Clears_The_Tooltip()
    {
        var target = new Border();

        Ex062_AttachedPropertyAuthoring.SetBadgeCount(target, 7);
        Ex062_AttachedPropertyAuthoring.SetBadgeCount(target, 0);

        Assert.Null(ToolTip.GetTip(target));
    }

    // Two things at once: per-target storage (a single static backing int would
    // fail), and that the property attaches to any Control rather than to one
    // hard-coded type - a TextBlock and a Border are unrelated leaves of the
    // hierarchy.
    [AvaloniaFact]
    public void Each_Target_Carries_Its_Own_Count()
    {
        var border = new Border();
        var text = new TextBlock();

        Ex062_AttachedPropertyAuthoring.SetBadgeCount(border, 2);
        Ex062_AttachedPropertyAuthoring.SetBadgeCount(text, 9);

        Assert.Equal(2, Ex062_AttachedPropertyAuthoring.GetBadgeCount(border));
        Assert.Equal(9, Ex062_AttachedPropertyAuthoring.GetBadgeCount(text));
        Assert.Equal("2 items", ToolTip.GetTip(border));
        Assert.Equal("9 items", ToolTip.GetTip(text));
    }
}
