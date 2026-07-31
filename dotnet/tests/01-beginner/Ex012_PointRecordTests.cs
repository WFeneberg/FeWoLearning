using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex012_PointRecordTests
{
    [Fact]
    public void Translate_ReturnsShiftedPoint()
    {
        var origin = new Point2D(1, 2);

        var moved = origin.Translate(3, -1);

        Assert.Equal(new Point2D(4, 1), moved);
    }

    [Fact]
    public void RecordEquality_IsBasedOnValues()
    {
        var a = new Point2D(5, 7);
        var b = new Point2D(5, 7);

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void Translate_DoesNotMutateOriginal()
    {
        var original = new Point2D(0, 0);

        var moved = original.Translate(2, 2);

        Assert.Equal(new Point2D(0, 0), original);
        Assert.Equal(new Point2D(2, 2), moved);
    }
}
