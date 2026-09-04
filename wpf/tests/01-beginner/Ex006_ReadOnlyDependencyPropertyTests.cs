using System.Windows;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex006_ReadOnlyDependencyPropertyTests : WpfTestContext
{
    private static DependencyProperty IsConnectedProperty
        => DependencyPropertyReflection.Property(typeof(Ex006_ConnectionMonitor), "IsConnectedProperty");

    private static DependencyPropertyKey IsConnectedPropertyKey
        => DependencyPropertyReflection.Key(typeof(Ex006_ConnectionMonitor), "IsConnectedPropertyKey");

    [WpfFact]
    public void Defaults_To_False()
    {
        var monitor = new Ex006_ConnectionMonitor();

        Assert.False(monitor.IsConnected);
    }

    [WpfFact]
    public void Registration_Is_Marked_Read_Only()
    {
        // The whole subject of the exercise, checked on the registration itself rather
        // than only inferred from behaviour.
        Assert.True(IsConnectedProperty.ReadOnly);
    }

    [WpfFact]
    public void The_Key_And_The_Property_Share_One_Identity()
    {
        Assert.Same(IsConnectedProperty, IsConnectedPropertyKey.DependencyProperty);
    }

    [WpfFact]
    public void Connect_Sets_IsConnected_True()
    {
        var monitor = new Ex006_ConnectionMonitor();

        monitor.Connect();

        Assert.True(monitor.IsConnected);
        Assert.Equal(true, monitor.GetValue(IsConnectedProperty));
    }

    [WpfFact]
    public void Disconnect_Sets_IsConnected_False()
    {
        var monitor = new Ex006_ConnectionMonitor();
        monitor.Connect();

        monitor.Disconnect();

        Assert.False(monitor.IsConnected);
        Assert.Equal(false, monitor.GetValue(IsConnectedProperty));
    }

    [WpfFact]
    public void Public_SetValue_On_The_Read_Only_Property_Is_Rejected()
    {
        var monitor = new Ex006_ConnectionMonitor();

        // This is the point of RegisterReadOnly: SetValue(DependencyProperty, ...) has no
        // idea a key exists and WPF refuses the write outright rather than silently
        // ignoring it. A property that merely happens to look read-only from the outside
        // (a getter with no public setter) would let this call through with no error at
        // all, which is exactly what this test rules out.
        Assert.Throws<InvalidOperationException>(() => monitor.SetValue(IsConnectedProperty, true));
    }

    [WpfFact]
    public void Only_A_Write_Through_The_Key_Succeeds()
    {
        var monitor = new Ex006_ConnectionMonitor();

        // Same instance, same underlying storage - only the handle used to reach it
        // differs. Proving both halves on the same object is what shows the read-only-ness
        // lives in the registration, not in some other accident of the class shape.
        Assert.Throws<InvalidOperationException>(() => monitor.SetValue(IsConnectedProperty, true));
        Assert.False(monitor.IsConnected);

        monitor.SetValue(IsConnectedPropertyKey, true);

        Assert.True(monitor.IsConnected);
        Assert.Equal(true, monitor.GetValue(IsConnectedProperty));
    }

    [WpfFact]
    public void Clearing_The_Value_Through_The_Property_Is_Rejected_Too()
    {
        var monitor = new Ex006_ConnectionMonitor();
        monitor.SetValue(IsConnectedPropertyKey, true);

        // ClearValue is a write, same as SetValue: WPF refuses it through the plain
        // DependencyProperty for a read-only registration exactly as it refuses SetValue.
        // Only ClearValue(DependencyPropertyKey) is legal.
        Assert.Throws<InvalidOperationException>(() => monitor.ClearValue(IsConnectedProperty));
        Assert.True(monitor.IsConnected);
    }

    [WpfFact]
    public void Clearing_The_Value_Through_The_Key_Falls_Back_To_The_Registered_Default()
    {
        var monitor = new Ex006_ConnectionMonitor();
        monitor.SetValue(IsConnectedPropertyKey, true);

        monitor.ClearValue(IsConnectedPropertyKey);

        Assert.False(monitor.IsConnected);
    }
}
