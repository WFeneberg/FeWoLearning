using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex024_StyleBasedOnTests : WpfTestContext
{
    [WpfFact]
    public void BuildBaseStyle_Has_Both_Of_Its_Own_Setters()
    {
        var baseStyle = Ex024_StyleBasedOn.BuildBaseStyle();

        Assert.Equal(typeof(Button), baseStyle.TargetType);
        var setters = baseStyle.Setters.Cast<Setter>().ToList();
        Assert.Equal(2, setters.Count);
        Assert.Contains(setters, s => s.Property == Button.WidthProperty && Equals(s.Value, 100.0));
        Assert.Contains(setters, s => s.Property == Button.HeightProperty && Equals(s.Value, 30.0));
    }

    [WpfFact]
    public void BuildDerivedStyle_Points_BasedOn_At_The_Base_And_Overrides_Only_Width()
    {
        var baseStyle = Ex024_StyleBasedOn.BuildBaseStyle();
        var derived = Ex024_StyleBasedOn.BuildDerivedStyle(baseStyle);

        Assert.Equal(typeof(Button), derived.TargetType);
        Assert.Same(baseStyle, derived.BasedOn);

        // Setter override order, inspected on the graph itself rather than only through
        // an effective value: the derived style redeclares Width and adds Tag, but must
        // NOT redeclare Height - that is what proves Height still comes from the BasedOn
        // chain rather than from a second, merely-equal Setter copied into derived.
        var setters = derived.Setters.Cast<Setter>().ToList();
        Assert.Equal(2, setters.Count);
        Assert.Contains(setters, s => s.Property == Button.WidthProperty && Equals(s.Value, 150.0));
        Assert.Contains(setters, s => s.Property == Button.TagProperty && Equals(s.Value, "derived"));
        Assert.DoesNotContain(setters, s => s.Property == Button.HeightProperty);
    }

    [WpfFact]
    public void The_Override_Wins_The_Inherited_Setter_Survives_And_The_New_One_Applies()
    {
        var baseStyle = Ex024_StyleBasedOn.BuildBaseStyle();
        var derived = Ex024_StyleBasedOn.BuildDerivedStyle(baseStyle);
        var button = new Button { Style = derived };

        Layout(button);

        Assert.Equal(150.0, button.Width);   // derived's own Setter overrides the base's
        Assert.Equal(30.0, button.Height);   // never redeclared - inherited through BasedOn
        Assert.Equal("derived", button.Tag); // new in the derived style

        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.HeightProperty).BaseValueSource);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.TagProperty).BaseValueSource);
    }

    [WpfFact]
    public void Applying_The_Derived_Style_Seals_The_Base_Style_Too()
    {
        var baseStyle = Ex024_StyleBasedOn.BuildBaseStyle();
        var derived = Ex024_StyleBasedOn.BuildDerivedStyle(baseStyle);

        Assert.False(baseStyle.IsSealed);
        Assert.False(derived.IsSealed);

        var button = new Button { Style = derived };

        // Sealing propagates through the BasedOn chain: the base style becomes just as
        // unmodifiable as the derived one the moment the derived style is used.
        Assert.True(derived.IsSealed);
        Assert.True(baseStyle.IsSealed);
        Assert.Throws<InvalidOperationException>(() => baseStyle.Setters.Add(new Setter(Button.TagProperty, "late")));
    }
}
