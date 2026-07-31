namespace FeWoLearning.Exercises.Beginner;

// Exercise 025 — ParamsSum (reference solution).
public static class ParamsSum
{
    public static int Sum(params int[] nums)
    {
        var total = 0;
        foreach (var n in nums)
        {
            total += n;
        }

        return total;
    }
}
