using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 084 - ChangeSetSortAndCount (advanced).
/// Goal:   Two projections off the same change-set stream that ex083 introduced: a
///         SORTED mirror, which unlike ex083's filtered one does not follow the
///         source's order at all, and a COUNT that only reports when the number of
///         items actually changed.
/// Drills: ChangeSetMixins.WhenCountChanged and CountHasChanged, sorted insertion
///         from a change set, telling a size change apart from a content change.
/// Passes: dotnet test --filter FullyQualifiedName~Ex084_
///
/// The count is the subtle half. Replacing source[0] changes the collection's
/// CONTENTS but not its SIZE, and a naive "raise something whenever anything
/// happens" count observable fires anyway - which makes every downstream consumer
/// recompute for nothing. That is exactly what CountHasChanged is for, and
/// WhenCountChanged is the operator built on it.
///
/// Measured, and what the test pins down: subscribing emits once for the state the
/// collection is already in; an Add and a Remove each emit; a Replace does NOT.
/// Note that IReactiveChangeSet's own Count is the number of CHANGES in that set,
/// not the size of the collection - so the size has to come from the collection.
public class Ex084_ChangeSetSortAndCount : IDisposable
{
    /// <summary>Given. Do not change. Values are distinct.</summary>
    public ObservableCollection<int> Source { get; } = [];

    /// <summary>
    /// Every Source value in ASCENDING order, whatever order they arrived in. The
    /// same instance for this object's whole life.
    /// </summary>
    public ObservableCollection<int> Sorted { get; } = [];

    /// <summary>
    /// Given. Do not change. One entry per report of the collection's size, in
    /// order - so a run that reports the same size twice is visible here.
    /// </summary>
    public ObservableCollection<int> ReportedCounts { get; } = [];

    /// <summary>
    /// Wire both projections. Called from Start, which the test calls after seeding
    /// Source.
    ///
    /// For Sorted: apply each change so the result stays ascending. An Add inserts
    /// at the right place, a Remove takes its value out, a Replace does both, and a
    /// Move changes nothing at all - the sorted order does not care where in Source
    /// a value sits, which is the difference from ex083.
    ///
    /// For ReportedCounts: append Source.Count, but ONLY for change sets that
    /// actually changed the size. Use the change-set stream's own count operator
    /// rather than comparing sizes by hand.
    /// </summary>
    public void Start() =>
        throw new NotImplementedException(
            "TODO: Ex084 - two subscriptions on Source.ToReactiveChangeSet(): one " +
            "maintaining Sorted per change, one going through WhenCountChanged and " +
            "appending Source.Count to ReportedCounts. Hold on to both so Dispose can " +
            "end them");

    private IDisposable? _subscriptions;

    public void Dispose() => _subscriptions?.Dispose();
}
