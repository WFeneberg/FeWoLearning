namespace FeWoLearning.Architecture.Exercises.Scale.Ex073;

// Exercise 073 — TenantConfiguration (reference solution).
public sealed class TenantSettings(
    IReadOnlyDictionary<string, string?> defaults,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, string?>> perTenant)
{
    public string? Get(string tenantId, string key) =>
        Overrides(tenantId).TryGetValue(key, out var tenantValue)
            ? tenantValue
            : defaults.GetValueOrDefault(key);

    public string? SourceOf(string tenantId, string key)
    {
        if (Overrides(tenantId).ContainsKey(key)) return "tenant";
        if (defaults.ContainsKey(key)) return "default";
        return null;
    }

    public IReadOnlyDictionary<string, string?> Effective(string tenantId)
    {
        // Start from the defaults and lay the overrides ON TOP. Returning the tenant's own
        // dictionary is the natural mistake, and its failure is invisible at the point of
        // the bug: Get() keeps working perfectly, one key at a time, while Effective()
        // quietly hands back three keys for a tenant with forty settings and whatever
        // consumes it uses ITS defaults for the other thirty-seven. Nothing throws, every
        // value is plausible, and it surfaces weeks later as one customer behaving oddly.
        var effective = new Dictionary<string, string?>(defaults, StringComparer.Ordinal);

        foreach (var (key, value) in Overrides(tenantId))
            effective[key] = value;

        return effective;
    }

    /// <summary>
    /// An unknown tenant has no overrides - it is not an error. A new customer has to work
    /// before anybody has touched anything.
    /// </summary>
    private IReadOnlyDictionary<string, string?> Overrides(string tenantId) =>
        perTenant.GetValueOrDefault(tenantId) ?? new Dictionary<string, string?>();
}
