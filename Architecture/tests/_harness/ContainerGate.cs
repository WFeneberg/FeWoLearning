namespace FeWoLearning.Architecture.Tests.Harness;

/// <summary>
/// Gates the container-backed facts. FactAttribute.Skip is not virtual in
/// xunit.v3 3.2.2, so the idiomatic custom [ContainerFact] overriding it fails
/// CS0506 - the gate has to be a call in the test body instead. The MSBuild
/// property reaches the test process through runtimeconfig.json, because an
/// MSBuild property is otherwise invisible at runtime.
/// </summary>
public static class ContainerGate
{
    public static bool Enabled { get; } =
        Environment.GetEnvironmentVariable("FEWO_ARCH_CONTAINERS") == "1"
        || AppContext.GetData("FeWoLearning.Architecture.Containers") as string == "true";

    public static void SkipUnlessEnabled() =>
        Assert.SkipUnless(Enabled,
            "Container tests are off. Enable with: dotnet test -p:Containers=true");
}
