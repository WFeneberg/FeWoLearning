namespace FeWoLearning.Exercises.Beginner;

// Exercise 004 — Array Statistics (reference solution).
public static class ArrayStatistics
{
    public static int Min(int[] values)
    {
        var min = values[0];
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] < min)
            {
                min = values[i];
            }
        }

        return min;
    }

    public static int Max(int[] values)
    {
        var max = values[0];
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] > max)
            {
                max = values[i];
            }
        }

        return max;
    }

    public static int Sum(int[] values)
    {
        var sum = 0;
        for (var i = 0; i < values.Length; i++)
        {
            sum += values[i];
        }

        return sum;
    }
}
