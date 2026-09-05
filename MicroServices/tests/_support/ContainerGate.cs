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
    public static bool Enabled =>
        Environment.GetEnvironmentVariable("FEWO_MS_CONTAINERS") == "1"
        || AppContext.GetData("FeWoLearning.MicroServices.Containers") as string == "true";

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
}
