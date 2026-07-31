using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex019_ParseOrDefaultTests
{
    [Theory]
    [InlineData("42", -1, 42)]
    [InlineData("0", -1, 0)]
    [InlineData("-7", 0, -7)]
    [InlineData("abc", -1, -1)]
    [InlineData("", 99, 99)]
    [InlineData("3.14", 5, 5)]
    [InlineData("12abc", 5, 5)]
    public void ParseIntOrDefault_ReturnsExpected(string text, int fallback, int expected)
        => Assert.Equal(expected, ParseOrDefault.ParseIntOrDefault(text, fallback));

    [Fact]
    public void ParseIntOrDefault_NullInput_ReturnsFallback()
        => Assert.Equal(7, ParseOrDefault.ParseIntOrDefault(null, 7));
}
