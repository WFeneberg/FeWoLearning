using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex061_PatternMatchingListsTests
{
    [Theory]
    [InlineData(new int[] { }, "Empty")]
    [InlineData(new[] { 7 }, "Single:7")]
    [InlineData(new[] { 3, 3 }, "Pair:Equal")]
    [InlineData(new[] { 1, 2, 1 }, "Bookended")]
    [InlineData(new[] { 5, 1, 2, 3, 5 }, "Bookended")]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, "Sorted")]
    [InlineData(new[] { 1, 1, 2, 2, 3 }, "Sorted")]
    [InlineData(new[] { 5, 3, 1, 2, 4 }, "Other")]
    [InlineData(new[] { 1, 2, 3 }, "Other")]
    [InlineData(new[] { 4, 2 }, "Other")]
    public void Classify_ReturnsExpectedLabel(int[] sequence, string expected)
        => Assert.Equal(expected, PatternMatchingLists.Classify(sequence));
}
