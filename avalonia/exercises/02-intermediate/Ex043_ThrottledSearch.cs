using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 043 - ThrottledSearch (intermediate).
/// Goal:   Commit a search box's Query to CommittedQuery only after typing pauses
///         for 300ms, and never issue the same search twice in a row.
/// Drills: Throttle plus DistinctUntilChanged.
///
/// Measured on this machine against ReactiveUI 24.1.0/Primitives 7.1.0, with a
/// ReactiveUI.Primitives.Concurrency.VirtualClock:
///   this.WhenAnyValue(x => x.Query)
///       .Throttle(TimeSpan.FromMilliseconds(300), scheduler)
///       .DistinctUntilChanged()
///       .ToProperty(this, x => x.CommittedQuery, string.Empty);
/// Three rapid Query writes produced NOTHING; AdvanceBy(299ms) produced NOTHING;
/// a further AdvanceBy(2ms) produced exactly one emission - the LAST value. Fully
/// deterministic, no wall-clock at all - the injected ISequencer is what makes a
/// test able to control this without a real 300ms wait.
///
/// SearchCount matters here for a reason that is easy to miss: ObservableAsPropertyHelper
/// already suppresses a CONSECUTIVE re-assignment of the same value on its own (the same
/// "assigning a value it already holds is a no-op" rule that governs a plain property),
/// so a Throttle-without-DistinctUntilChanged mistake can still look correct if you only
/// ever watch CommittedQuery - the OAPH's own plumbing quietly absorbs the duplicate
/// before it becomes a second PropertyChanged. SearchCount is a second, independent
/// subscriber to the SAME pipeline with no such built-in suppression, standing in for
/// "an actual search request was issued" - it is what actually proves DistinctUntilChanged
/// is doing anything.
/// Passes: dotnet test --filter FullyQualifiedName~Ex043_
public class Ex043_ThrottledSearchViewModel : ReactiveObject
{
    private string _query = string.Empty;
    public string Query { get => _query; set => this.RaiseAndSetIfChanged(ref _query, value); }

    private readonly ObservableAsPropertyHelper<string> _committedQuery = null!;
    public string CommittedQuery => _committedQuery.Value;

    /// <summary>How many times a search would actually have been issued.</summary>
    public int SearchCount { get; private set; }

    /// <summary>
    /// TODO:
    ///   var committed = this.WhenAnyValue(x => x.Query)
    ///       .Throttle(TimeSpan.FromMilliseconds(300), scheduler)
    ///       .DistinctUntilChanged();
    ///   committed.Subscribe(_ => SearchCount++);
    ///   _committedQuery = committed.ToProperty(this, x => x.CommittedQuery, string.Empty);
    /// Use the INJECTED scheduler for Throttle, not Sequencer.Default/CurrentThread -
    /// a test driving this with a VirtualClock needs its own scheduler to actually be
    /// the one Throttle schedules against. Subscribe SearchCount to the SAME
    /// Throttle+DistinctUntilChanged observable that feeds CommittedQuery, not to
    /// CommittedQuery itself (see the SearchCount remark above for why).
    /// </summary>
    public Ex043_ThrottledSearchViewModel(ISequencer scheduler)
    {
        throw new NotImplementedException(
            "TODO: Ex043 - back CommittedQuery with WhenAnyValue(x => x.Query)." +
            "Throttle(300ms, scheduler).DistinctUntilChanged().ToProperty(...), and " +
            "increment SearchCount from a second Subscribe on that same observable");
    }
}
