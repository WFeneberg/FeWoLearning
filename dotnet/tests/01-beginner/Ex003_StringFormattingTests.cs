using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex003_StringFormattingTests
{
    [Theory]
    [InlineData("Ada", 36, "Name: Ada, Age: 36")]
    [InlineData("Bob", 7, "Name: Bob, Age: 07")]
    [InlineData("Grace", 0, "Name: Grace, Age: 00")]
    [InlineData("Zoe", 100, "Name: Zoe, Age: 100")]
    public void Describe_ReturnsExpected(string name, int age, string expected)
        => Assert.Equal(expected, StringFormatting.Describe(name, age));
}
