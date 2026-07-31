using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex017_FilterEvenSquaresTests
{
    [Fact]
    public void Evaluate_OneToTen_ReturnsSquaresOfEvenNumbers()
    {
        var expected = new List<int> { 4, 16, 36, 64, 100 };

        Assert.Equal(expected, FilterEvenSquares.Evaluate(1, 10));
    }

    [Fact]
    public void Evaluate_NoEvenNumbersInRange_ReturnsEmptyList()
    {
        var expected = new List<int>();

        Assert.Equal(expected, FilterEvenSquares.Evaluate(1, 1));
    }

    [Fact]
    public void Evaluate_RangeStartingAtEvenNumber_IncludesIt()
    {
        var expected = new List<int> { 4, 16 };

        Assert.Equal(expected, FilterEvenSquares.Evaluate(2, 5));
    }

    [Fact]
    public void Evaluate_NegativeToPositiveRange_ReturnsExpectedSquares()
    {
        var expected = new List<int> { 4, 0, 4 };

        Assert.Equal(expected, FilterEvenSquares.Evaluate(-2, 2));
    }
}
