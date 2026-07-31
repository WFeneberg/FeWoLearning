using System.Linq.Expressions;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 077 — Expression tree builder (advanced).
// Goal:   Dynamically build an Expression<Func<int,bool>> predicate for a given
//         comparison operator and threshold, compile it, and use it to filter data.
// Drills: expression trees, Expression.Parameter/Constant/Lambda, delegate compilation.
public static class ExpressionTreeBuilder
{
    public enum Comparison
    {
        GreaterThan,
        LessThan,
        EqualTo,
        GreaterThanOrEqual,
        LessThanOrEqual,
        NotEqualTo,
    }

    // Builds (but does not compile) an expression tree representing "x <op> threshold".
    public static Expression<Func<int, bool>> BuildPredicate(Comparison op, int threshold)
        => throw new NotImplementedException();

    // Compiles the predicate built by BuildPredicate into an invokable delegate.
    public static Func<int, bool> CompilePredicate(Comparison op, int threshold)
        => throw new NotImplementedException();

    // Filters the source sequence using a compiled predicate for the given operator/threshold.
    public static List<int> Filter(IEnumerable<int> source, Comparison op, int threshold)
        => throw new NotImplementedException();
}
