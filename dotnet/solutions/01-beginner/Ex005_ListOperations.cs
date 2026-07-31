namespace FeWoLearning.Exercises.Beginner;

// Exercise 005 — List<T> operations (reference solution).
public static class ListOperations
{
    public static List<int> AddValue(List<int> numbers, int value)
    {
        numbers.Add(value);
        return numbers;
    }

    public static List<int> RemoveFirst(List<int> numbers, int value)
    {
        numbers.Remove(value);
        return numbers;
    }

    public static List<int> Deduplicate(List<int> numbers)
    {
        var seen = new HashSet<int>();
        var result = new List<int>();

        foreach (var n in numbers)
        {
            if (seen.Add(n))
            {
                result.Add(n);
            }
        }

        return result;
    }
}
