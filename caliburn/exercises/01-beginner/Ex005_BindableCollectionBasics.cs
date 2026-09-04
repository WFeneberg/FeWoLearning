// Exercise 005 - BindableCollection Basics (beginner).
// Goal:   Rebuild a whole list without making the UI redraw once per item.
// Drills: BindableCollection<T>, IsNotifying as a notification suspension switch, and
//         Refresh() as the single Reset that tells the view "start over".
// Passes: dotnet test --filter FullyQualifiedName~Ex005_
//
// A bound ItemsControl reacts to every CollectionChanged event. Clearing a list of 500 and
// re-adding 500 naively is 501 CollectionChanged events and 501 rounds of container
// generation. Suspending notification and raising one Reset at the end is one.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex005_BindableCollectionBasics : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Appends one item, announcing it the ordinary way.</summary>
    public void AddItem(string item) =>
        throw new NotImplementedException("TODO: Ex005 - append the item to Items");

    /// <summary>
    /// Replaces the entire contents, costing the view exactly ONE notification no matter
    /// how many items are involved.
    /// </summary>
    public void ReplaceAll(IEnumerable<string> items) =>
        throw new NotImplementedException("TODO: Ex005 - swap the contents in a single notification");

    // TODO for ReplaceAll: switch Items.IsNotifying off, clear, add the new items one at a
    // time, switch it back on, then call Items.Refresh() to raise the single Reset.
    // Restore IsNotifying even on the way out of an exception - items is a lazy sequence and
    // can throw mid-enumeration, and a half-suspended collection is silent forever.
}
