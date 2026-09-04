using System.Reflection;
using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex001_HelloPropertyTests : UnoTestContext
{
    private static DependencyProperty LevelProperty
    {
        get
        {
            var registration = typeof(Ex001_HelloProperty).GetField(
                "LevelProperty",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.True(
                registration is not null,
                "Ex001_HelloProperty declares no public static field called LevelProperty - the field is the property's identity.");
            Assert.True(registration!.IsInitOnly, "LevelProperty must be readonly - the field is the property's identity.");
            Assert.Equal(typeof(DependencyProperty), registration.FieldType);

            var value = registration.GetValue(null) as DependencyProperty;
            Assert.True(value is not null, "LevelProperty is null - was DependencyProperty.Register ever called?");
            return value!;
        }
    }

    [Fact]
    public void Exposes_The_Registration_As_A_Public_Static_Readonly_Field()
    {
        Assert.NotNull(LevelProperty);
    }

    [Fact]
    public void Defaults_To_The_Registered_Default()
    {
        Assert.Equal(5, new Ex001_HelloProperty().Level);
    }

    [Fact]
    public void Round_Trips_A_Value()
    {
        var gauge = new Ex001_HelloProperty { Level = 42 };

        Assert.Equal(42, gauge.Level);
    }

    [Fact]
    public void Clr_Property_Reads_And_Writes_The_Dependency_Property()
    {
        var gauge = new Ex001_HelloProperty();

        // A plain auto-property would keep these two views of "Level" apart.
        gauge.Level = 7;
        Assert.Equal(7, gauge.GetValue(LevelProperty));

        gauge.SetValue(LevelProperty, 9);
        Assert.Equal(9, gauge.Level);
    }

    [Fact]
    public void Clearing_The_Value_Falls_Back_To_The_Default()
    {
        var gauge = new Ex001_HelloProperty { Level = 42 };

        gauge.ClearValue(LevelProperty);

        Assert.Equal(5, gauge.Level);
    }
}
