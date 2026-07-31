using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex067_BinarySearchImplTests
{
    private static readonly int[] SortedValues = { -7, -3, 0, 2, 5, 8, 11, 15, 20, 34, 55 };

    [Theory]
    [InlineData(-7, 0)]
    [InlineData(-3, 1)]
    [InlineData(0, 2)]
    [InlineData(2, 3)]
    [InlineData(5, 4)]
    [InlineData(8, 5)]
    [InlineData(11, 6)]
    [InlineData(15, 7)]
    [InlineData(20, 8)]
    [InlineData(34, 9)]
    [InlineData(55, 10)]
    public void Search_FindsPresentValues_AtCorrectIndex(int target, int expectedIndex)
        => Assert.Equal(expectedIndex, BinarySearchImpl.Search(SortedValues, target));

    [Theory]
    [InlineData(-100)]
    [InlineData(-8)]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(21)]
    [InlineData(56)]
    [InlineData(1000)]
    public void Search_ReturnsMinusOne_WhenValueMissing(int target)
        => Assert.Equal(-1, BinarySearchImpl.Search(SortedValues, target));

    [Fact]
    public void Search_EmptyArray_ReturnsMinusOne()
        => Assert.Equal(-1, BinarySearchImpl.Search(Array.Empty<int>(), 5));

    [Fact]
    public void Search_SingleElementArray_FindsMatch()
        => Assert.Equal(0, BinarySearchImpl.Search(new[] { 42 }, 42));

    [Fact]
    public void Search_SingleElementArray_MissesNonMatch()
        => Assert.Equal(-1, BinarySearchImpl.Search(new[] { 42 }, 7));
}
