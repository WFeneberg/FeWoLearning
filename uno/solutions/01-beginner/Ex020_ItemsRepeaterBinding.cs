// Exercise 020 - Items Repeater Binding (beginner).
// Goal:   Render one element per item of a collection.
// Drills: ItemsRepeater.ItemsSource and ItemTemplate, {Binding} against the item itself,
//         and ItemsSourceView as the repeater's view of any collection you hand it.
// Passes: dotnet test --filter FullyQualifiedName~Ex020_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Exercises.Beginner;

public static class Ex020_ItemsRepeaterBinding
{
    private static readonly DataTemplate Template = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <TextBlock Text="{Binding}" />
        </DataTemplate>
        """);

    /// <summary>
    /// The shared item template: a TextBlock bound to the item itself with <c>{Binding}</c>
    /// - an empty path, meaning "the whole data context". Parsed once and cached.
    /// </summary>
    public static DataTemplate ItemTemplate => Template;

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/>, one templated element
    /// per item, stacked vertically by
    /// <see cref="FeWoLearning.Uno.Support.StackEverythingLayout"/>.
    /// </summary>
    public static ItemsRepeater CreateList(object items) => new()
    {
        // Anything enumerable goes in; the repeater wraps it in an ItemsSourceView, which
        // is also where it picks up INotifyCollectionChanged if the collection has it.
        ItemsSource = items,
        ItemTemplate = ItemTemplate,

        // A repeater has no layout of its own - no Layout means no elements. The default
        // when created from markup is a virtualising StackLayout; this one realises
        // everything, which is what a windowless test tree needs.
        Layout = new StackEverythingLayout(),
    };
}
