// Exercise 019 - Data Template Basics (beginner).
// Goal:   Describe how an object should look, once, and hand that description to a control.
// Drills: DataTemplate as markup (there is no code form), ContentControl.Content against
//         ContentTemplate, and XamlReader.Load as the code path to a template.
// Passes: dotnet test --filter FullyQualifiedName~Ex019_

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex019_DataTemplateBasics
{
    // Parsed once, at first use. A DataTemplate has no public constructor and no builder
    // API - markup is the only way to describe one, so code that needs one at runtime goes
    // through XamlReader.
    private static readonly DataTemplate Template = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <TextBlock Text="{Binding Caption}" />
        </DataTemplate>
        """);

    /// <summary>
    /// The shared template: a single TextBlock whose Text binds to the <c>Caption</c> path
    /// of whatever object it is applied to. Built with
    /// <see cref="XamlReader.Load(string)"/>, and returned as the same instance every time
    /// so callers can prove it is reused rather than rebuilt.
    /// </summary>
    public static DataTemplate CaptionTemplate => Template;

    /// <summary>
    /// A ContentControl showing <paramref name="item"/> through
    /// <see cref="CaptionTemplate"/>. No TextBlock is created here: the template makes it.
    /// </summary>
    public static ContentControl CreateCard(object item) => new()
    {
        Content = item,

        // Content is the data, ContentTemplate is the instruction for drawing it. Sharing
        // one template across controls is safe: each control asks it for its own tree, and
        // the item becomes that tree's DataContext, which is what makes {Binding Caption}
        // inside the template resolve per card.
        ContentTemplate = CaptionTemplate,
    };
}
