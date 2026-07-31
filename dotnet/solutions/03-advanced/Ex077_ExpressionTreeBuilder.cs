using System.Linq.Expressions;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 077 — Expression tree builder (reference solution).
// Build the tree by hand: a ParameterExpression "x", a ConstantExpression for the
// threshold, a BinaryExpression comparing them, wrapped in a lambda over int -> bool.
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

    public static Expression<Func<int, bool>> BuildPredicate(Comparison op, int threshold)
    {
        var parameter = Expression.Parameter(typeof(int), "x");
        var constant = Expression.Constant(threshold, typeof(int));

        Expression body = op switch
        {
            Comparison.GreaterThan => Expression.GreaterThan(parameter, constant),
            Comparison.LessThan => Expression.LessThan(parameter, constant),
            Comparison.EqualTo => Expression.Equal(parameter, constant),
            Comparison.GreaterThanOrEqual => Expression.GreaterThanOrEqual(parameter, constant),
            Comparison.LessThanOrEqual => Expression.LessThanOrEqual(parameter, constant),
            Comparison.NotEqualTo => Expression.NotEqual(parameter, constant),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown comparison."),
        };

        return Expression.Lambda<Func<int, bool>>(body, parameter);
    }

    public static Func<int, bool> CompilePredicate(Comparison op, int threshold)
        => BuildPredicate(op, threshold).Compile();

    public static List<int> Filter(IEnumerable<int> source, Comparison op, int threshold)
    {
        var predicate = CompilePredicate(op, threshold);
        return source.Where(predicate).ToList();
    }
}
