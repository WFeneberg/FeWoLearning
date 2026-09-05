using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 020 — JsonDepthAndUnknownMembers (reference solution).
public static class Ex020_JsonDepthAndUnknownMembers
{
    private static readonly JsonSerializerOptions Options = new()
    {
        // Comfortably above any legitimate payload this track parses, and
        // comfortably below a nesting depth deep enough to threaten the stack.
        MaxDepth = 16,
        PropertyNameCaseInsensitive = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    public static bool TryParse<T>(string json, out T? value, out string? error)
    {
        try
        {
            value = JsonSerializer.Deserialize<T>(json, Options);
            error = null;
            return true;
        }
        catch (JsonException)
        {
            // A fixed, generic message - never ex.Message, which for an
            // unmapped-member failure names the target type and its path.
            value = default;
            error = "The request body could not be parsed.";
            return false;
        }
    }
}
