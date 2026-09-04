// Exercise 021 - Observable Collection Updates (beginner).
// Goal:   Change a collection and have the UI follow, without rebuilding it.
// Drills: INotifyCollectionChanged reaching an ItemsRepeater, ObservableCollection.Move as
//         one event instead of a remove-then-insert pair, and why Clear-then-Add is the
//         wrong way to replace a list.
// Passes: dotnet test --filter FullyQualifiedName~Ex021_

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
    public ItemsRepeater CreateView() => new()
    {
        // The repeater subscribes to CollectionChanged through its ItemsSourceView. Handing
        // it Tracks.ToList() instead would render once and then never move again.
        ItemsSource = Tracks,
        ItemTemplate = Template,
        Layout = new StackEverythingLayout(),
    };

    /// <summary>
    /// Moves <paramref name="track"/> one position towards the front, or does nothing if it
    /// is already first or not in the list. Must raise exactly one collection change.
    /// </summary>
    public void MoveUp(string track)
    {
        var index = Tracks.IndexOf(track);

        if (index <= 0)
        {
            // Not found, or already first. Either way there is nothing to announce.
            return;
        }

        // One Move event. RemoveAt + Insert would be two, and between them the UI would
        // discard the element for this track and build a fresh one.
        Tracks.Move(index, index - 1);
    }

    /// <summary>
    /// Makes <see cref="Tracks"/> hold exactly <paramref name="tracks"/>, in that order,
    /// without ever raising a Reset: tracks that survive keep their element in the UI.
    /// </summary>
    /// <remarks>Assumes the tracks are distinct - a playlist of names, not a multiset.</remarks>
    public void ReplaceAll(IEnumerable<string> tracks)
    {
        var target = tracks.ToList();

        // Backwards, so removing does not shift the indices still to be visited.
        for (var i = Tracks.Count - 1; i >= 0; i--)
        {
            if (!target.Contains(Tracks[i]))
            {
                Tracks.RemoveAt(i);
            }
        }

        for (var i = 0; i < target.Count; i++)
        {
            var current = Tracks.IndexOf(target[i]);

            if (current < 0)
            {
                Tracks.Insert(i, target[i]);
            }
            else if (current != i)
            {
                Tracks.Move(current, i);
            }
        }

        // Nothing above touches a track that is already in the right place, so replacing a
        // list with itself raises no events at all - and a Reset, which Clear() would have
        // raised, never happens.
    }
}
