using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex043_SpanSliceParserTests
{
    [Fact]
    public void ParseInts_ParsesCommaSeparatedValues()
    {
        var result = SpanSliceParser.ParseInts("10,-3,42,0,7");

        Assert.Equal(new[] { 10, -3, 42, 0, 7 }, result);
    }

    [Fact]
    public void ParseInts_SingleValue_ReturnsSingleElementArray()
    {
        var result = SpanSliceParser.ParseInts("99");

        Assert.Equal(new[] { 99 }, result);
    }

    [Fact]
    public void ParseInts_EmptyString_ReturnsEmptyArray()
    {
        var result = SpanSliceParser.ParseInts("");

        Assert.Empty(result);
    }

    [Fact]
    public void ParseInts_ToleratesSpacesAroundValues()
    {
        var result = SpanSliceParser.ParseInts(" 1, 2 ,3 ,  -4 ");

        Assert.Equal(new[] { 1, 2, 3, -4 }, result);
    }
}
