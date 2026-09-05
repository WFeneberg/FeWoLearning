using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give one container two endpoints: an internal HTTP one and an
///         externally exposed TCP admin one.
/// Drills: `WithHttpEndpoint` vs `WithEndpoint`; EndpointAnnotation.TargetPort
///         (the port INSIDE the container) vs Port (the port offered to the rest
///         of the model) vs IsExternal (exposed at publish time).
/// Passes: The "gateway" container carries exactly TWO EndpointAnnotations -
///         "http" (scheme http, TargetPort 8080, no fixed Port, not external)
///         and "admin" (scheme tcp, Port 9090, TargetPort 9091, external).
/// </summary>
public static class Ex003_EndpointsAndBindings
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: add a container 'gateway' (image nginx) with an http endpoint "
            + "listening on container port 8080, plus a second endpoint named "
            + "'admin' (scheme tcp) on host port 9090 / container port 9091 that "
            + "is exposed externally.");
}
