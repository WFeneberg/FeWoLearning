using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex007_EnvironmentLiteralsAndCallbacksTests
{
    /// <summary>
    /// Runs every environment callback the resource carries under a context for the
    /// given operation, and hands back what they wrote.
    ///
    /// This detour exists because the annotation TYPES cannot separate a literal from
    /// a callback. Measured on Aspire 13.5.3: WithEnvironment(name, value) writes an
    /// internal EnvironmentAnnotation which DERIVES from EnvironmentCallbackAnnotation,
    /// so OfType&lt;EnvironmentCallbackAnnotation&gt;() counts literals too - an
    /// all-literal worker and an all-callback worker both report the same count. Any
    /// fact built on that count would grade nothing.
    /// </summary>
    private static async Task<Dictionary<string, object>> RunEnvironmentCallbacksAsync(
        IResource resource, DistributedApplicationOperation operation, CancellationToken ct)
    {
        var values = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(operation), resource, values, ct);
        foreach (var annotation in resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            await annotation.Callback(context);
        }
        return values;
    }

    [Fact]
    public async Task A_literal_is_the_same_string_in_every_context()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var worker = model.Resource("worker");
        var ct = TestContext.Current.CancellationToken;

        var run = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Run, ct);
        var publish = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Publish, ct);

        // A literal survives into the environment as a plain string, unchanged, in
        // both operations. Assert.IsType is the load-bearing half: it rejects the
        // learner who "improved" REGION into a callback returning a ReferenceExpression
        // or a parameter, which is a different mechanism than the row asks for.
        Assert.Equal("eu-west", Assert.IsType<string>(run["REGION"]));
        Assert.Equal("eu-west", Assert.IsType<string>(publish["REGION"]));
    }

    [Fact]
    public async Task The_callback_reads_the_other_resources_endpoint()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var ct = TestContext.Current.CancellationToken;

        var values = await RunEnvironmentCallbacksAsync(
            model.Resource("worker"), DistributedApplicationOperation.Run, ct);

        // The mutant: WithEnvironment("API_URL", "http://localhost:8080"). It produces
        // a value that LOOKS right in a local run and is wrong everywhere else, and it
        // lands a System.String here rather than an EndpointReference - so the type
        // assertion, not the value, is what rejects it.
        var endpoint = Assert.IsType<EndpointReference>(values["API_URL"]);

        // Both halves are pinned, because reading the wrong endpoint off the right
        // resource (or the right endpoint off the wrong one) is the near-miss.
        Assert.Equal("api", endpoint.Resource.Name);
        Assert.Equal("http", endpoint.EndpointName);

        // Measured and accepted: WithEnvironment("API_URL", api.GetEndpoint("http")),
        // the non-callback overload, lands the identical EndpointReference and passes
        // this fact. It is an equivalent spelling for THIS variable, not a cheat -
        // and it is why the row needs the fact below as well, which no overload of
        // the literal form can satisfy.
    }

    [Fact]
    public async Task Only_a_callback_can_answer_differently_per_context()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var worker = model.Resource("worker");
        var ct = TestContext.Current.CancellationToken;

        var run = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Run, ct);
        var publish = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Publish, ct);

        // This is the fact no literal can pass, and the reason the row exists. A
        // literal is fixed when the model is built, so it answers "run" in a publish
        // context too; the callback is handed a fresh EnvironmentCallbackContext per
        // evaluation and reads ExecutionContext from it. Measured: the all-literal
        // mutant returns "run" for BOTH lines below and fails on the second.
        Assert.Equal("run", run["MODE"]);
        Assert.Equal("publish", publish["MODE"]);
    }

    [Fact]
    public async Task The_manifest_carries_the_literal_and_the_resolved_reference()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex007_EnvironmentLiteralsAndCallbacks.Configure,
            TestContext.Current.CancellationToken);

        var env = manifest.RootElement
            .GetProperty("resources").GetProperty("worker").GetProperty("env");

        // The literal publishes as itself...
        Assert.Equal("eu-west", env.GetProperty("REGION").GetString());

        // ...while the endpoint publishes as an unresolved expression naming the
        // resource and binding, which is what a deployment target substitutes. The
        // hard-coded-URL mutant publishes "http://localhost:8080" here and fails.
        Assert.Equal("{api.bindings.http.url}", env.GetProperty("API_URL").GetString());

        // And the manifest is generated in publish mode, so the context-sensitive
        // variable proves it end to end rather than only in the hand-built context
        // of the fact above.
        Assert.Equal("publish", env.GetProperty("MODE").GetString());
    }
}
