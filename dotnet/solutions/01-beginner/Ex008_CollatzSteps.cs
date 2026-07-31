namespace FeWoLearning.Exercises.Beginner;

// Exercise 008 — Collatz Steps (reference solution).
public static class CollatzSteps
{
    public static int Count(int n)
    {
        long value = n;
        int steps = 0;

        while (value != 1)
        {
            value = value % 2 == 0 ? value / 2 : (3 * value) + 1;
            steps++;
        }

        return steps;
    }
}
