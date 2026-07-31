namespace FeWoLearning.Exercises.Beginner;

// Exercise 029 — MathUtilsStatic (reference solution).
public static class MathUtilsStatic
{
    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
