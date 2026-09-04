// Exercise 020 - Items Repeater Binding (beginner).
// Goal:   Render one element per item of a collection.
// Drills: ItemsRepeater.ItemsSource and ItemTemplate, {Binding} against the item itself,
//         and ItemsSourceView as the repeater's view of any collection you hand it.
// Passes: dotnet test --filter FullyQualifiedName~Ex020_
//
// ItemsRepeater is a layout panel for data, not a control: no scrolling, no selection, no
// default styling. That is also why it is the collection host these exercises use - an
// ItemsControl or ListView needs a live visual tree before it builds a single item.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex020_ItemsRepeaterBinding
{
    /// <summary>
    /// The shared item template: a TextBlock bound to the item itself with <c>{Binding}</c>
    /// - an empty path, meaning "the whole data context". Parsed once and cached.
    /// </summary>
    public static DataTemplate ItemTemplate =>
        // TODO: parse a DataTemplate whose content is <TextBlock Text="{Binding}" /> and
        // cache it in a static field.
        throw new NotImplementedException("TODO: Ex020 - parse and cache the item template");

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/>, one templated element
    /// per item, stacked vertically by
    /// <see cref="FeWoLearning.Uno.Support.StackEverythingLayout"/>.
    /// </summary>
    public static ItemsRepeater CreateList(object items) =>
        // TODO: create the repeater, set ItemsSource, ItemTemplate and Layout.
        throw new NotImplementedException("TODO: Ex020 - build the repeater over the items");
}
