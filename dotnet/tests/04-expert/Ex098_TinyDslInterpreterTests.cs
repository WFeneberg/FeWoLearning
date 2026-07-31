using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex098_TinyDslInterpreterTests
{
    [Theory]
    [InlineData("2 + 3 * 4", 14)]
    [InlineData("(2 + 3) * 4", 20)]
    [InlineData("10 / 4", 2.5)]
    [InlineData("2 + 3 * 4 - 1", 13)]
    [InlineData("-2 + 5", 3)]
    [InlineData("-(2 + 5)", -7)]
    [InlineData("100 - 20 / 5 * 2", 92)]
    public void EvaluatesRespectingPrecedence(string expression, double expected)
        => Assert.Equal(expected, TinyDslInterpreter.Evaluate(expression), 10);

    [Fact]
    public void PowerIsRightAssociative()
        // 3 ^ 2 = 9, then 2 ^ 9 = 512 (not (2^3)^2 = 64).
        => Assert.Equal(512, TinyDslInterpreter.Evaluate("2 ^ 3 ^ 2"), 10);

    [Fact]
    public void UnaryMinusBindsLooserThanPower()
        // -2^2 must be -(2^2) = -4, not (-2)^2 = 4.
        => Assert.Equal(-4, TinyDslInterpreter.Evaluate("-2 ^ 2"), 10);

    [Fact]
    public void SupportsNestedParenthesesAndDecimals()
        => Assert.Equal(15, TinyDslInterpreter.Evaluate("((1 + 2.5) * 2) + 8"), 10);

    [Fact]
    public void ResolvesVariablesFromDictionary()
    {
        var vars = new Dictionary<string, double> { ["x"] = 3, ["y"] = 4 };
        Assert.Equal(22, TinyDslInterpreter.Evaluate("x * 2 + y * 4", vars), 10);
    }

    [Fact]
    public void UnknownIdentifierThrowsFormatException()
        => Assert.Throws<FormatException>(() => TinyDslInterpreter.Evaluate("x + 1"));

    [Fact]
    public void DivisionByZeroThrows()
        => Assert.Throws<DivideByZeroException>(() => TinyDslInterpreter.Evaluate("1 / 0"));

    [Fact]
    public void MalformedExpressionThrowsFormatException()
        => Assert.Throws<FormatException>(() => TinyDslInterpreter.Evaluate("2 + * 3"));

    [Fact]
    public void UnbalancedParenthesesThrowsFormatException()
        => Assert.Throws<FormatException>(() => TinyDslInterpreter.Evaluate("(1 + 2"));
}
