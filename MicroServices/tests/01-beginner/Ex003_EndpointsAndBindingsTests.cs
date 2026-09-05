using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex003_EndpointsAndBindingsTests
{
    private static IReadOnlyList<EndpointAnnotation> Endpoints()
        => ModelHarness.Build(Ex003_EndpointsAndBindings.Configure)
            .Resource("gateway")
            .Annotations.OfType<EndpointAnnotation>()
            .ToList();

    [Fact]
    public void Two_endpoints_stay_two_annotations()
    {
        // The trap this rejects: WithHttpEndpoint and WithEndpoint UPDATE an
        // existing endpoint of the same name rather than adding a second one, so
        // calling them twice without distinct names silently leaves ONE annotation
        // and the "two endpoints" claim is false. Names are asserted too, because
        // two annotations called "http" and "http2" are not the ones asked for.
        var endpoints = Endpoints();

        Assert.Equal(2, endpoints.Count);
        Assert.Equal(
            new[] { "admin", "http" },
            endpoints.Select(e => e.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void Http_endpoint_binds_the_container_port_only()
    {
        var http = Assert.Single(Endpoints(), e => e.Name == "http");

        // TargetPort is the port the process inside the container listens on; Port
        // is a fixed host-side port. Passing 8080 as `port` instead of `targetPort`
        // is the classic mistake and is rejected here by BOTH halves: TargetPort
        // would be null and Port would be 8080.
        Assert.Equal(8080, http.TargetPort);
        Assert.Null(http.Port);
        Assert.Equal("http", http.UriScheme);

        // Not externally exposed: only the admin endpoint is.
        Assert.False(http.IsExternal);
    }

    [Fact]
    public void Admin_endpoint_is_external_and_pins_both_ports()
    {
        var admin = Assert.Single(Endpoints(), e => e.Name == "admin");

        Assert.Equal(9090, admin.Port);
        Assert.Equal(9091, admin.TargetPort);
        Assert.Equal("tcp", admin.UriScheme);

        // IsExternal is publish-time exposure and is independent of the ports - a
        // solution that only set the ports, or that marked BOTH endpoints external,
        // fails here or in the http test.
        Assert.True(admin.IsExternal);
    }
}
