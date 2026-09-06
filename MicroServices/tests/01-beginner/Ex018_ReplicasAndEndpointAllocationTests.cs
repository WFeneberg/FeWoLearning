using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex018_ReplicasAndEndpointAllocationTests
{
    private static EndpointAnnotation OnlyEndpointOf(IResource resource)
        => Assert.Single(resource.Annotations.OfType<EndpointAnnotation>());

    [Fact]
    public void Only_the_service_that_scales_carries_a_replica_annotation()
    {
        var model = ModelHarness.Build(Ex018_ReplicasAndEndpointAllocation.Configure);

        // Both have to be project resources, and not because the row is about
        // projects: WithReplicas is declared on IResourceBuilder<ProjectResource>
        // alone on 13.5.3, so there is no container spelling of this exercise.
        var web = Assert.IsType<ProjectResource>(model.Resource("web"));
        var admin = Assert.IsType<ProjectResource>(model.Resource("admin"));

        var replicas = Assert.Single(web.Annotations.OfType<ReplicaAnnotation>());
        Assert.Equal(3, replicas.Replicas);

        // The absence is half the grade. WithReplicas(3) on both looks like "the
        // model scales" and quietly puts three instances behind the pinned,
        // proxyless port below - the exact arrangement the row is about.
        Assert.Empty(admin.Annotations.OfType<ReplicaAnnotation>());
    }

    [Fact]
    public void The_replicated_service_leaves_its_host_port_to_the_proxy()
    {
        var model = ModelHarness.Build(Ex018_ReplicasAndEndpointAllocation.Configure);
        var web = model.Resource("web");

        // Assert.Single, not Assert.Contains: an extra endpoint means the launch
        // profile got a say. launchProfileName: null is what stops that, and its
        // trace in the model is this annotation - measured, omitting it yields an
        // http endpoint with a FIXED Port 5080 and a null TargetPort, which passes a
        // "there is an http endpoint" test and fails the whole point of the row.
        Assert.Single(web.Annotations.OfType<ExcludeLaunchProfileAnnotation>());
        var endpoint = OnlyEndpointOf(web);

        // The grading fact. Three instances cannot each own one host port, so the
        // replicated service must not name one; Aspire allocates a single proxy
        // address instead. WithHttpEndpoint(port: 5099, targetPort: 8080) alongside
        // WithReplicas(3) is the plausible wrong answer, and it is wrong quietly -
        // measured on 13.5.3 it neither throws nor warns.
        Assert.Null(endpoint.Port);
        Assert.Equal(8080, endpoint.TargetPort);

        // ... and the proxy has to actually be in the picture. isProxied: false here
        // would leave three processes fighting for one address with nothing in front
        // of them.
        Assert.True(endpoint.IsProxied);
    }

    [Fact]
    public void The_single_instance_service_is_the_only_one_allowed_to_pin_a_port()
    {
        var model = ModelHarness.Build(Ex018_ReplicasAndEndpointAllocation.Configure);
        var admin = model.Resource("admin");

        Assert.Single(admin.Annotations.OfType<ExcludeLaunchProfileAnnotation>());
        var endpoint = OnlyEndpointOf(admin);

        // Without this fact the row would be satisfied by never pinning a port
        // anywhere, which teaches nothing: the claim is that a fixed, proxyless port
        // is a choice you make BECAUSE the resource stays at one instance, not a
        // thing to avoid.
        Assert.Equal(5099, endpoint.Port);
        Assert.Equal(5099, endpoint.TargetPort);
        Assert.False(endpoint.IsProxied);
    }
}
