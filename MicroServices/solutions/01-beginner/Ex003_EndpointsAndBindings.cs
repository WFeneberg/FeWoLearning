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
    {
        builder.AddContainer("gateway", "nginx")
               // targetPort is what the process inside the container listens on.
               // Leaving port null lets Aspire allocate the host-side port, which is
               // the normal case - pinning it is the exception, not the default.
               // The endpoint name defaults to "http" here.
               .WithHttpEndpoint(targetPort: 8080)
               // WithEndpoint is the general form: any scheme, an explicit name, and
               // isExternal, which WithHttpEndpoint does not expose. A distinct name
               // is what makes this a SECOND annotation rather than an update of the
               // first one.
               .WithEndpoint(port: 9090, targetPort: 9091, scheme: "tcp",
                             name: "admin", isExternal: true);
    }
}
