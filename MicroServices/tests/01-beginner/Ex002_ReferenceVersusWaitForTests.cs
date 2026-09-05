using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex002_ReferenceVersusWaitForTests
{
    [Fact]
    public void Worker_waits_for_the_database()
    {
        var model = ModelHarness.Build(Ex002_ReferenceVersusWaitFor.Configure);

        // WithReference alone injects configuration but does NOT order startup.
        // Only WaitFor leaves a WaitAnnotation, so this is the fact that separates
        // the two - a solution using only WithReference must fail here. The
        // annotation must name "orders": waiting for the SERVER is not the same
        // promise as waiting for the database on it.
        var waits = model.Resource("worker").Annotations.OfType<WaitAnnotation>().ToList();
        Assert.Contains(waits, w => w.Resource.Name == "orders");
    }

    [Fact]
    public void Worker_carries_a_reference_annotation_pair()
    {
        var model = ModelHarness.Build(Ex002_ReferenceVersusWaitFor.Configure);
        var worker = model.Resource("worker");

        // WithReference writes an EnvironmentCallbackAnnotation (the config
        // injection) AND a Reference relationship; AddContainer on its own writes
        // neither, so a WaitFor-only solution fails both assertions.
        //
        // Measured caveat, recorded so nobody re-derives it: hand-rolling
        // WithEnvironment("ConnectionStrings__orders", orders) produces the SAME two
        // annotations and the same manifest entry. That spelling is therefore
        // model-indistinguishable from WithReference here and is accepted - it is an
        // equivalent, not a cheat. What is rejected is omitting the injection
        // altogether, or injecting from "pg" instead of "orders".
        Assert.NotEmpty(worker.Annotations.OfType<EnvironmentCallbackAnnotation>());
        Assert.Contains(
            worker.Annotations.OfType<ResourceRelationshipAnnotation>(),
            r => r.Type == "Reference" && r.Resource.Name == "orders");
    }

    [Fact]
    public async Task Worker_receives_the_connection_string()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex002_ReferenceVersusWaitFor.Configure,
            TestContext.Current.CancellationToken);

        var env = manifest.RootElement
            .GetProperty("resources").GetProperty("worker").GetProperty("env");

        // Conversely, WaitFor alone orders startup but injects nothing, so this is
        // the fact that a WaitFor-only solution fails. The expected value also pins
        // WHICH resource was referenced: referencing "pg" emits
        // ConnectionStrings__pg instead.
        Assert.Equal(
            "{orders.connectionString}",
            env.GetProperty("ConnectionStrings__orders").GetString());
    }
}
