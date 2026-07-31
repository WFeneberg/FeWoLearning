using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex008_CollatzStepsTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 7)]
    [InlineData(6, 8)]
    [InlineData(7, 16)]
    [InlineData(27, 111)]
    public void Count_ReturnsExpected(int n, int expected)
        => Assert.Equal(expected, CollatzSteps.Count(n));
}
