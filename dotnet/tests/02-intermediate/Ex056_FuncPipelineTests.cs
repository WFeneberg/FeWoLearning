using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex056_FuncPipelineTests
{
    [Fact]
    public void Compose_AppliesStepsInOrder()
    {
        Func<int, int> pipeline = FuncPipeline.Compose(
            x => x + 1,
            x => x * 2,
            x => x - 3);

        // (5 + 1) * 2 - 3 = 9
        Assert.Equal(9, pipeline(5));
    }

    [Fact]
    public void Compose_WithNoSteps_IsIdentity()
    {
        Func<int, int> pipeline = FuncPipeline.Compose();

        Assert.Equal(42, pipeline(42));
    }

    [Fact]
    public void Compose_WithSingleStep_AppliesThatStepOnly()
    {
        Func<int, int> pipeline = FuncPipeline.Compose(x => x * x);

        Assert.Equal(49, pipeline(7));
    }

    [Theory]
    [InlineData(0, 6)]
    [InlineData(10, 26)]
    [InlineData(-4, -2)]
    public void Run_ComposesAndAppliesInSingleCall(int input, int expected)
    {
        // pipeline: double, then add 6
        var result = FuncPipeline.Run(input, x => x * 2, x => x + 6);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compose_OrderMatters_ReversedStepsGiveDifferentResult()
    {
        Func<int, int> forward = FuncPipeline.Compose(x => x - 1, x => x * 10);
        Func<int, int> reversed = FuncPipeline.Compose(x => x * 10, x => x - 1);

        Assert.Equal(40, forward(5));
        Assert.Equal(49, reversed(5));
        Assert.NotEqual(forward(5), reversed(5));
    }
}
