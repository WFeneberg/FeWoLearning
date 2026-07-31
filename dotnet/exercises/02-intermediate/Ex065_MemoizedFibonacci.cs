namespace FeWoLearning.Exercises.Intermediate;

// Exercise 065 — Memoized Fibonacci (intermediate).
// Goal:   Compute the nth Fibonacci number using top-down recursion backed
//         by a dictionary-based memo cache, so repeated sub-problems are
//         never recomputed.
// Drills: recursion, memoization, dictionaries, algorithmic complexity.
public static class MemoizedFibonacci
{
    // Number of times the recursive helper was entered during the most
    // recent call to Calculate — used to verify memoization actually
    // bounds the number of recursive calls (as opposed to naive recursion,
    // which is exponential in n).
    public static int CallCount { get; private set; }

    public static long Calculate(int n) => throw new NotImplementedException();
}
