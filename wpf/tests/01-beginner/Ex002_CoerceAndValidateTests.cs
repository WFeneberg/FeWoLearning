using System.Reflection;
using System.Windows;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex002_CoerceAndValidateTests : WpfTestContext
{
    private static DependencyProperty Property(string fieldName)
    {
        var registration = typeof(Ex002_CoerceAndValidate).GetField(
            fieldName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        Assert.NotNull(registration);
        Assert.True(registration!.IsInitOnly, $"{fieldName} must be readonly - the field is the property's identity.");
        Assert.Equal(typeof(DependencyProperty), registration.FieldType);

        var value = registration.GetValue(null) as DependencyProperty;
        Assert.NotNull(value);
        return value!;
    }

    [WpfFact]
    public void Defaults_Come_From_The_Registrations()
    {
        var mixer = new Ex002_CoerceAndValidate();

        Assert.Equal(50, mixer.Volume);
        Assert.Equal(100, mixer.Maximum);
    }

    [WpfFact]
    public void Clr_Wrapper_And_Dependency_Property_Read_And_Write_The_Same_Storage()
    {
        var mixer = new Ex002_CoerceAndValidate();
        var volumeProperty = Property("VolumeProperty");

        // Values stay inside [0, Maximum] so coercion never enters the picture here -
        // this test is only about whether Volume and VolumeProperty are one storage.
        mixer.Volume = 70;
        Assert.Equal(70, mixer.GetValue(volumeProperty));

        mixer.SetValue(volumeProperty, 30);
        Assert.Equal(30, mixer.Volume);
    }

    [WpfFact]
    public void Coercion_Clamps_Into_The_Allowed_Range()
    {
        var mixer = new Ex002_CoerceAndValidate();

        mixer.Volume = 500;
        Assert.Equal(100, mixer.Volume);

        mixer.Volume = -5;
        Assert.Equal(0, mixer.Volume);
    }

    [WpfFact]
    public void Coercion_Applies_Even_When_Written_Through_SetValue()
    {
        var mixer = new Ex002_CoerceAndValidate();
        var volumeProperty = Property("VolumeProperty");

        // Going around the CLR wrapper is exactly what a binding, a style setter or an
        // animation does - a setter-side clamp cannot see this write at all, only a
        // CoerceValueCallback registered on the property itself can.
        mixer.SetValue(volumeProperty, 500);

        Assert.Equal(100, mixer.Volume);
        Assert.Equal(100, mixer.GetValue(volumeProperty));
    }

    [WpfFact]
    public void Lowering_The_Maximum_Re_Coerces_The_Volume()
    {
        var mixer = new Ex002_CoerceAndValidate { Volume = 90 };

        mixer.Maximum = 40;

        // This is what CoerceValue(VolumeProperty) buys: a value that was legal becomes
        // illegal because a *different* property changed, and the store has to catch up.
        Assert.Equal(40, mixer.Volume);
    }

    [WpfFact]
    public void Raising_The_Maximum_Again_Restores_The_Original_Value()
    {
        var mixer = new Ex002_CoerceAndValidate { Volume = 90 };

        mixer.Maximum = 40;
        mixer.Maximum = 200;

        // Coercion does not overwrite the local value, it only masks it - so the 90 is
        // still in the store and comes back. A setter that clamped by hand lost it.
        Assert.Equal(90, mixer.Volume);
    }

    [WpfFact]
    public void Validation_Rejects_A_Value_Outright()
    {
        var mixer = new Ex002_CoerceAndValidate();

        // ValidateValueCallback returning false is a hard error, not a clamp: WPF wraps
        // it in an ArgumentException and the store is left untouched.
        Assert.Throws<ArgumentException>(() => mixer.Volume = -5000);
        Assert.Equal(50, mixer.Volume);
    }

    [WpfFact]
    public void Validation_Rejects_A_Raw_SetValue_Too()
    {
        var mixer = new Ex002_CoerceAndValidate();
        var volumeProperty = Property("VolumeProperty");

        // Same hard rejection when the write goes around the CLR wrapper entirely.
        Assert.Throws<ArgumentException>(() => mixer.SetValue(volumeProperty, -5000));
        Assert.Equal(50, mixer.Volume);
    }

    [WpfFact]
    public void Property_Changed_Callback_Sees_Old_And_New_Values()
    {
        var mixer = new Ex002_CoerceAndValidate();

        mixer.Volume = 70;
        mixer.Volume = 20;

        Assert.Equal(new[] { (50, 70), (70, 20) }, mixer.Changes);
    }

    [WpfFact]
    public void Property_Changed_Callback_Reports_The_Coerced_Value_Not_The_Raw_One()
    {
        var mixer = new Ex002_CoerceAndValidate();

        mixer.Volume = 500;

        // Coercion runs before the changed callback, so the callback never sees the 500.
        // That ordering is the whole point of the exercise.
        Assert.Equal(new[] { (50, 100) }, mixer.Changes);
    }

    [WpfFact]
    public void Clearing_The_Value_Falls_Back_To_The_Registered_Default()
    {
        var mixer = new Ex002_CoerceAndValidate { Volume = 90 };
        var volumeProperty = Property("VolumeProperty");

        mixer.ClearValue(volumeProperty);

        Assert.Equal(50, mixer.Volume);
    }

    [WpfFact]
    public void Registrations_Are_Named_And_Owned_As_Expected()
    {
        var volume = Property("VolumeProperty");
        var maximum = Property("MaximumProperty");

        Assert.Equal("Volume", volume.Name);
        Assert.Equal("Maximum", maximum.Name);
        Assert.Equal(typeof(int), volume.PropertyType);
        Assert.Equal(typeof(int), maximum.PropertyType);
        Assert.Equal(typeof(Ex002_CoerceAndValidate), volume.OwnerType);
        Assert.Equal(typeof(Ex002_CoerceAndValidate), maximum.OwnerType);
    }
}
