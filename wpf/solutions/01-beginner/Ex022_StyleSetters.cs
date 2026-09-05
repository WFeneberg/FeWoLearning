// Exercise 022 - Style setters, applied in code (beginner). REFERENCE SOLUTION.
// Goal:   Build a Style with more than one Setter and apply it to an element entirely in
//         code - there is no markup in this tier, so this is the code equivalent of an
//         inline <Style> block with Setter children.
// Drills: Style (TargetType-scoped, a bag of Setters), Setter (one property/value pair
//         each), and applying a style in code by assigning FrameworkElement.Style. Also:
//         a Style seals the moment it is actually used - assigning it to an element's
//         Style property seals it immediately, before any layout pass - so it must be
//         fully built (all Setters added) first; adding a Setter afterwards throws.

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex022_StyleSetters
{
    /// <summary>
    /// Builds a Style targeting typeof(Button) with two Setters: Width = <paramref name="width"/>
    /// and Tag = <paramref name="tag"/>.
    /// </summary>
    public static Style BuildStyle(double width, string tag)
    {
        var style = new Style(typeof(Button));
        style.Setters.Add(new Setter(Button.WidthProperty, width));
        style.Setters.Add(new Setter(Button.TagProperty, tag));
        return style;
    }

    /// <summary>
    /// Applies <paramref name="style"/> to <paramref name="button"/> in code - the
    /// equivalent of Button.Style="{StaticResource ...}" or an inline &lt;Style&gt; in XAML,
    /// with no markup at all.
    /// </summary>
    public static void Apply(Button button, Style style)
    {
        button.Style = style;
    }
}
