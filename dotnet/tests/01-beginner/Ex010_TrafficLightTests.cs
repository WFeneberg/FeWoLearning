using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex010_TrafficLightTests
{
    [Fact]
    public void Next_CyclesThroughFullSequence()
    {
        var light = TrafficLight.Red;

        light = light.Next();
        Assert.Equal(TrafficLight.Green, light);

        light = light.Next();
        Assert.Equal(TrafficLight.Yellow, light);

        light = light.Next();
        Assert.Equal(TrafficLight.Red, light);
    }

    [Theory]
    [InlineData(TrafficLight.Red, TrafficLight.Green)]
    [InlineData(TrafficLight.Green, TrafficLight.Yellow)]
    [InlineData(TrafficLight.Yellow, TrafficLight.Red)]
    public void Next_ReturnsExpected(TrafficLight current, TrafficLight expected)
        => Assert.Equal(expected, current.Next());
}
