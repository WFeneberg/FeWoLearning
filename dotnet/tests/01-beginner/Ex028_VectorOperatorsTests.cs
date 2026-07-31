using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex028_VectorOperatorsTests
{
    [Fact]
    public void Addition_CombinesComponents()
    {
        var a = new VectorOperators(2, 3);
        var b = new VectorOperators(4, -1);

        var result = a + b;

        Assert.Equal(6, result.X);
        Assert.Equal(2, result.Y);
    }

    [Fact]
    public void Equality_ComparesByValue()
    {
        var a = new VectorOperators(5, 7);
        var b = new VectorOperators(5, 7);
        var c = new VectorOperators(5, 8);

        Assert.True(a == b);
        Assert.False(a == c);
        Assert.False(a != b);
        Assert.True(a != c);
        Assert.Equal(a, b);
    }
}
