using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 043 - ThrottledSearch (intermediate).
/// Goal:   Commit a search box's Query to CommittedQuery only after typing pauses
///         for 300ms, and never issue the same search twice in a row.
/// Drills: Throttle plus DistinctUntilChanged.
/// Passes: dotnet test --filter FullyQualifiedName~Ex043_
public class Ex043_ThrottledSearchViewModel : ReactiveObject
{
    private string _query = string.Empty;
    public string Query { get => _query; set => this.RaiseAndSetIfChanged(ref _query, value); }

    private readonly ObservableAsPropertyHelper<string> _committedQuery;
    public string CommittedQuery => _committedQuery.Value;

    public int SearchCount { get; private set; }

    public Ex043_ThrottledSearchViewModel(ISequencer scheduler)
    {
        var committed = this.WhenAnyValue(x => x.Query)
            .Throttle(TimeSpan.FromMilliseconds(300), scheduler)
            .DistinctUntilChanged();

        committed.Subscribe(_ => SearchCount++);
        _committedQuery = committed.ToProperty(this, x => x.CommittedQuery, string.Empty);
    }
}
