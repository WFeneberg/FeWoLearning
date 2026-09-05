using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Attach HTTP health checks to two containers, each probing its own path.
/// Drills: `WithHttpHealthCheck`, and the HealthCheckAnnotation it writes onto the
///         resource. The annotation key encodes resource, endpoint, path and
///         expected status code - so the path is part of the model, not a detail
///         that only shows up at runtime.
/// Passes: "api" carries exactly one HealthCheckAnnotation whose key names
///         "/healthz", "admin" exactly one whose key names "/ready", and neither
///         key mentions the other's path.
/// </summary>
public static class Ex004_HealthChecksInTheModel
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        // WithHttpHealthCheck derives its base address from an endpoint, so the
        // endpoint has to exist first. The annotation it writes is pure model data -
        // nothing polls anything until the app actually runs.
        builder.AddContainer("api", "nginx")
               .WithHttpEndpoint(targetPort: 8080)
               .WithHttpHealthCheck("/healthz");

        builder.AddContainer("admin", "nginx")
               .WithHttpEndpoint(targetPort: 8080)
               .WithHttpHealthCheck("/ready");
    }
}
