using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex084_
public class Ex084_ChangeSetSortAndCount : IDisposable
{
    /// <summary>Given. Do not change.</summary>
    public ObservableCollection<int> Source { get; } = [];

    public ObservableCollection<int> Sorted { get; } = [];

    /// <summary>Given. Do not change.</summary>
    public ObservableCollection<int> ReportedCounts { get; } = [];

    private IDisposable? _sorting;
    private IDisposable? _counting;

    public void Start()
    {
        var changeSets = Source.ToReactiveChangeSet();

        _sorting = changeSets.Subscribe(changes =>
        {
            foreach (var change in changes)
            {
                switch (change.Reason)
                {
                    case ReactiveChangeReason.Add:
                        InsertSorted(change.Current);
                        break;

                    case ReactiveChangeReason.Remove:
                        Sorted.Remove(change.Current);
                        break;

                    case ReactiveChangeReason.Replace:
                        Sorted.Remove(change.Previous);
                        InsertSorted(change.Current);
                        break;

                    // A Move changes where a value sits in Source, which sorted
                    // order does not depend on. Nothing to do.
                    case ReactiveChangeReason.Move:
                        break;
                }
            }
        });

        _counting = changeSets
            .WhenCountChanged()
            .Subscribe(_ => ReportedCounts.Add(Source.Count));
    }

    private void InsertSorted(int value)
    {
        var index = Sorted.Count(existing => existing < value);
        Sorted.Insert(index, value);
    }

    public void Dispose()
    {
        _sorting?.Dispose();
        _counting?.Dispose();
    }
}
