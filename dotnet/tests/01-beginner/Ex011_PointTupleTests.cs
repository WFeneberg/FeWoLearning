using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex011_PointTupleTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0.0)]
    [InlineData(0, 0, 3, 4, 5.0)]
    [InlineData(0, 0, 4, 3, 5.0)]
    [InlineData(1, 1, 4, 5, 5.0)]
    [InlineData(-1, -1, 2, 3, 5.0)]
    [InlineData(2, 2, 2, 2, 0.0)]
    public void Distance_ReturnsExpected(double x1, double y1, double x2, double y2, double expected)
        => Assert.Equal(expected, PointTuple.Distance((x1, y1), (x2, y2)), 3);
}
