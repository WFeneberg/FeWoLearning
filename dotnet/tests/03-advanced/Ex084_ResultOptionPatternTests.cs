using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex084_ResultOptionPatternTests
{
    [Fact]
    public void SuccessMapTransformsValue()
    {
        var result = Result<int>.Success(4).Map(x => x * 2);
        Assert.True(result.IsSuccess);
        Assert.Equal(8, result.Value);
    }

    [Fact]
    public void FailureMapShortCircuitsAndPreservesError()
    {
        var callCount = 0;
        var result = Result<int>.Failure("boom")
            .Map(x => { callCount++; return x * 2; });

        Assert.True(result.IsFailure);
        Assert.Equal("boom", result.Error);
        Assert.Equal(0, callCount); // mapper must never run on a failed Result
    }

    [Fact]
    public void BindChainsSuccessesWithoutThrowing()
    {
        var result = ResultOptionPattern.ParseAndDivide("20/4");
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void ChainedBindAndMapPropagateFailureWithoutThrowingAndShortCircuit()
    {
        var laterMapCalls = 0;
        var laterBindCalls = 0;

        // "10/0" parses fine but division fails -> Failure should propagate
        // through further Bind/Map calls without ever invoking them, and
        // without throwing any exception.
        var result = ResultOptionPattern.ParseAndDivide("10/0")
            .Map(x => { laterMapCalls++; return x + 100; })
            .Bind(x => { laterBindCalls++; return Result<string>.Success(x.ToString()); });

        Assert.True(result.IsFailure);
        Assert.Equal("Division by zero.", result.Error);
        Assert.Equal(0, laterMapCalls);
        Assert.Equal(0, laterBindCalls);
    }

    [Fact]
    public void ChainedBindPropagatesParseFailureWithoutThrowing()
    {
        var result = ResultOptionPattern.ParseAndDivide("abc/2")
            .Map(x => x * 10);

        Assert.True(result.IsFailure);
        Assert.Equal("'abc' is not a valid integer.", result.Error);
    }

    [Fact]
    public void AccessingValueOnFailureThrowsInvalidOperation()
    {
        var result = Result<int>.Failure("nope");
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void GetValueOrDefaultReturnsFallbackOnFailure()
    {
        var result = ResultOptionPattern.Divide(1, 0);
        Assert.Equal(-1, result.GetValueOrDefault(-1));
    }

    [Fact]
    public void GetValueOrDefaultReturnsValueOnSuccess()
    {
        var result = ResultOptionPattern.Divide(9, 3);
        Assert.Equal(3, result.GetValueOrDefault(-1));
    }
}
