// Exercise 070 - Capability Probe (intermediate).
// Goal:   Ask what the device can do, not what it is called.
// Drills: a capability registry over named probes, caching a probe's answer, and a
//         fallback for a capability nobody registered.
// Passes: dotnet test --filter FullyQualifiedName~Ex070_
//
// "if (platform == android)" ages badly: the next platform, or the next version of this
// one, breaks it silently. "if (Supports("camera"))" keeps working, and it is also what
// ApiInformation.IsTypePresent is for on WinUI - the same idea, one level down.
//
// Caching matters more than it looks: a probe can be expensive (a permission check, a
// hardware query), and the UI asks the same question on every layout pass.

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Answers capability questions from probes registered once at startup.
/// </summary>
public sealed class Ex070_CapabilityProbe
{
    private readonly Dictionary<string, Func<bool>> _probes = [];
    private readonly Dictionary<string, bool> _answers = [];

    /// <summary>How many times each probe has actually been run, by name.</summary>
    public Dictionary<string, int> ProbeCalls { get; } = [];

    /// <summary>
    /// Registers <paramref name="probe"/> under <paramref name="capability"/>, replacing any
    /// previous registration and forgetting any cached answer for it.
    /// </summary>
    public void Register(string capability, Func<bool> probe)
    {
        _probes[capability] = probe;

        // A new probe invalidates the old answer, or re-registering would be a no-op that
        // looks like it worked.
        _answers.Remove(capability);
    }

    /// <summary>
    /// Whether <paramref name="capability"/> is available. An unregistered capability is
    /// false - unknown means "do not offer it", never "assume it works". A registered probe
    /// runs at most once.
    /// </summary>
    public bool Supports(string capability)
    {
        if (_answers.TryGetValue(capability, out var cached))
        {
            return cached;
        }

        if (!_probes.TryGetValue(capability, out var probe))
        {
            // Unknown means "do not offer it". Defaulting to true shows a button that
            // fails when pressed, on exactly the platforms nobody tested.
            return false;
        }

        var answer = probe();

        ProbeCalls[capability] = ProbeCalls.GetValueOrDefault(capability) + 1;

        // Both answers are cached. Caching only the true ones is the easy bug: a false
        // answer then re-runs an expensive probe on every query.
        _answers[capability] = answer;
        return answer;
    }

    /// <summary>
    /// Forgets every cached answer, so the probes run again - for after a permission
    /// prompt, where the answer legitimately changes.
    /// </summary>
    // The probes stay registered; only their answers go. After a permission prompt the
    // answer legitimately changes, and a cache with no way to forget is worse than none.
    public void Invalidate() => _answers.Clear();
}
