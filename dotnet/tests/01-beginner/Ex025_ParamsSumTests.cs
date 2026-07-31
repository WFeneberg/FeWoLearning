using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex025_ParamsSumTests
{
    [Fact]
    public void Sum_NoArguments_ReturnsZero()
        => Assert.Equal(0, ParamsSum.Sum());

    [Fact]
    public void Sum_OneArgument_ReturnsThatValue()
        => Assert.Equal(42, ParamsSum.Sum(42));

    [Fact]
    public void Sum_ManyArguments_ReturnsTotal()
        => Assert.Equal(15, ParamsSum.Sum(1, 2, 3, 4, 5));

    [Fact]
    public void Sum_IncludesNegativeValues()
        => Assert.Equal(-1, ParamsSum.Sum(2, -3));
}
