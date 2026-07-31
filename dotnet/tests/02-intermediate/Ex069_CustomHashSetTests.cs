using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex069_CustomHashSetTests
{
    [Fact]
    public void Add_NewItems_IncreasesCount()
    {
        var set = new CustomHashSet();

        Assert.True(set.Add("apple"));
        Assert.True(set.Add("banana"));
        Assert.True(set.Add("cherry"));

        Assert.Equal(3, set.Count);
    }

    [Fact]
    public void Add_DuplicateItem_DoesNotGrowCount()
    {
        var set = new CustomHashSet();

        Assert.True(set.Add("apple"));
        Assert.False(set.Add("apple"));
        Assert.False(set.Add("apple"));

        Assert.Equal(1, set.Count);
    }

    [Fact]
    public void Contains_ReturnsTrueOnlyForAddedItems()
    {
        var set = new CustomHashSet();
        set.Add("apple");
        set.Add("banana");

        Assert.True(set.Contains("apple"));
        Assert.True(set.Contains("banana"));
        Assert.False(set.Contains("cherry"));
        Assert.False(set.Contains(""));
    }

    [Fact]
    public void Contains_ReturnsFalse_OnEmptySet()
    {
        var set = new CustomHashSet();

        Assert.False(set.Contains("anything"));
        Assert.Equal(0, set.Count);
    }

    [Fact]
    public void Add_ManyItemsAcrossBuckets_TracksCountAndMembership()
    {
        var set = new CustomHashSet();
        var words = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

        foreach (var word in words)
        {
            set.Add(word);
        }

        // Re-adding all of them again must not change the count.
        foreach (var word in words)
        {
            set.Add(word);
        }

        Assert.Equal(words.Length, set.Count);
        foreach (var word in words)
        {
            Assert.True(set.Contains(word));
        }

        Assert.False(set.Contains("eleven"));
    }
}
