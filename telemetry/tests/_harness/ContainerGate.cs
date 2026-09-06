namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Gates the container-backed facts. Call it as the FIRST statement of any fact that
/// needs Docker; everything after the call is skipped unless the run passed
/// <c>-p:Containers=true</c>.
///
/// It is a call and not a custom <c>[ContainerFact]</c> attribute because
/// <c>FactAttribute.Skip</c> is not virtual in xunit.v3 3.2.2 - overriding it fails
/// with CS0506.
///
/// The MSBuild property reaches the test process through a
/// RuntimeHostConfigurationOption in the .csproj; an MSBuild property is otherwise
/// invisible at runtime.
/// </summary>
public static class ContainerGate
{
    private const string Key = "FeWoLearning.Telemetry.Containers";

    public static bool Enabled =>
        bool.TryParse(AppContext.GetData(Key) as string, out var on) && on;

    public static void SkipUnlessEnabled() =>
        Assert.SkipUnless(Enabled, "Container-backed fact. Re-run with -p:Containers=true.");
}
