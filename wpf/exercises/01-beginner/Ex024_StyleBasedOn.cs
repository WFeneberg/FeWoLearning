// Exercise 024 - BasedOn style inheritance (beginner).
// Goal:   Derive one Style from another with BasedOn, overriding one Setter while leaving
//         another to the base style entirely - and see the two seal together the moment
//         the derived style is actually used.
// Drills: BasedOn (a Style pointing at another Style as its parent) and setter override
//         order: a Setter for the same property in the derived style wins over the base
//         style's, while a property the derived style never touches still resolves through
//         the BasedOn chain to the base style's Setter.
// Passes: dotnet test --filter FullyQualifiedName~Ex024_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex024_StyleBasedOn
{
    /// <summary>Builds the base Style: TargetType typeof(Button), Setters for Width = 100.0
    /// and Height = 30.0.</summary>
    public static Style BuildBaseStyle()
        // TODO: var style = new Style(typeof(Button));
        //       style.Setters.Add(new Setter(Button.WidthProperty, 100.0));
        //       style.Setters.Add(new Setter(Button.HeightProperty, 30.0));
        //       return style;
        => throw new NotImplementedException("TODO: Ex024 - build a Style with TargetType typeof(Button) and two Setters: Button.WidthProperty = 100.0, Button.HeightProperty = 30.0");

    /// <summary>
    /// Builds a derived Style: TargetType typeof(Button), BasedOn <paramref name="baseStyle"/>,
    /// overriding Width to 150.0 - Height is deliberately left out, so it must still come
    /// from <paramref name="baseStyle"/> through the BasedOn chain - and adding a new
    /// Setter for Tag = "derived".
    /// </summary>
    public static Style BuildDerivedStyle(Style baseStyle)
        // TODO: var style = new Style(typeof(Button)) { BasedOn = baseStyle };
        //       style.Setters.Add(new Setter(Button.WidthProperty, 150.0));
        //       style.Setters.Add(new Setter(Button.TagProperty, "derived"));
        //       return style;
        // Do NOT add a Setter for Height here - that would defeat the point of this row.
        => throw new NotImplementedException("TODO: Ex024 - build a Style with TargetType typeof(Button), BasedOn = baseStyle, that overrides Button.WidthProperty to 150.0 (do not redeclare Height - it must still come from baseStyle) and adds a new Setter for Button.TagProperty = \"derived\"");
}
