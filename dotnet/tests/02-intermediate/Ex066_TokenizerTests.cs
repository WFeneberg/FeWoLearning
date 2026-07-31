using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex066_TokenizerTests
{
    [Fact]
    public void Tokenize_SimpleAddition_ReturnsExpectedTokens()
    {
        var expected = new List<string> { "3", "+", "4", "*", "2" };
        Assert.Equal(expected, Tokenizer.Tokenize("3 + 4 * 2"));
    }

    [Fact]
    public void Tokenize_DecimalsAndParentheses_ReturnsExpectedTokens()
    {
        var expected = new List<string> { "12.5", "*", "(", "3", "-", "1", ")" };
        Assert.Equal(expected, Tokenizer.Tokenize("12.5*(3-1)"));
    }

    [Fact]
    public void Tokenize_NoWhitespace_StillSplitsTokens()
    {
        var expected = new List<string> { "100", "/", "5", "-", "20" };
        Assert.Equal(expected, Tokenizer.Tokenize("100/5-20"));
    }

    [Fact]
    public void Tokenize_ExtraWhitespaceIsIgnored()
    {
        var expected = new List<string> { "7", "-", "2" };
        Assert.Equal(expected, Tokenizer.Tokenize("   7   -    2  "));
    }

    [Fact]
    public void Tokenize_EmptyExpression_ReturnsEmptyList()
    {
        Assert.Empty(Tokenizer.Tokenize(""));
    }

    [Fact]
    public void Tokenize_SingleNumber_ReturnsSingleToken()
    {
        var expected = new List<string> { "42" };
        Assert.Equal(expected, Tokenizer.Tokenize("42"));
    }

    [Fact]
    public void Tokenize_InvalidCharacter_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Tokenizer.Tokenize("3 + $4"));
    }

    [Fact]
    public void Tokenize_MultipleDecimalPointsInNumber_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Tokenizer.Tokenize("1.2.3 + 4"));
    }
}
