using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 053 — Json Custom Converter (reference solution).
public readonly struct SimpleDate : IEquatable<SimpleDate>
{
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }

    public SimpleDate(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public bool Equals(SimpleDate other) => Year == other.Year && Month == other.Month && Day == other.Day;

    public override bool Equals(object? obj) => obj is SimpleDate other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Year, Month, Day);
}

public sealed class JsonCustomConverter : JsonConverter<SimpleDate>
{
    private const string Format = "yyyy-MM-dd";

    public override SimpleDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString()
            ?? throw new JsonException("Expected a JSON string for SimpleDate.");

        var parsed = DateTime.ParseExact(text, Format, CultureInfo.InvariantCulture);
        return new SimpleDate(parsed.Year, parsed.Month, parsed.Day);
    }

    public override void Write(Utf8JsonWriter writer, SimpleDate value, JsonSerializerOptions options)
    {
        var text = new DateTime(value.Year, value.Month, value.Day).ToString(Format, CultureInfo.InvariantCulture);
        writer.WriteStringValue(text);
    }
}
