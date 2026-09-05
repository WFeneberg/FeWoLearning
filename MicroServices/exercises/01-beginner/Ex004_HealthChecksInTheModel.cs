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
        => throw new NotImplementedException(
            "TODO: add containers 'api' and 'admin' (image nginx), each with an "
            + "http endpoint on container port 8080, then give 'api' an HTTP health "
            + "check on '/healthz' and 'admin' one on '/ready'.");
}
