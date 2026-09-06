using Aspire.Hosting;
using FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Maps an exercise id to its Configure method, so one AppHost can run any exercise
/// instead of the repo needing 100 executable AppHost projects.
/// Add one line per exercise, in the same commit as the exercise.
/// </summary>
public static class ExerciseRegistry
{
    private static readonly Dictionary<string, Action<IDistributedApplicationBuilder>> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ex001"] = Ex001_ContainerResourceBasics.Configure,
        ["ex002"] = Ex002_ReferenceVersusWaitFor.Configure,
        ["ex003"] = Ex003_EndpointsAndBindings.Configure,
        ["ex004"] = Ex004_HealthChecksInTheModel.Configure,
        ["ex005"] = Ex005_ParametersAndSecrets.Configure,
        ["ex006"] = Ex006_ImageRegistryTagAndDigest.Configure,
        ["ex007"] = Ex007_EnvironmentLiteralsAndCallbacks.Configure,
        ["ex008"] = Ex008_ContainerArgsAndEntrypoint.Configure,
        ["ex009"] = Ex009_VolumesAndBindMounts.Configure,
        ["ex010"] = Ex010_ContainerLifetime.Configure,
        ["ex011"] = Ex011_ProjectResources.Configure,
        ["ex012"] = Ex012_ExecutableResources.Configure,
        ["ex013"] = Ex013_ConnectionStringResources.Configure,
        ["ex014"] = Ex014_ParentAndChildResources.Configure,
        ["ex015"] = Ex015_WaitForCompletion.Configure,
    };

    public static Action<IDistributedApplicationBuilder>? Lookup(string id)
        => Map.TryGetValue(id, out var configure) ? configure : null;

    public static IEnumerable<string> Known => Map.Keys.Order();
}
