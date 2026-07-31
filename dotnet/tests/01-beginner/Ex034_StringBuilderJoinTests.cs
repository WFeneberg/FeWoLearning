using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex034_StringBuilderJoinTests
{
    [Fact]
    public void BuildCsvLine_JoinsPlainFieldsWithCommas()
    {
        var fields = new[] { "Alice", "42", "Berlin" };

        Assert.Equal("Alice,42,Berlin", StringBuilderJoin.BuildCsvLine(fields));
    }

    [Fact]
    public void BuildCsvLine_EscapesFieldsContainingCommas()
    {
        var fields = new[] { "a", "b,c", "d" };

        Assert.Equal("a,\"b,c\",d", StringBuilderJoin.BuildCsvLine(fields));
    }

    [Fact]
    public void BuildCsvLine_ReturnsEmptyStringForEmptyArray()
    {
        Assert.Equal(string.Empty, StringBuilderJoin.BuildCsvLine(Array.Empty<string>()));
    }
}
