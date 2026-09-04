using System.Reflection;
using System.Windows;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex001_ClrToDependencyPropertyTests : WpfTestContext
{
    // Reflected, not referenced directly: the field does not exist yet in the stub, and
    // the test has to fail on the TODO rather than on a compile error.
    private static DependencyProperty ThresholdProperty
    {
        get
        {
            var registration = typeof(Ex001_ClrToDependencyProperty).GetField(
                "ThresholdProperty",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.NotNull(registration);
            Assert.True(registration!.IsInitOnly, "ThresholdProperty must be readonly - the field is the property's identity.");
            Assert.Equal(typeof(DependencyProperty), registration.FieldType);

            var value = registration.GetValue(null) as DependencyProperty;
            Assert.NotNull(value);
            return value!;
        }
    }

    [WpfFact]
    public void Exposes_The_Registration_As_A_Public_Static_Readonly_Field()
    {
        Assert.NotNull(ThresholdProperty);
    }

    [WpfFact]
    public void Registers_Under_The_Expected_Name_And_Owner()
    {
        Assert.Equal("Threshold", ThresholdProperty.Name);
        Assert.Equal(typeof(int), ThresholdProperty.PropertyType);
        Assert.Equal(typeof(Ex001_ClrToDependencyProperty), ThresholdProperty.OwnerType);
    }

    [WpfFact]
    public void Defaults_To_The_Registered_Default()
    {
        Assert.Equal(5, new Ex001_ClrToDependencyProperty().Threshold);
    }

    [WpfFact]
    public void Clr_Property_Reads_And_Writes_The_Dependency_Property()
    {
        var gauge = new Ex001_ClrToDependencyProperty();

        // A plain auto-property would keep these two views of "Threshold" apart.
        gauge.Threshold = 7;
        Assert.Equal(7, gauge.GetValue(ThresholdProperty));

        gauge.SetValue(ThresholdProperty, 9);
        Assert.Equal(9, gauge.Threshold);
    }

    [WpfFact]
    public void Clearing_The_Value_Falls_Back_To_The_Default()
    {
        var gauge = new Ex001_ClrToDependencyProperty { Threshold = 42 };

        gauge.ClearValue(ThresholdProperty);

        Assert.Equal(5, gauge.Threshold);
    }
}
