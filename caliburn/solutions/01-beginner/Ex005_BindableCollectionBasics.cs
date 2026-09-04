// Exercise 005 - BindableCollection Basics (beginner).
// Goal:   Rebuild a whole list without making the UI redraw once per item.
// Drills: BindableCollection<T>, IsNotifying as a notification suspension switch, and
//         Refresh() as the single Reset that tells the view "start over".
// Passes: dotnet test --filter FullyQualifiedName~Ex005_

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex005_BindableCollectionBasics : PropertyChangedBase
{
    /// <summary>The bound collection. Created once - never reassign it, or bindings break.</summary>
    public BindableCollection<string> Items { get; } = new();

    /// <summary>Appends one item, announcing it the ordinary way.</summary>
    public void AddItem(string item) => Items.Add(item);

    /// <summary>
    /// Replaces the entire contents, costing the view exactly ONE notification no matter
    /// how many items are involved.
    /// </summary>
    public void ReplaceAll(IEnumerable<string> items)
    {
        Items.IsNotifying = false;
        try
        {
            Items.Clear();
            foreach (var item in items) Items.Add(item);
        }
        finally
        {
            // try/finally because a half-suspended collection is silent forever, and the
            // symptom shows up in whatever binds to it rather than here.
            Items.IsNotifying = true;
        }

        Items.Refresh();
    }
}
