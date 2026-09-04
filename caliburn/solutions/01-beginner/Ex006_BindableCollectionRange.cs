// Exercise 006 - BindableCollection Range Operations (beginner).
// Goal:   Batch collection changes so the view re-reads the list once, not once per item -
//         and never pay for a batch that has nothing to do.
// Drills: BindableCollection<T>.AddRange/RemoveRange as single-Reset batch operations, and
//         guarding a range call so an empty batch stays completely silent.
// Passes: dotnet test --filter FullyQualifiedName~Ex006_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex006_BindableCollectionRange : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Adds every item in one batch, whatever the batch contains - even none.</summary>
    public void AddRange(IEnumerable<string> items) => Items.AddRange(items);

    /// <summary>Removes every item in one batch, whatever the batch contains - even none.</summary>
    public void RemoveRange(IEnumerable<string> items) => Items.RemoveRange(items);

    /// <summary>
    /// Same job as <see cref="AddRange"/>, except a batch with nothing in it must leave the
    /// collection completely silent - no Reset, no PropertyChanged, nothing at all.
    /// </summary>
    public void AddRangeIfAny(IEnumerable<string> items)
    {
        var materialized = items as ICollection<string> ?? items.ToList();
        if (materialized.Count == 0) return;

        Items.AddRange(materialized);
    }
}
