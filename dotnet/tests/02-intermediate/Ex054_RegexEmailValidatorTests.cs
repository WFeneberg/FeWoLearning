using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex054_RegexEmailValidatorTests
{
    [Theory]
    [InlineData("jane.doe@example.com", true)]
    [InlineData("john_smith+work@sub.example.co", true)]
    [InlineData("a@b.io", true)]
    [InlineData("first.last@my-domain.org", true)]
    [InlineData("no-at-sign.example.com", false)]
    [InlineData("missing.domain@", false)]
    [InlineData("@missing-local.com", false)]
    [InlineData("double..dot@example.com", false)]
    [InlineData("has space@example.com", false)]
    [InlineData("no.tld@example", false)]
    [InlineData("bad.tld@example.c", false)]
    [InlineData(" leading.space@example.com", false)]
    [InlineData("trailing.space@example.com ", false)]
    [InlineData("two@@signs@example.com", false)]
    public void IsValid_ReturnsExpected(string email, bool expected)
        => Assert.Equal(expected, RegexEmailValidator.IsValid(email));
}
