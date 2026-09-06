using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 083 - ChangeSetFilterPipeline (advanced).
/// Goal:   Keep a filtered mirror of a collection up to date from its CHANGES
///         rather than by re-scanning it: subscribe to a change-set stream and
///         apply each change to the mirror yourself.
/// Drills: ChangeSetExtensions.ToReactiveChangeSet, IReactiveChangeSet of T,
///         ReactiveChange with its Reason, Current, Previous and indexes,
///         ReactiveChangeReason.
/// Passes: dotnet test --filter FullyQualifiedName~Ex083_
///
/// THIS IS NOT DynamicData. ReactiveUI 24 ships its own change sets in
/// ReactiveUI.Core - ToReactiveChangeSet, IReactiveChangeSet of T, ReactiveChange
/// of T - and this track references no DynamicData at all. The API is much
/// smaller too: there is no Filter or Transform operator to lean on, so applying
/// the changes IS the exercise.
///
/// Measured behaviour of the stream, all of which the test relies on:
///   - subscribing emits ONE change set describing the collection as it already
///     stands, with every existing item as an Add;
///   - Add gives Reason Add with the new item in Current and its position in
///     CurrentIndex;
///   - Remove gives Reason Remove with the REMOVED item in Current - not in
///     Previous - and its old position in CurrentIndex;
///   - assigning source[i] gives Reason Replace, with the old value in Previous
///     and the new one in Current;
///   - Move gives Reason Move with PreviousIndex and CurrentIndex;
///   - Clear() is expanded into one Remove PER ITEM, not a single reset.
/// That last one is why a naive handler that only understands Add and Remove
/// happens to survive a Clear, and why the Replace case is where it breaks.
public class Ex083_ChangeSetFilterPipeline : IDisposable
{
    /// <summary>
    /// Given. Do not change. The collection being mirrored. Its values are
    /// DISTINCT - the test never puts the same number in twice - so you may locate
    /// an item in Filtered by value rather than having to track identity.
    /// </summary>
    public ObservableCollection<int> Source { get; } = [];

    /// <summary>Given. Do not change. Only values passing this belong in Filtered.</summary>
    public static bool Matches(int value) => value % 2 == 0;

    /// <summary>
    /// The mirror: every Source item that Matches, in Source's own relative order,
    /// and nothing else. The SAME instance for this object's whole life, because a
    /// view binds to it once.
    /// </summary>
    public ObservableCollection<int> Filtered { get; } = [];

    /// <summary>
    /// Subscribe to Source's change sets and keep Filtered in step. Called from
    /// Start, which the test calls after seeding Source - so the very first change
    /// set already describes several items.
    ///
    /// Handle all four reasons the test exercises: Add, Remove, Replace and Move.
    /// A Replace can turn a matching value into a non-matching one and the other
    /// way round, so it is a removal, an insertion, or neither, depending on the
    /// two values.
    ///
    /// Keeping the relative order is what makes this more than a set: work out
    /// where a new match belongs among the matches already in Filtered, rather
    /// than appending it.
    /// </summary>
    public void Start() =>
        throw new NotImplementedException(
            "TODO: Ex083 - Source.ToReactiveChangeSet().Subscribe(...), applying " +
            "each ReactiveChange to Filtered per its Reason, and keep the " +
            "subscription in _subscription so Dispose can end it");

    private IDisposable? _subscription;

    public void Dispose() => _subscription?.Dispose();
}
