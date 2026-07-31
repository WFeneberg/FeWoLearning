using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex020_PalindromeCheckTests
{
    [Theory]
    [InlineData("racecar", true)]
    [InlineData("Racecar", true)]
    [InlineData("A man a plan a canal Panama", true)]
    [InlineData("hello", false)]
    [InlineData("", true)]
    [InlineData("Was it a car or a cat I saw", true)]
    [InlineData("dotnet", false)]
    public void IsPalindrome_ReturnsExpected(string input, bool expected)
        => Assert.Equal(expected, PalindromeCheck.IsPalindrome(input));
}
