using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex068_MergeSortImplTests
{
    [Fact]
    public void Sort_EmptyArray_ReturnsEmptyArray()
    {
        var input = System.Array.Empty<int>();

        var result = MergeSortImpl.Sort(input);

        Assert.Empty(result);
    }

    [Fact]
    public void Sort_AlreadySortedArray_ReturnsSameOrder()
    {
        var input = new[] { 1, 2, 3, 4, 5 };

        var result = MergeSortImpl.Sort(input);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void Sort_UnorderedArray_ReturnsAscendingOrder()
    {
        var input = new[] { 38, 27, 43, 3, 9, 82, 10 };

        var result = MergeSortImpl.Sort(input);

        Assert.Equal(new[] { 3, 9, 10, 27, 38, 43, 82 }, result);
    }

    [Fact]
    public void Sort_ArrayWithDuplicatesAndNegatives_ReturnsAscendingOrder()
    {
        var input = new[] { 5, -1, 3, 5, -8, 0, -1, 7, 3 };

        var result = MergeSortImpl.Sort(input);

        Assert.Equal(new[] { -8, -1, -1, 0, 3, 3, 5, 5, 7 }, result);
    }

    [Fact]
    public void Sort_SingleElementArray_ReturnsSameArray()
    {
        var input = new[] { 42 };

        var result = MergeSortImpl.Sort(input);

        Assert.Equal(new[] { 42 }, result);
    }

    [Fact]
    public void Sort_DoesNotMutateInputArray()
    {
        var input = new[] { 9, 4, 7, 1 };

        MergeSortImpl.Sort(input);

        Assert.Equal(new[] { 9, 4, 7, 1 }, input);
    }
}
