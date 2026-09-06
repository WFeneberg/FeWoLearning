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
        ["ex016"] = Ex016_CustomAnnotationsAndExtensions.Configure,
        ["ex017"] = Ex017_DashboardUrls.Configure,
        ["ex018"] = Ex018_ReplicasAndEndpointAllocation.Configure,
        ["ex019"] = Ex019_ExcludeFromManifest.Configure,
        ["ex020"] = Ex020_RunVersusPublishMode.Configure,
        ["ex024"] = Ex024_ResourceCommands.Configure,
        ["ex025"] = Ex025_EventingAndLifecycleHooks.Configure,
        ["ex026"] = Ex026_SqlServerFirstConnection.Configure,
        ["ex027"] = Ex027_PostgresFirstConnection.Configure,
        ["ex028"] = Ex028_MongoFirstConnection.Configure,
        ["ex029"] = Ex029_RedisFirstConnection.Configure,
        ["ex030"] = Ex030_DatabaseAdminTools.Configure,
    };

    /// <summary>
    /// Exercises that are real rows in catalog.md but have NO AppHost model to run: they
    /// are service-side, so their whole subject lives in an IServiceCollection or an
    /// endpoint and there is nothing for the dashboard to show. They are listed here
    /// rather than left out, so `--exercise ex021` says why instead of "unknown".
    /// </summary>
    private static readonly Dictionary<string, string> WithoutAModel = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ex021"] = "ex021 (ServiceDefaults) is service-side: it registers services in an "
                    + "IHostApplicationBuilder, and has no resource graph. Run `dotnet test "
                    + "--filter FullyQualifiedName~Ex021_` instead.",
        ["ex022"] = "ex022 (OpenTelemetryRegistration) is service-side: it registers a custom "
                    + "ActivitySource and Meter with the OpenTelemetry providers, and has no "
                    + "resource graph. Run `dotnet test --filter FullyQualifiedName~Ex022_` instead.",
        ["ex023"] = "ex023 (LivenessVersusReadiness) is service-side: it registers health checks "
                    + "and maps two probe endpoints, and has no resource graph. Run `dotnet test "
                    + "--filter FullyQualifiedName~Ex023_` instead.",
    };

    /// <summary>Why an exercise cannot be run in the playground, or null if it can.</summary>
    public static string? NoModelReason(string id)
        => WithoutAModel.TryGetValue(id, out var reason) ? reason : null;

    public static Action<IDistributedApplicationBuilder>? Lookup(string id)
        => Map.TryGetValue(id, out var configure) ? configure : null;

    public static IEnumerable<string> Known => Map.Keys.Concat(WithoutAModel.Keys).Order();
}
