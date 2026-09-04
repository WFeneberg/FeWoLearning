// Exercise 061 - Settings Storage (intermediate).
// Goal:   Keep a handful of user preferences across runs.
// Drills: ApplicationData.Current.LocalSettings as a string-keyed bag of WinRT-legal
//         values, the difference between "absent" and "default", and containers for
//         grouping.
// Passes: dotnet test --filter FullyQualifiedName~Ex061_
//
// The bag stores object, so anything compiles and only some things work: a WinRT-legal
// primitive, a string, a date, or an ApplicationDataCompositeValue. Handing it a POCO
// fails at runtime, on the user's machine, on save - which is why serialising to a string
// yourself is the usual answer for anything structured.

using Windows.Storage;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Typed access to a few settings, over the untyped settings bag.
/// </summary>
public sealed class Ex061_SettingsStorage
{
    private readonly ApplicationDataContainer _settings;

    /// <summary>Uses the app's local settings.</summary>
    public Ex061_SettingsStorage()
        : this(ApplicationData.Current.LocalSettings)
    {
    }

    /// <summary>
    /// Uses a specific container - which is how a test gets its own namespace instead of
    /// fighting over the real app's settings.
    /// </summary>
    public Ex061_SettingsStorage(ApplicationDataContainer settings) => _settings = settings;

    /// <summary>
    /// Reads an int setting, or <paramref name="fallback"/> when the key was never written.
    /// </summary>
    public int GetInt(string key, int fallback) =>
        // Both halves matter: the key may be absent, and what is under it may not be an
        // int. A cast would throw on a user's machine, at startup, because a previous
        // version of the app wrote a string here.
        _settings.Values.TryGetValue(key, out var stored) && stored is int value
            ? value
            : fallback;

    /// <summary>Writes an int setting.</summary>
    public void SetInt(string key, int value) => _settings.Values[key] = value;

    /// <summary>Whether the key has ever been written.</summary>
    // Deliberately not "does it differ from the default": absent and
    // happens-to-equal-the-default are different states, and only one of them should
    // follow a future change of the default.
    public bool Has(string key) => _settings.Values.ContainsKey(key);

    /// <summary>
    /// Forgets a setting, so the next read falls back again. Removing a key that is not
    /// there is a no-op.
    /// </summary>
    public void Forget(string key) => _settings.Values.Remove(key);
}
