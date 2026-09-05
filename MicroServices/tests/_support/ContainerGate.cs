namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// The opt-in for L3 tests that start real database containers.
///
/// FactAttribute.Skip is NOT virtual in xunit.v3 3.2.2, so the usual
/// "custom [ContainerFact] overriding Skip" pattern does not compile (CS0506).
/// Gating happens in the test body via Assert.SkipUnless instead.
/// </summary>
public static class ContainerGate
{
    /// <summary>
    /// Flow-local override, used only by the harness's own canary facts so they can
    /// exercise BOTH directions of the gate without touching process-global state
    /// (an environment variable would leak into every test running in parallel, and
    /// could switch a real container test on by accident).
    /// </summary>
    private static readonly AsyncLocal<bool?> Forced = new();

    public static bool Enabled =>
        Forced.Value
        ?? (Environment.GetEnvironmentVariable("FEWO_MS_CONTAINERS") == "1"
            || AppContext.GetData("FeWoLearning.MicroServices.Containers") as string == "true");

    /// <summary>
    /// Call as the first line of any test that needs a real container.
    ///
    /// Deliberately checks only the switch, never whether Docker is reachable: with
    /// the switch ON and no daemon the test must FAIL, loudly, when Aspire cannot
    /// start the container. A broken Docker setup must not be able to masquerade as
    /// a green run by silently skipping (spec section 5).
    /// </summary>
    public static void Require() =>
        Assert.SkipUnless(Enabled,
            "Container tests are off. Enable with: dotnet test -p:Containers=true");

    /// <summary>
    /// Harness-only: pretend the switch is <paramref name="enabled"/> for the current
    /// asynchronous flow. Exists so the canary facts in <see cref="HarnessMechanicsTests"/>
    /// can prove the gate opens as well as closes. Never call this from an exercise test.
    /// </summary>
    internal static IDisposable Force(bool enabled) => new ForcedScope(enabled);

    private sealed class ForcedScope : IDisposable
    {
        private readonly bool? _previous;
        private bool _disposed;

        internal ForcedScope(bool enabled)
        {
            _previous = Forced.Value;
            Forced.Value = enabled;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Forced.Value = _previous;
        }
    }
}
