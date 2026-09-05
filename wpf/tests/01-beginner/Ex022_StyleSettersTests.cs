using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex022_StyleSettersTests : WpfTestContext
{
    [WpfFact]
    public void BuildStyle_Produces_A_Button_Style_With_Both_Setters()
    {
        var style = Ex022_StyleSetters.BuildStyle(120.0, "wide");

        // Structural: inspect the style graph directly, not only an effective value -
        // this is what proves both Setters exist, independent of ever applying the style.
        Assert.Equal(typeof(Button), style.TargetType);
        var setters = style.Setters.Cast<Setter>().ToList();
        Assert.Equal(2, setters.Count);
        Assert.Contains(setters, s => s.Property == Button.WidthProperty && Equals(s.Value, 120.0));
        Assert.Contains(setters, s => s.Property == Button.TagProperty && Equals(s.Value, "wide"));
    }

    [WpfFact]
    public void Apply_Assigns_The_Style_And_Both_Setters_Take_Effect()
    {
        // Different width/tag than the structural test above: no single hard-coded
        // literal in Apply could satisfy both this and BuildStyle_Produces_... at once.
        var style = Ex022_StyleSetters.BuildStyle(200.0, "tall");
        var button = new Button();

        Ex022_StyleSetters.Apply(button, style);
        Layout(button);

        Assert.Same(style, button.Style);
        Assert.Equal(200.0, button.Width);
        Assert.Equal("tall", button.Tag);

        // Mechanism, not just effective value: both properties must actually have come
        // from the Style, not from a direct SetValue/local assignment that happens to
        // agree with it.
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.TagProperty).BaseValueSource);
    }

    [WpfFact]
    public void Applying_The_Style_Seals_It_Immediately()
    {
        var style = Ex022_StyleSetters.BuildStyle(120.0, "wide");
        var button = new Button();

        Assert.False(style.IsSealed);

        Ex022_StyleSetters.Apply(button, style);

        // Sealed as soon as it is used - no Layout(...) needed to observe it.
        Assert.True(style.IsSealed);
    }

    [WpfFact]
    public void A_Sealed_Style_Rejects_Further_Setters()
    {
        var style = Ex022_StyleSetters.BuildStyle(120.0, "wide");
        var button = new Button();
        Ex022_StyleSetters.Apply(button, style);

        Assert.Throws<InvalidOperationException>(() => style.Setters.Add(new Setter(Button.HeightProperty, 30.0)));
    }
}
