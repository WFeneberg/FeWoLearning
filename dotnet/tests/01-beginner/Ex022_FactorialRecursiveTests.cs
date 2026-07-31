using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex022_FactorialRecursiveTests
{
    [Theory]
    [InlineData(0, 1L)]
    [InlineData(1, 1L)]
    [InlineData(2, 2L)]
    [InlineData(3, 6L)]
    [InlineData(5, 120L)]
    [InlineData(10, 3628800L)]
    public void Compute_ReturnsExpected(int n, long expected)
        => Assert.Equal(expected, FactorialRecursive.Compute(n));

    [Fact]
    public void Compute_NegativeInput_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => FactorialRecursive.Compute(-1));
}
