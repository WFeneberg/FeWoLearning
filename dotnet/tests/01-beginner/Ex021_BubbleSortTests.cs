using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex021_BubbleSortTests
{
    public static IEnumerable<object[]> RandomArrays()
    {
        yield return new object[] { new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6, 0 } };
        yield return new object[] { new[] { 42, -7, 13, 0, -100, 56, 23, -1, 8, 99, 17 } };
        yield return new object[] { new[] { 1 } };
        yield return new object[] { new[] { 2, 2, 1, 1, 3, 3 } };
        yield return new object[] { new int[0] };
        yield return new object[] { new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 } };
    }

    [Theory]
    [MemberData(nameof(RandomArrays))]
    public void Sort_MatchesArraySort_ForRandomArrays(int[] values)
    {
        var expected = (int[])values.Clone();
        Array.Sort(expected);

        var actual = (int[])values.Clone();
        BubbleSort.Sort(actual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sort_SpecificArray_ProducesExpectedOrder()
    {
        var values = new[] { 64, 34, 25, 12, 22, 11, 90 };

        BubbleSort.Sort(values);

        Assert.Equal(new[] { 11, 12, 22, 25, 34, 64, 90 }, values);
    }
}
