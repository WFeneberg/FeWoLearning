namespace FeWoLearning.Exercises.Intermediate;

// Exercise 068 — Merge Sort (reference solution).
public static class MergeSortImpl
{
    public static int[] Sort(int[] values)
    {
        if (values.Length <= 1)
        {
            return (int[])values.Clone();
        }

        int mid = values.Length / 2;
        var left = Sort(values[..mid]);
        var right = Sort(values[mid..]);

        return Merge(left, right);
    }

    private static int[] Merge(int[] left, int[] right)
    {
        var result = new int[left.Length + right.Length];
        int i = 0, j = 0, k = 0;

        while (i < left.Length && j < right.Length)
        {
            result[k++] = left[i] <= right[j] ? left[i++] : right[j++];
        }

        while (i < left.Length)
        {
            result[k++] = left[i++];
        }

        while (j < right.Length)
        {
            result[k++] = right[j++];
        }

        return result;
    }
}
