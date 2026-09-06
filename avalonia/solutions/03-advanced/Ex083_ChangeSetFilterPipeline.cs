using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex083_
public class Ex083_ChangeSetFilterPipeline : IDisposable
{
    /// <summary>Given. Do not change.</summary>
    public ObservableCollection<int> Source { get; } = [];

    /// <summary>Given. Do not change.</summary>
    public static bool Matches(int value) => value % 2 == 0;

    public ObservableCollection<int> Filtered { get; } = [];

    private IDisposable? _subscription;

    public void Start() =>
        _subscription = Source.ToReactiveChangeSet().Subscribe(changes =>
        {
            foreach (var change in changes)
            {
                switch (change.Reason)
                {
                    case ReactiveChangeReason.Add:
                        InsertIfMatching(change.Current, change.CurrentIndex);
                        break;

                    // The removed item arrives in Current, not in Previous.
                    case ReactiveChangeReason.Remove:
                        RemoveIfMatching(change.Current);
                        break;

                    case ReactiveChangeReason.Replace:
                        RemoveIfMatching(change.Previous);
                        InsertIfMatching(change.Current, change.CurrentIndex);
                        break;

                    case ReactiveChangeReason.Move:
                        RemoveIfMatching(change.Current);
                        InsertIfMatching(change.Current, change.CurrentIndex);
                        break;
                }
            }
        });

    private void InsertIfMatching(int value, int sourceIndex)
    {
        if (Matches(value))
        {
            Filtered.Insert(MatchesBefore(sourceIndex), value);
        }
    }

    private void RemoveIfMatching(int value)
    {
        if (Matches(value))
        {
            Filtered.Remove(value);
        }
    }

    /// <summary>
    /// Where an item at <paramref name="sourceIndex"/> belongs among the matches:
    /// Source has already been mutated by the time the change set arrives, so
    /// counting the matches ahead of it gives the position directly.
    /// </summary>
    private int MatchesBefore(int sourceIndex) =>
        Source.Take(Math.Min(sourceIndex, Source.Count)).Count(Matches);

    public void Dispose() => _subscription?.Dispose();
}
