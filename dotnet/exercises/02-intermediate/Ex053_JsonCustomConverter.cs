using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 053 — Json Custom Converter (intermediate).
// Goal:   Implement a custom JsonConverter<T> for a simple date-only value type
//         (SimpleDate), serializing it as a "yyyy-MM-dd" JSON string, and
//         reading it back into an equal SimpleDate on deserialize.
// Drills: System.Text.Json, JsonConverter<T>, Read/Write overrides,
//         JsonSerializerOptions composition, value-type equality.

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

    public bool Equals(SimpleDate other) => throw new NotImplementedException();

    public override bool Equals(object? obj) => obj is SimpleDate other && Equals(other);

    public override int GetHashCode() => throw new NotImplementedException();
}

public sealed class JsonCustomConverter : JsonConverter<SimpleDate>
{
    public override SimpleDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, SimpleDate value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}
