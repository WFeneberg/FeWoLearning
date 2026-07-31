using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex007_SumOfDigitsTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(9, 9)]
    [InlineData(123, 6)]
    [InlineData(4567, 22)]
    [InlineData(-123, 6)]
    [InlineData(-8, 8)]
    [InlineData(1000000, 1)]
    public void Compute_ReturnsExpected(int n, int expected)
        => Assert.Equal(expected, SumOfDigits.Compute(n));
}
