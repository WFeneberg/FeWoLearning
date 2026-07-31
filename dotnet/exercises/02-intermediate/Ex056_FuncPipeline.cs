namespace FeWoLearning.Exercises.Intermediate;

// Exercise 056 — Func Pipeline (intermediate).
// Goal:   Compose a sequence of Func<int,int> transformations into a single
//         pipeline that applies each transformation in order to an input
//         value, returning the final result.
// Drills: delegates, Func<>, composition, LINQ Aggregate.
public static class FuncPipeline
{
    // Combine the given transformations into a single Func<int,int> that
    // applies each one in order (steps[0] first, steps[^1] last).
    // If steps is empty, the returned function must be the identity function.
    public static Func<int, int> Compose(params Func<int, int>[] steps)
        => throw new NotImplementedException();

    // Convenience method: build the pipeline from steps and immediately
    // apply it to the given input, returning the final transformed value.
    public static int Run(int input, params Func<int, int>[] steps)
        => throw new NotImplementedException();
}
