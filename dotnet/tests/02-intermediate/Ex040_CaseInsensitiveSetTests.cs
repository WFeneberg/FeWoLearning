using System.Collections.Generic;
using System.Linq;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex040_CaseInsensitiveSetTests
{
    [Fact]
    public void Build_RemovesDuplicatesDifferingOnlyByCase()
    {
        var input = new[] { "Apple", "apple", "APPLE", "Banana", "banana", "Cherry" };

        var result = CaseInsensitiveSet.Build(input);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Build_KeepsFirstOccurrenceCasing()
    {
        var input = new[] { "Apple", "apple", "APPLE" };

        var result = CaseInsensitiveSet.Build(input);

        Assert.Single(result);
        Assert.Equal("Apple", result.Single());
    }

    [Fact]
    public void Build_PreservesDistinctValuesWithDifferentContent()
    {
        var input = new[] { "one", "two", "three" };

        var result = CaseInsensitiveSet.Build(input);

        Assert.Equal(new HashSet<string>(input), result);
    }

    [Theory]
    [InlineData("hello", "HELLO", true)]
    [InlineData("hello", "Hello", true)]
    [InlineData("hello", "world", false)]
    public void Comparer_Equals_IsCaseInsensitive(string a, string b, bool expected)
    {
        var comparer = new CaseInsensitiveComparer();

        Assert.Equal(expected, comparer.Equals(a, b));
    }

    [Fact]
    public void Comparer_GetHashCode_IsSameForDifferentCasing()
    {
        var comparer = new CaseInsensitiveComparer();

        Assert.Equal(comparer.GetHashCode("Test"), comparer.GetHashCode("test"));
        Assert.Equal(comparer.GetHashCode("Test"), comparer.GetHashCode("TEST"));
    }

    [Fact]
    public void Build_ResultContainsLookupIgnoringCase()
    {
        var input = new[] { "Alpha", "Beta" };

        var result = CaseInsensitiveSet.Build(input);

        Assert.Contains("ALPHA", result, new CaseInsensitiveComparer());
        Assert.Contains("beta", result, new CaseInsensitiveComparer());
    }
}
