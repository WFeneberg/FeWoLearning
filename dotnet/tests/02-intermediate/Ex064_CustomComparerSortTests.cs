using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex064_CustomComparerSortTests
{
    [Fact]
    public void SortByLengthThenAlpha_OrdersByLengthThenAlphabetically()
    {
        var input = new List<string> { "banana", "fig", "kiwi", "apple", "pear", "date", "a" };

        var result = CustomComparerSort.SortByLengthThenAlpha(input);

        Assert.Equal(
            new[] { "a", "fig", "date", "kiwi", "pear", "apple", "banana" },
            result);
    }

    [Fact]
    public void SortByLengthThenAlpha_UsesListSortWithComparerDirectly()
    {
        var list = new List<string> { "bb", "aa", "c", "b", "a" };

        list.Sort(new LengthThenAlphaComparer());

        Assert.Equal(new[] { "a", "b", "c", "aa", "bb" }, list);
    }

    [Fact]
    public void LengthThenAlphaComparer_Compare_ReturnsExpectedSign()
    {
        var comparer = new LengthThenAlphaComparer();

        Assert.True(comparer.Compare("a", "bb") < 0);
        Assert.True(comparer.Compare("bb", "a") > 0);
        Assert.True(comparer.Compare("apple", "bear") > 0);
        Assert.True(comparer.Compare("bear", "apple") < 0);
        Assert.Equal(0, comparer.Compare("cat", "cat"));
    }
}
