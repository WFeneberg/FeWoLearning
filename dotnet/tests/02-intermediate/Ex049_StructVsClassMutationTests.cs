using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex049_StructVsClassMutationTests
{
    [Fact]
    public void MoveStruct_DoesNotAffectCallersCopy()
    {
        var point = new PointStruct { X = 1, Y = 2 };

        StructVsClassMutation.MoveStruct(point, 10, 20);

        Assert.Equal(1, point.X);
        Assert.Equal(2, point.Y);
    }

    [Fact]
    public void MoveClass_MutatesCallersInstance()
    {
        var point = new PointClass { X = 1, Y = 2 };

        StructVsClassMutation.MoveClass(point, 10, 20);

        Assert.Equal(11, point.X);
        Assert.Equal(22, point.Y);
    }
}
