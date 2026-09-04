// Exercise 021 - Observable Collection Updates (beginner).
// Goal:   Change a collection and have the UI follow, without rebuilding it.
// Drills: INotifyCollectionChanged reaching an ItemsRepeater, ObservableCollection.Move as
//         one event instead of a remove-then-insert pair, and why Clear-then-Add is the
//         wrong way to replace a list.
// Passes: dotnet test --filter FullyQualifiedName~Ex021_
//
// A Reset event tells every listener "everything changed, start over": elements are thrown
// away and rebuilt, scroll position and selection go with them. Fine-grained events are
// what let the UI keep the parts that did not move.

using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using FeWoLearning.Uno.Support;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed class Ex021_ObservableCollectionUpdates
{
    private static readonly DataTemplate Template = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <TextBlock Text="{Binding}" />
        </DataTemplate>
        """);

    /// <summary>The live playlist. The same instance for the lifetime of this object.</summary>
    public ObservableCollection<string> Tracks { get; } = [];

    /// <summary>An ItemsRepeater bound to <see cref="Tracks"/>.</summary>
    public ItemsRepeater CreateView() =>
        // TODO: build the repeater over Tracks, with Template and a StackEverythingLayout.
        throw new NotImplementedException("TODO: Ex021 - build the view over Tracks");

    /// <summary>
    /// Moves <paramref name="track"/> one position towards the front, or does nothing if it
    /// is already first or not in the list. Must raise exactly one collection change.
    /// </summary>
    public void MoveUp(string track) =>
        // TODO: find the index and move it. Removing and re-inserting raises two events,
        // and the UI throws away the element in between.
        throw new NotImplementedException("TODO: Ex021 - move the track up in one step");

    /// <summary>
    /// Makes <see cref="Tracks"/> hold exactly <paramref name="tracks"/>, in that order,
    /// without ever raising a Reset: tracks that survive keep their element in the UI.
    /// </summary>
    public void ReplaceAll(IEnumerable<string> tracks) =>
        // TODO: apply the difference. Remove what is gone, then insert what is new at the
        // right index. Tracks.Clear() raises a Reset and is what this exercise is against.
        throw new NotImplementedException("TODO: Ex021 - replace the contents without a reset");
}
