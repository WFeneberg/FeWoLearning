// Exercise 019 - Data Template Basics (beginner).
// Goal:   Describe how an object should look, once, and hand that description to a control.
// Drills: DataTemplate as markup (there is no code form), ContentControl.Content against
//         ContentTemplate, and XamlReader.Load as the code path to a template.
// Passes: dotnet test --filter FullyQualifiedName~Ex019_
//
// A DataTemplate is a factory for a visual tree, not a tree itself: the same template
// instance can be handed to many controls and each builds its own copy.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex019_DataTemplateBasics
{
    /// <summary>
    /// The shared template: a single TextBlock whose Text binds to the <c>Caption</c> path
    /// of whatever object it is applied to. Built with
    /// <see cref="XamlReader.Load(string)"/>, and returned as the same instance every time
    /// so callers can prove it is reused rather than rebuilt.
    /// </summary>
    public static DataTemplate CaptionTemplate =>
        // TODO: parse a DataTemplate whose content is
        //   <TextBlock Text="{Binding Caption}" />
        // Remember the default XAML namespace, or XamlReader cannot resolve the elements.
        // Cache it in a static field - this property must not build a new one per call.
        throw new NotImplementedException("TODO: Ex019 - parse and cache the caption template");

    /// <summary>
    /// A ContentControl showing <paramref name="item"/> through
    /// <see cref="CaptionTemplate"/>. No TextBlock is created here: the template makes it.
    /// </summary>
    public static ContentControl CreateCard(object item) =>
        throw new NotImplementedException("TODO: Ex019 - show the item through the template");
}
