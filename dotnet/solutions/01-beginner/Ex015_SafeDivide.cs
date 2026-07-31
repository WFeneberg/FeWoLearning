namespace FeWoLearning.Exercises.Beginner;

// Exercise 015 — SafeDivide (reference solution).
public static class SafeDivide
{
    public static int? Divide(int numerator, int denominator)
    {
        try
        {
            return numerator / denominator;
        }
        catch (DivideByZeroException)
        {
            return null;
        }
    }
}
