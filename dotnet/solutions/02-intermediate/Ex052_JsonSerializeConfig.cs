using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 052 — JsonSerializeConfig (reference solution).
public static class JsonSerializeConfig
{
    public sealed class Settings
    {
        public string? UserName { get; set; }
        public int RetryCount { get; set; }
        public bool IsEnabled { get; set; }
        public string? Description { get; set; }
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string Serialize(Settings settings) => JsonSerializer.Serialize(settings, Options);
}
