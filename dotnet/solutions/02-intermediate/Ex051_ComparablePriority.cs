namespace FeWoLearning.Exercises.Intermediate;

// Exercise 051 — Comparable Priority (reference solution).
public class WorkItem : IComparable<WorkItem>
{
    public string Title { get; }
    public int Priority { get; }

    public WorkItem(string title, int priority)
    {
        Title = title;
        Priority = priority;
    }

    public int CompareTo(WorkItem? other)
    {
        if (other is null)
        {
            return 1;
        }

        int priorityComparison = Priority.CompareTo(other.Priority);
        return priorityComparison != 0
            ? priorityComparison
            : string.CompareOrdinal(Title, other.Title);
    }

    public override string ToString() => $"{Title} (P{Priority})";
}
