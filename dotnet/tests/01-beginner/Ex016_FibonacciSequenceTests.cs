using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex016_FibonacciSequenceTests
{
    [Fact]
    public void Generate_FirstTenNumbers_MatchesExpectedSequence()
    {
        var expected = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };

        var actual = FibonacciSequence.Generate(10).ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Generate_Zero_ReturnsEmptySequence()
    {
        var actual = FibonacciSequence.Generate(0).ToArray();

        Assert.Empty(actual);
    }

    [Fact]
    public void Generate_One_ReturnsJustZero()
    {
        var actual = FibonacciSequence.Generate(1).ToArray();

        Assert.Equal(new long[] { 0 }, actual);
    }
}
