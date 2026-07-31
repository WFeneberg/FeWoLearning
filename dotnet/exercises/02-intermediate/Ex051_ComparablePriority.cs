namespace FeWoLearning.Exercises.Intermediate;

// Exercise 051 — Comparable Priority (intermediate).
// Goal:   Implement a WorkItem class that implements IComparable<WorkItem>,
//         ordering items by their Priority (lower priority number sorts first).
//         When priorities are equal, order should fall back to comparing Title
//         (ordinal, case-sensitive) so ordering is fully deterministic.
// Drills: IComparable<T>, custom ordering, List<T>.Sort, tie-breaking logic.
public class WorkItem : IComparable<WorkItem>
{
    public string Title { get; }
    public int Priority { get; }

    public WorkItem(string title, int priority)
    {
        Title = title;
        Priority = priority;
    }

    public int CompareTo(WorkItem? other) => throw new NotImplementedException();

    public override string ToString() => $"{Title} (P{Priority})";
}
