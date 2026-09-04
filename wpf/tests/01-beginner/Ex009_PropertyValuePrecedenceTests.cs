using System.Windows;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex009_PropertyValuePrecedenceTests : WpfTestContext
{
    private static DependencyProperty ToneProperty
        => DependencyPropertyReflection.Property(typeof(Ex009_Badge), "ToneProperty");

    private static Style StyleSettingToneTo(string value)
        => new(typeof(Ex009_Badge)) { Setters = { new Setter(ToneProperty, value) } };

    [WpfFact]
    public void With_Nothing_Set_The_Value_Comes_From_The_Registered_Default()
    {
        var badge = new Ex009_Badge();

        Assert.Equal("Neutral", badge.GetValue(ToneProperty));
        Assert.Equal(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource);
    }

    [WpfFact]
    public void A_Style_Setter_Outranks_The_Default()
    {
        var badge = new Ex009_Badge { Style = StyleSettingToneTo("FromStyle") };

        Layout(badge);

        Assert.Equal("FromStyle", badge.GetValue(ToneProperty));
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource);
    }

    [WpfFact]
    public void A_Local_Value_Outranks_The_Style_Setter()
    {
        var badge = new Ex009_Badge { Style = StyleSettingToneTo("FromStyle") };
        Layout(badge);

        // Written through SetValue directly, not through the CLR wrapper: precedence is a
        // property-system concept and has to hold regardless of which handle wrote it.
        badge.SetValue(ToneProperty, "Local");

        Assert.Equal("Local", badge.GetValue(ToneProperty));
        Assert.Equal(BaseValueSource.Local, DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource);
    }

    [WpfFact]
    public void Clearing_The_Local_Value_Falls_Back_To_The_Style_Not_The_Default()
    {
        var badge = new Ex009_Badge { Style = StyleSettingToneTo("FromStyle") };
        Layout(badge);
        badge.SetValue(ToneProperty, "Local");

        badge.ClearValue(ToneProperty);

        Assert.Equal("FromStyle", badge.GetValue(ToneProperty));
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource);
    }

    [WpfFact]
    public void Removing_The_Style_Falls_All_The_Way_Back_To_The_Default()
    {
        var badge = new Ex009_Badge { Style = StyleSettingToneTo("FromStyle") };
        Layout(badge);

        badge.Style = null;
        Layout(badge);

        Assert.Equal("Neutral", badge.GetValue(ToneProperty));
        Assert.Equal(BaseValueSource.Default, DependencyPropertyHelper.GetValueSource(badge, ToneProperty).BaseValueSource);
    }

    [WpfFact]
    public void The_Clr_Wrapper_And_SetValue_Reach_The_Same_Storage()
    {
        var badge = new Ex009_Badge();

        // Neither precedence test above ever calls the Tone property itself - this is
        // what proves Tone is not a shadow field quietly tracking GetValue/SetValue from
        // the side without actually being backed by them.
        badge.Tone = "ViaClr";
        Assert.Equal("ViaClr", badge.GetValue(ToneProperty));

        badge.SetValue(ToneProperty, "ViaSetValue");
        Assert.Equal("ViaSetValue", badge.Tone);
    }
}
