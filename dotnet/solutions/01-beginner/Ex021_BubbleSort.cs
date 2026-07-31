namespace FeWoLearning.Exercises.Beginner;

// Exercise 021 — Bubble Sort (reference solution).
public static class BubbleSort
{
    public static void Sort(int[] values)
    {
        for (var i = 0; i < values.Length - 1; i++)
        {
            var swapped = false;
            for (var j = 0; j < values.Length - 1 - i; j++)
            {
                if (values[j] > values[j + 1])
                {
                    (values[j], values[j + 1]) = (values[j + 1], values[j]);
                    swapped = true;
                }
            }

            if (!swapped)
            {
                break;
            }
        }
    }
}
