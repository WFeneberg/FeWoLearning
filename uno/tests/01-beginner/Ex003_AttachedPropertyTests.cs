using System.Reflection;
using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex003_AttachedPropertyTests : UnoTestContext
{
    private static DependencyProperty SlotProperty
    {
        get
        {
            var registration = typeof(Ex003_AttachedProperty).GetField(
                "SlotProperty",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.NotNull(registration);
            Assert.True(registration!.IsInitOnly, "SlotProperty must be readonly - the field is the property's identity.");
            Assert.Equal(typeof(DependencyProperty), registration.FieldType);

            var value = registration.GetValue(null) as DependencyProperty;
            Assert.NotNull(value);
            return value!;
        }
    }

    [Fact]
    public void Exposes_The_Registration_As_A_Public_Static_Readonly_Field()
    {
        Assert.NotNull(SlotProperty);
    }

    [Fact]
    public void An_Untouched_Element_Reports_The_Registered_Default()
    {
        Assert.Equal(-1, Ex003_AttachedProperty.GetSlot(new Border()));
    }

    [Fact]
    public void Round_Trips_A_Value_On_An_Element()
    {
        var border = new Border();

        Ex003_AttachedProperty.SetSlot(border, 2);

        Assert.Equal(2, Ex003_AttachedProperty.GetSlot(border));
    }

    [Fact]
    public void Each_Element_Carries_Its_Own_Value()
    {
        var first = new Border();
        var second = new Border();

        Ex003_AttachedProperty.SetSlot(first, 1);
        Ex003_AttachedProperty.SetSlot(second, 9);

        Assert.Equal(1, Ex003_AttachedProperty.GetSlot(first));
        Assert.Equal(9, Ex003_AttachedProperty.GetSlot(second));
    }

    [Fact]
    public void The_Value_Lives_On_The_Element_Not_In_A_Side_Table()
    {
        var border = new Border();

        Ex003_AttachedProperty.SetSlot(border, 4);
        Assert.Equal(4, border.GetValue(SlotProperty));

        // And the element is the single source of truth in both directions - this is what
        // lets XAML, styles and animations set an attached property without the accessors.
        border.SetValue(SlotProperty, 6);
        Assert.Equal(6, Ex003_AttachedProperty.GetSlot(border));
    }

    [Fact]
    public void Works_On_Any_DependencyObject_Not_Just_The_Intended_Parent()
    {
        // Grid.Row does not care whether the element is in a Grid, and neither does Slot.
        var text = new TextBlock();

        Ex003_AttachedProperty.SetSlot(text, 3);

        Assert.Equal(3, Ex003_AttachedProperty.GetSlot(text));
    }
}
