namespace FeWoLearning.Exercises.Beginner;

// Exercise 001 — FizzBuzz (reference solution).
public static class FizzBuzz
{
    public static string Evaluate(int n) => (n % 3, n % 5) switch
    {
        (0, 0) => "FizzBuzz",
        (0, _) => "Fizz",
        (_, 0) => "Buzz",
        _ => n.ToString(),
    };
}
