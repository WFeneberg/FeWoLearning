namespace FeWoLearning.Architecture.Exercises.Scale.Ex073;

// Exercise 073 — TenantConfiguration (scale).
// Goal:   Let a tenant override some settings without having to restate the ones it does
//         not care about.
// Drills: per-tenant overrides, inheritance, unknown tenants, merge vs replace.
// Passes: override  - a tenant's own value wins over the default.
//         fallback  - a key the tenant does not mention falls through to the default.
//         unknown   - a tenant nobody has configured gets the defaults, not an exception.
//                     A new customer must work before anybody has touched anything.
//         THE ONE    - Effective(tenant) MERGES: it contains every default key plus the
//                     tenant's overrides. Returning the tenant's own dictionary is the
//                     natural mistake and silently drops every setting they did not
//                     override - which is most of them.
//         isolation - one tenant's overrides are invisible to another.
//
// The merge is the fact worth the exercise because the failure it prevents is invisible
// at the point of the bug. Get() works perfectly - one key at a time, falling back
// correctly - while Effective() quietly returns a three-key dictionary for a tenant with
// forty settings, and whatever consumes it uses ITS defaults for the other thirty-seven.
// The values are all plausible, nothing throws, and the difference shows up weeks later
// as one customer behaving oddly.
public sealed class TenantSettings(
    IReadOnlyDictionary<string, string?> defaults,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, string?>> perTenant)
{
    /// <summary>The tenant's value if it has one, otherwise the default, otherwise null.</summary>
    public string? Get(string tenantId, string key) =>
        throw new NotImplementedException("TODO: Ex073 - the tenant's value, falling back to the default");

    /// <summary>"tenant", "default", or null when nothing defines the key.</summary>
    public string? SourceOf(string tenantId, string key) =>
        throw new NotImplementedException("TODO: Ex073 - which layer supplied the effective value");

    /// <summary>Every setting that applies to this tenant: the defaults, overridden.</summary>
    public IReadOnlyDictionary<string, string?> Effective(string tenantId) =>
        throw new NotImplementedException(
            "TODO: Ex073 - start from the defaults and lay the tenant's overrides on top, rather than returning either one");
}
