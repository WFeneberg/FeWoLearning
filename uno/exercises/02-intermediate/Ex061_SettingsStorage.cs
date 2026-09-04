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
        // TODO: TryGetValue, and only accept it when the stored value really is an int -
        // the bag holds object, and a previous version of the app may have written a
        // string under the same key.
        throw new NotImplementedException("TODO: Ex061 - read an int setting");

    /// <summary>Writes an int setting.</summary>
    public void SetInt(string key, int value) =>
        throw new NotImplementedException("TODO: Ex061 - write an int setting");

    /// <summary>Whether the key has ever been written.</summary>
    public bool Has(string key) =>
        throw new NotImplementedException("TODO: Ex061 - is the key present?");

    /// <summary>
    /// Forgets a setting, so the next read falls back again. Removing a key that is not
    /// there is a no-op.
    /// </summary>
    public void Forget(string key) =>
        throw new NotImplementedException("TODO: Ex061 - remove the setting");
}
