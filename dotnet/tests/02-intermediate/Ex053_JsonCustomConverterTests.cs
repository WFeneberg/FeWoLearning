using System.Text.Json;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex053_JsonCustomConverterTests
{
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonCustomConverter());
        return options;
    }

    [Fact]
    public void Write_ProducesIsoDateString()
    {
        var options = CreateOptions();
        var date = new SimpleDate(2024, 3, 7);

        var json = JsonSerializer.Serialize(date, options);

        Assert.Equal("\"2024-03-07\"", json);
    }

    [Fact]
    public void RoundTrip_SerializeThenDeserialize_YieldsEqualValue()
    {
        var options = CreateOptions();
        var original = new SimpleDate(1999, 12, 31);

        var json = JsonSerializer.Serialize(original, options);
        var roundTripped = JsonSerializer.Deserialize<SimpleDate>(json, options);

        Assert.Equal(original, roundTripped);
        Assert.Equal(original.Year, roundTripped.Year);
        Assert.Equal(original.Month, roundTripped.Month);
        Assert.Equal(original.Day, roundTripped.Day);
    }

    [Fact]
    public void Read_ParsesIsoDateString()
    {
        var options = CreateOptions();

        var result = JsonSerializer.Deserialize<SimpleDate>("\"2020-01-05\"", options);

        Assert.Equal(new SimpleDate(2020, 1, 5), result);
    }

    [Fact]
    public void RoundTrip_WithinObject_PreservesValue()
    {
        var options = CreateOptions();
        var original = new List<SimpleDate> { new(2000, 6, 15), new(2023, 11, 1) };

        var json = JsonSerializer.Serialize(original, options);
        var roundTripped = JsonSerializer.Deserialize<List<SimpleDate>>(json, options);

        Assert.NotNull(roundTripped);
        Assert.Equal(original, roundTripped!);
    }
}
