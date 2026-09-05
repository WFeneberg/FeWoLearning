using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 070 - ObservableCollectionSync (intermediate).
/// Goal:   Bring a bound ObservableCollection into line with a fresh source list
///         by DIFFING it - individual Insert/Remove/Move calls - instead of
///         clearing and refilling. The collection instance a view is bound to must
///         survive, and an unchanged sync must raise nothing at all.
/// Drills: ObservableCollection mutation, CollectionChanged semantics, why Clear
///         is not a free operation behind a bound ItemsSource.
/// Passes: dotnet test --filter FullyQualifiedName~Ex070_
///
/// Clear() plus a refill produces the correct final contents and is the answer
/// this exercise exists to reject. It raises a RESET, which tells every bound
/// control only that everything it knew is gone: selection, scroll position and
/// item containers are all discarded. The test therefore records every
/// CollectionChanged event and fails on a single Reset, fails if an identical
/// sync raises anything, and pins the exact event for a one-item change.
///
/// A workable algorithm, which the tests are written against:
///   1. walk backwards through Target and RemoveAt anything the source no longer
///      holds - backwards, so the indices ahead of you stay valid;
///   2. walk forwards over source by index i: if Target[i] is already the right
///      item, move on; if the item exists further along, Move it to i; otherwise
///      Insert it at i.
public class Ex070_ObservableCollectionSync
{
    /// <summary>
    /// Given. Do not change. The one instance a view binds to: SyncTo must never
    /// replace it.
    /// </summary>
    public ObservableCollection<string> Target { get; } = [];

    public void SyncTo(IReadOnlyList<string> source) =>
        throw new NotImplementedException(
            "TODO: Ex070 - make Target equal source using RemoveAt, Move and Insert " +
            "on individual items. Do not call Clear, do not assign a new collection, " +
            "and leave items that are already in the right place untouched");
}
