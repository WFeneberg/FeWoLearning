// Exercise 006 - BindableCollection Range Operations (beginner).
// Goal:   Batch collection changes so the view re-reads the list once, not once per item -
//         and never pay for a batch that has nothing to do.
// Drills: BindableCollection<T>.AddRange/RemoveRange as single-Reset batch operations, and
//         guarding a range call so an empty batch stays completely silent.
// Passes: dotnet test --filter FullyQualifiedName~Ex006_
//
// AddRange(n items) and RemoveRange(n items) each raise exactly ONE CollectionChanged, with
// action Reset - never one event per item, however many items are involved. That single
// Reset also carries PropertyChanged for Count and Item[]. The sharp bit: AddRange with an
// EMPTY sequence still raises that Reset, and so does RemoveRange asked to remove items that
// were never in the collection - a bound ItemsControl re-reads the whole list either way,
// even though nothing actually changed. A range call that might have nothing to do is the
// CALLER's job to guard against, not the collection's.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex006_BindableCollectionRange : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Adds every item in one batch, whatever the batch contains - even none.</summary>
    public void AddRange(IEnumerable<string> items) =>
        throw new NotImplementedException("TODO: Ex006 - delegate straight to Items.AddRange");

    /// <summary>Removes every item in one batch, whatever the batch contains - even none.</summary>
    public void RemoveRange(IEnumerable<string> items) =>
        throw new NotImplementedException("TODO: Ex006 - delegate straight to Items.RemoveRange");

    /// <summary>
    /// Same job as <see cref="AddRange"/>, except a batch with nothing in it must leave the
    /// collection completely silent - no Reset, no PropertyChanged, nothing at all.
    /// </summary>
    public void AddRangeIfAny(IEnumerable<string> items) =>
        throw new NotImplementedException("TODO: Ex006 - materialize items and skip the call entirely when there are none");
}
