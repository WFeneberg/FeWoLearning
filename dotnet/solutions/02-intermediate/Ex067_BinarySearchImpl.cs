namespace FeWoLearning.Exercises.Intermediate;

// Exercise 067 — Binary Search (reference solution).
public static class BinarySearchImpl
{
    public static int Search(int[] sortedValues, int target)
    {
        int low = 0;
        int high = sortedValues.Length - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            int candidate = sortedValues[mid];

            if (candidate == target)
            {
                return mid;
            }

            if (candidate < target)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return -1;
    }
}
