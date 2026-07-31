using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex077_ExpressionTreeBuilderTests
{
    [Fact]
    public void BuildPredicateProducesExpressionMatchingOperator()
    {
        Expression<Func<int, bool>> expr = ExpressionTreeBuilder.BuildPredicate(
            ExpressionTreeBuilder.Comparison.GreaterThan, 5);

        var binary = Assert.IsAssignableFrom<BinaryExpression>(expr.Body);
        Assert.Equal(ExpressionType.GreaterThan, binary.NodeType);
        Assert.Single(expr.Parameters);
    }

    [Fact]
    public void CompilePredicateEvaluatesCorrectlyForEachOperator()
    {
        var gt = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.GreaterThan, 5);
        Assert.True(gt(6));
        Assert.False(gt(5));

        var lt = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.LessThan, 5);
        Assert.True(lt(4));
        Assert.False(lt(5));

        var eq = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.EqualTo, 5);
        Assert.True(eq(5));
        Assert.False(eq(4));

        var ge = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.GreaterThanOrEqual, 5);
        Assert.True(ge(5));
        Assert.False(ge(4));

        var le = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.LessThanOrEqual, 5);
        Assert.True(le(5));
        Assert.False(le(6));

        var ne = ExpressionTreeBuilder.CompilePredicate(ExpressionTreeBuilder.Comparison.NotEqualTo, 5);
        Assert.True(ne(4));
        Assert.False(ne(5));
    }

    [Fact]
    public void FilterReturnsExpectedMatchingElements()
    {
        var source = new[] { 1, 3, 5, 7, 9, 2, 4, 6, 8, 10 };

        var result = ExpressionTreeBuilder.Filter(source, ExpressionTreeBuilder.Comparison.GreaterThan, 5);

        Assert.Equal(new List<int> { 7, 9, 6, 8, 10 }, result);
    }

    [Fact]
    public void FilterWithEqualToReturnsOnlyMatchingElement()
    {
        var source = Enumerable.Range(1, 10);

        var result = ExpressionTreeBuilder.Filter(source, ExpressionTreeBuilder.Comparison.EqualTo, 7);

        Assert.Equal(new List<int> { 7 }, result);
    }

    [Fact]
    public void FilterWithNoMatchesReturnsEmptyList()
    {
        var source = new[] { 1, 2, 3 };

        var result = ExpressionTreeBuilder.Filter(source, ExpressionTreeBuilder.Comparison.GreaterThan, 100);

        Assert.Empty(result);
    }
}
