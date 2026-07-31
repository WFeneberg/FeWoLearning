using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex029_MathUtilsStaticTests
{
    [Theory]
    [InlineData(5, 0, 10, 5)]
    [InlineData(0, 0, 10, 0)]
    [InlineData(10, 0, 10, 10)]
    [InlineData(-3, 0, 10, 0)]
    [InlineData(15, 0, 10, 10)]
    [InlineData(-100, -10, -1, -10)]
    [InlineData(7, 7, 7, 7)]
    public void Clamp_ReturnsExpected(int value, int min, int max, int expected)
        => Assert.Equal(expected, MathUtilsStatic.Clamp(value, min, max));
}
