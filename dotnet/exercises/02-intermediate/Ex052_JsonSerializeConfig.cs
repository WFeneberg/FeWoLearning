namespace FeWoLearning.Exercises.Intermediate;

// Exercise 052 — JsonSerializeConfig (intermediate).
// Goal:   Serialize a simple settings object to a JSON string using
//         System.Text.Json with camelCase property naming and null-value
//         properties omitted from the output.
// Drills: System.Text.Json, JsonSerializerOptions, naming policies,
//         ignoring null values when serializing.
public static class JsonSerializeConfig
{
    public sealed class Settings
    {
        public string? UserName { get; set; }
        public int RetryCount { get; set; }
        public bool IsEnabled { get; set; }
        public string? Description { get; set; }
    }

    public static string Serialize(Settings settings) => throw new NotImplementedException();
}
