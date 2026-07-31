using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex002_NullableValueTypesTests
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-4, 10, 6)]
    [InlineData(0, 0, 0)]
    [InlineData(null, 5, null)]
    [InlineData(5, null, null)]
    [InlineData(null, null, null)]
    public void Add_ReturnsExpected(int? a, int? b, int? expected)
        => Assert.Equal(expected, NullableValueTypes.Add(a, b));
}
