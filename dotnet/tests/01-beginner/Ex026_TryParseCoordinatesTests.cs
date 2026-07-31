using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex026_TryParseCoordinatesTests
{
    [Fact]
    public void TryParsePoint_ValidInput_ReturnsTrueAndCorrectValues()
    {
        var success = TryParseCoordinates.TryParsePoint("3,4", out var x, out var y);

        Assert.True(success);
        Assert.Equal(3, x);
        Assert.Equal(4, y);
    }

    [Fact]
    public void TryParsePoint_NegativeValues_ReturnsTrueAndCorrectValues()
    {
        var success = TryParseCoordinates.TryParsePoint("-10,7", out var x, out var y);

        Assert.True(success);
        Assert.Equal(-10, x);
        Assert.Equal(7, y);
    }

    [Theory]
    [InlineData("3")]
    [InlineData("3,4,5")]
    [InlineData("a,4")]
    [InlineData("3,b")]
    [InlineData("")]
    [InlineData("3,")]
    public void TryParsePoint_MalformedInput_ReturnsFalseAndZeroes(string input)
    {
        var success = TryParseCoordinates.TryParsePoint(input, out var x, out var y);

        Assert.False(success);
        Assert.Equal(0, x);
        Assert.Equal(0, y);
    }
}
