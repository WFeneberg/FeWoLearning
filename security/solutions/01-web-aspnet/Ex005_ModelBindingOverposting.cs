using System.Text.Json;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 005 — ModelBindingOverposting (reference solution).
public sealed class Ex005_UserProfile
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsAdministrator { get; set; }
}

public static class Ex005_ModelBindingOverposting
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    // A patch DTO with only the two fields a caller may ever change. Id and
    // IsAdministrator have no member here at all, so no deserializer setting -
    // strict or lenient - can ever bind them: a field the type doesn't declare
    // cannot be overposted into it, which is the whole point of the exercise.
    private sealed class Patch
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }

    public static Ex005_UserProfile Apply(Ex005_UserProfile existing, string requestJson)
    {
        var patch = JsonSerializer.Deserialize<Patch>(requestJson, Options) ?? new Patch();

        return new Ex005_UserProfile
        {
            Id = existing.Id,
            IsAdministrator = existing.IsAdministrator,
            DisplayName = patch.DisplayName ?? existing.DisplayName,
            Email = patch.Email ?? existing.Email,
        };
    }
}
