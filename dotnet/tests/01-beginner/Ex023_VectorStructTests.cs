using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex023_VectorStructTests
{
    [Fact]
    public void Add_ReturnsSumOfComponents()
    {
        var a = new VectorStruct(1, 2);
        var b = new VectorStruct(3, 4);

        var result = a.Add(b);

        Assert.Equal(4, result.X);
        Assert.Equal(6, result.Y);
    }

    [Fact]
    public void CopyingStruct_AndMutatingCopy_LeavesOriginalUnchanged()
    {
        var original = new VectorStruct(1, 1);
        var copy = original;

        copy.X = 99;
        copy.Y = 99;

        Assert.Equal(1, original.X);
        Assert.Equal(1, original.Y);
        Assert.Equal(99, copy.X);
        Assert.Equal(99, copy.Y);
    }
}
