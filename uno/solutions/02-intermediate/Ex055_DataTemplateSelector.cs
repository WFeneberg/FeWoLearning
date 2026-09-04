// Exercise 055 - Data Template Selector (intermediate).
// Goal:   Pick a different look per item, from code, while the tree is being built.
// Drills: DataTemplateSelector.SelectTemplateCore, ContentControl.ContentTemplateSelector,
//         and what returning null means.
// Passes: dotnet test --filter FullyQualifiedName~Ex055_
//
// The alternative is one template with everything in it and a pile of visibility
// converters. A selector keeps each case a separate, readable template - and because it is
// called per item, the choice can depend on the data rather than on a property of the host.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Chooses between a "long" and a "short" template depending on the item's text length.
/// </summary>
public sealed partial class Ex055_DataTemplateSelector : DataTemplateSelector
{
    /// <summary>Test fixture: renders the item into a TextBlock named "Long".</summary>
    public static readonly DataTemplate LongTemplate = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <TextBlock x:Name="Long" Text="{Binding}" />
        </DataTemplate>
        """);

    /// <summary>Test fixture: renders the item into a TextBlock named "Short".</summary>
    public static readonly DataTemplate ShortTemplate = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <TextBlock x:Name="Short" Text="{Binding}" />
        </DataTemplate>
        """);

    /// <summary>Strings of this length or more get <see cref="LongTemplate"/>.</summary>
    public int Threshold { get; set; } = 5;

    /// <summary>
    /// Returns <see cref="LongTemplate"/> for a string at or over <see cref="Threshold"/>,
    /// <see cref="ShortTemplate"/> for a shorter one, and null for anything that is not a
    /// string - null means "no opinion", and the host falls back to its ContentTemplate.
    /// </summary>
    protected override DataTemplate? SelectTemplateCore(object item) => item switch
    {
        string text => text.Length >= Threshold ? LongTemplate : ShortTemplate,

        // null is "no opinion", not "show nothing": the host falls back to its own
        // ContentTemplate. Returning a template here for data this selector does not
        // understand is how a selector ends up owning cases that are not its business.
        _ => null,
    };

    /// <summary>
    /// A ContentControl showing <paramref name="item"/> through this selector, with
    /// <paramref name="fallback"/> as the ContentTemplate for when the selector abstains.
    /// </summary>
    public static ContentControl CreateCard(object item, DataTemplateSelector selector, DataTemplate? fallback = null) => new()
    {
        Content = item,

        // The selector is consulted per item, and re-consulted whenever Content changes -
        // which is the difference to choosing a template once when the host is built.
        ContentTemplateSelector = selector,
        ContentTemplate = fallback,
    };
}
