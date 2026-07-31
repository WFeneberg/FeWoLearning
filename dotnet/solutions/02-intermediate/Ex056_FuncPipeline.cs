namespace FeWoLearning.Exercises.Intermediate;

// Exercise 056 — Func Pipeline (reference solution).
public static class FuncPipeline
{
    public static Func<int, int> Compose(params Func<int, int>[] steps)
        => input => steps.Aggregate(input, (current, step) => step(current));

    public static int Run(int input, params Func<int, int>[] steps)
        => Compose(steps)(input);
}
