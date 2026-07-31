using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex009_GradeClassifierTests
{
    [Theory]
    [InlineData(100, "A")]
    [InlineData(90, "A")]
    [InlineData(89, "B")]
    [InlineData(80, "B")]
    [InlineData(79, "C")]
    [InlineData(70, "C")]
    [InlineData(69, "D")]
    [InlineData(60, "D")]
    [InlineData(59, "F")]
    [InlineData(0, "F")]
    public void Classify_ReturnsExpected(int score, string expected)
        => Assert.Equal(expected, GradeClassifier.Classify(score));
}
