using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex024_SafeLookupTests
{
    private static readonly string[] Items = { "apple", "banana", "cherry", "date" };

    [Fact]
    public void TryFind_ReturnsMatch_WhenFound()
    {
        var result = SafeLookup.TryFind(Items, s => s.StartsWith("c"));

        Assert.Equal("cherry", result);
    }

    [Fact]
    public void TryFind_ReturnsNull_WhenNotFound()
    {
        var result = SafeLookup.TryFind(Items, s => s.StartsWith("z"));

        Assert.Null(result);
    }

    [Fact]
    public void TryFind_ReturnsFirstMatch_WhenMultipleMatch()
    {
        var result = SafeLookup.TryFind(Items, s => s.Length == 6);

        Assert.Equal("banana", result);
    }
}
