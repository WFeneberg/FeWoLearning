using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex015_SafeDivideTests
{
    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(9, 3, 3)]
    [InlineData(7, 2, 3)]
    [InlineData(-10, 2, -5)]
    public void Divide_ReturnsQuotient_WhenDenominatorNotZero(int numerator, int denominator, int expected)
        => Assert.Equal(expected, SafeDivide.Divide(numerator, denominator));

    [Fact]
    public void Divide_ReturnsNull_WhenDenominatorIsZero()
        => Assert.Null(SafeDivide.Divide(10, 0));
}
