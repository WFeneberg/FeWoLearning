using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex042_StringExtensionsTests
{
    [Theory]
    [InlineData("racecar", true)]
    [InlineData("A man, a plan, a canal: Panama", true)]
    [InlineData("Was it a car or a cat I saw?", true)]
    [InlineData("hello", false)]
    [InlineData("", false)]
    [InlineData("a", true)]
    [InlineData("No lemon, no melon", true)]
    [InlineData("dotnet", false)]
    public void IsPalindromeExt_ReturnsExpected(string input, bool expected)
        => Assert.Equal(expected, input.IsPalindromeExt());

    [Fact]
    public void IsPalindromeExt_Null_ReturnsFalse()
    {
        string? value = null;
        Assert.False(value.IsPalindromeExt());
    }

    [Fact]
    public void IsPalindromeExt_CanBeCalledAsInstanceMethod()
    {
        // Verifies the extension method syntax (instance-style call) works,
        // not just static invocation via StringExtensions.IsPalindromeExt(...).
        var result = "Madam".IsPalindromeExt();

        Assert.True(result);
    }
}
