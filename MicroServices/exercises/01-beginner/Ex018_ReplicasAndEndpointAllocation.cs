using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Run one service as three instances behind a single address, and a second,
///         deliberately single-instance service on a port that is pinned because a
///         tool outside the model dials it by number.
/// Drills: `WithReplicas`, ReplicaAnnotation, and what scaling does to endpoint
///         allocation. Three instances cannot all own one host port, so a replicated
///         service must leave EndpointAnnotation.Port null and let Aspire's PROXY own
///         the address and fan connections out. Pinning a port is only safe on the
///         resource that stays at one instance - and it is safe there even with the
///         proxy switched off.
/// Passes: "web" carries a ReplicaAnnotation with Replicas 3, and its single endpoint
///         has Port null, TargetPort 8080 and IsProxied true; "admin" carries NO
///         ReplicaAnnotation, and its single endpoint pins Port 5099 with IsProxied
///         false. Both pass launchProfileName: null, so the endpoints above are the
///         ones this file declared and not whatever launchSettings.json happens to
///         say.
/// Note:   The catalog row calls a fixed host port and replicas "contradictory".
///         Measured on 13.5.3, Aspire itself does NOT agree: WithReplicas(3) together
///         with a fixed Port neither throws nor warns, because a proxied endpoint has
///         exactly one listener - the proxy - in front of N instances. The genuine
///         contradiction is a fixed port on a PROXYLESS endpoint, where each instance
///         would have to bind the port itself; Aspire does not detect that either.
///         The one replica combination it does reject is a persistent container
///         lifetime ("uses multiple replicas and a persistent lifetime. These
///         features do not work together"). So this row grades the SHAPE of the
///         model, which is the thing that is actually checkable at L1.
/// Trap:   Leave launchProfileName off and the launch profile supplies the endpoint
///         instead - measured, "catalog" then arrives with a FIXED Port 5080 and a
///         null TargetPort, and WithReplicas(3) scales a service whose host port was
///         nailed down by a file nobody looked at.
/// </summary>
public static class Ex018_ReplicasAndEndpointAllocation
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex018 - add the Catalog project as \"web\" with no launch profile, "
            + "give it an http endpoint named \"http\" on target port 8080 with no "
            + "host port, and run three instances of it; add the Orders project as "
            + "\"admin\" with no launch profile and a single proxyless http endpoint "
            + "named \"http\" pinned to host port 5099 and target port 5099. Use "
            + "ServiceProject below for both project paths.");

    /// <summary>
    /// GIVEN, not a TODO. The absolute path of one of the track's shared services'
    /// project files, found by walking up from whatever directory the current host
    /// happens to be running in until the track's .slnx turns up. See
    /// MicroServices/README.md section 5 before changing this.
    /// </summary>
    internal static string ServiceProject(IDistributedApplicationBuilder builder, string name)
    {
        var dir = new DirectoryInfo(builder.AppHostDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "FeWoLearning.MicroServices.slnx")))
        {
            dir = dir.Parent;
        }

        var root = dir?.FullName
                   ?? throw new InvalidOperationException(
                       $"'{builder.AppHostDirectory}' is not inside MicroServices/.");

        return Path.Combine(root, "services", name, $"{name}.csproj");
    }
}
