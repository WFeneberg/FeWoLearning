namespace FeWoLearning.Exercises.Beginner;

// Exercise 007 — Sum of Digits (reference solution).
public static class SumOfDigits
{
    public static int Compute(int n)
    {
        int remaining = Math.Abs(n);
        int sum = 0;

        while (remaining > 0)
        {
            sum += remaining % 10;
            remaining /= 10;
        }

        return sum;
    }
}
