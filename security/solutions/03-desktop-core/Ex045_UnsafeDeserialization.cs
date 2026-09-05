using System.Text.Json;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 045 — UnsafeDeserialization (reference solution).
public static class Ex045_UnsafeDeserialization
{
    public static bool TryDeserialize(
        string json,
        IReadOnlyCollection<Type> allowedTypes,
        out object? value,
        out string? rejection)
    {
        value = null;

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            rejection = "The payload is not valid JSON.";
            return false;
        }

        using (document)
        {
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("type", out var typeProperty) ||
                typeProperty.ValueKind != JsonValueKind.String ||
                !root.TryGetProperty("data", out var dataProperty))
            {
                rejection = "The payload is not a recognised envelope.";
                return false;
            }

            var requestedTypeName = typeProperty.GetString();

            // Match against the caller's own allowlist by exact full name.
            // Deliberately never Type.GetType(requestedTypeName): resolving an
            // attacker-supplied string into a live Type is the unsafe
            // primitive itself, and it would let an assembly-qualified name
            // reach the same type through a different, unvetted path even
            // when that type happens to be one of the allowed ones.
            Type? match = null;
            foreach (var candidate in allowedTypes)
            {
                if (candidate.FullName == requestedTypeName)
                {
                    match = candidate;
                    break;
                }
            }

            if (match is null)
            {
                // Fixed, generic message - never requestedTypeName, which is
                // attacker-controlled and must never reach a log or an error
                // surface unescaped.
                rejection = "The payload names a type that is not permitted.";
                return false;
            }

            try
            {
                value = JsonSerializer.Deserialize(dataProperty.GetRawText(), match);
            }
            catch (JsonException)
            {
                rejection = "The payload's data could not be parsed for the requested type.";
                return false;
            }

            rejection = null;
            return true;
        }
    }
}
