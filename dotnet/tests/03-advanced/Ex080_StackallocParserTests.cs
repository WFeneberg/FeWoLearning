using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex080_StackallocParserTests
{
    [Fact]
    public void ParsesCommaSeparatedIntegersIntoStackallocSpan()
    {
        Span<int> buffer = stackalloc int[StackallocParser.MaxValues];

        int count = StackallocParser.Parse("10, 20, 30, -5", buffer);

        Assert.Equal(4, count);
        Assert.Equal(10, buffer[0]);
        Assert.Equal(20, buffer[1]);
        Assert.Equal(30, buffer[2]);
        Assert.Equal(-5, buffer[3]);
    }

    [Fact]
    public void ParsesSingleValue()
    {
        Span<int> buffer = stackalloc int[StackallocParser.MaxValues];

        int count = StackallocParser.Parse("42", buffer);

        Assert.Equal(1, count);
        Assert.Equal(42, buffer[0]);
    }

    [Fact]
    public void IgnoresLeadingAndTrailingWhitespaceAroundEntries()
    {
        Span<int> buffer = stackalloc int[StackallocParser.MaxValues];

        int count = StackallocParser.Parse("  1 ,2,   3  ", buffer);

        Assert.Equal(3, count);
        Assert.Equal(1, buffer[0]);
        Assert.Equal(2, buffer[1]);
        Assert.Equal(3, buffer[2]);
    }

    [Fact]
    public void ThrowsWhenInputHasMoreValuesThanDestinationCanHold()
    {
        Span<int> buffer = stackalloc int[2];
        bool threw = false;

        try
        {
            StackallocParser.Parse("1,2,3", buffer);
        }
        catch (ArgumentException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void ThrowsFormatExceptionForNonIntegerEntry()
    {
        Span<int> buffer = stackalloc int[StackallocParser.MaxValues];
        bool threw = false;

        try
        {
            StackallocParser.Parse("1,two,3", buffer);
        }
        catch (FormatException)
        {
            threw = true;
        }

        Assert.True(threw);
    }
}
