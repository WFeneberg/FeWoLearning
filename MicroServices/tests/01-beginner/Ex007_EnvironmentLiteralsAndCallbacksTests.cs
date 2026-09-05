using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex007_EnvironmentLiteralsAndCallbacksTests
{
    /// <summary>
    /// Which mechanism produced an environment variable. Measured on Aspire 13.5.3 by
    /// dumping the annotation types a built model carries:
    ///
    ///   WithEnvironment(name, "literal")          -> EnvironmentAnnotation          (DERIVED)
    ///   WithEnvironment(callback)                 -> EnvironmentCallbackAnnotation  (EXACT)
    ///   WithEnvironment(name, EndpointReference)  -> EnvironmentCallbackAnnotation  (EXACT)
    ///
    /// EnvironmentAnnotation - the annotation a LITERAL writes - DERIVES from
    /// EnvironmentCallbackAnnotation and is internal, so a test can neither name it nor
    /// count its way to the distinction: OfType&lt;EnvironmentCallbackAnnotation&gt;()
    /// collects both, and an all-literal worker and an all-callback one report the same
    /// count. An exact-type check is the one thing that separates them.
    ///
    /// Note the third line: the EndpointReference overload is deferred too, and lands in
    /// the same group as a callback. That is correct, not a leak - the partition is
    /// "value fixed when the model was built" versus "value computed later", which is
    /// exactly the distinction this row teaches.
    /// </summary>
    private enum Kind
    {
        /// <summary>Value baked in at model-build time.</summary>
        Literal,

        /// <summary>Value computed later, from an EnvironmentCallbackContext.</summary>
        Deferred
    }

    private static Kind KindOf(EnvironmentCallbackAnnotation annotation)
        => annotation.GetType() == typeof(EnvironmentCallbackAnnotation) ? Kind.Deferred : Kind.Literal;

    /// <summary>
    /// Runs the resource's environment callbacks under a context for the given
    /// operation and hands back what they wrote. With <paramref name="only"/> set, runs
    /// just one mechanism's annotations, so a fact can ask which mechanism produced
    /// which variable rather than only what the merged result looks like.
    /// </summary>
    private static async Task<Dictionary<string, object>> RunEnvironmentCallbacksAsync(
        IResource resource,
        DistributedApplicationOperation operation,
        CancellationToken ct,
        Kind? only = null)
    {
        var values = new Dictionary<string, object>();
        var context = new EnvironmentCallbackContext(
            new DistributedApplicationExecutionContext(operation), resource, values, ct);
        foreach (var annotation in resource.Annotations.OfType<EnvironmentCallbackAnnotation>()
                     .Where(a => only is null || KindOf(a) == only))
        {
            await annotation.Callback(context);
        }
        return values;
    }

    [Fact]
    public async Task Each_variable_comes_from_the_mechanism_the_row_asks_for()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var worker = model.Resource("worker");
        var ct = TestContext.Current.CancellationToken;

        var literals = await RunEnvironmentCallbacksAsync(
            worker, DistributedApplicationOperation.Run, ct, only: Kind.Literal);
        var deferred = await RunEnvironmentCallbacksAsync(
            worker, DistributedApplicationOperation.Run, ct, only: Kind.Deferred);

        // The row's subject is literal VERSUS callback, so it has to be graded in both
        // directions, and running the two groups separately is what makes that possible.
        //
        // Rejecting a callback that does a literal's job is the direction that is easy
        // to miss: an implementation using NO literal overload at all, writing
        // context.EnvironmentVariables["REGION"] = "eu-west" from inside the callback,
        // still lands a System.String in the merged result and is invisible to any
        // assertion that only inspects merged values. Measured: it passes every other
        // fact in this file. Here REGION is absent from the literal group and the first
        // line below throws KeyNotFound. The same goes for the subtler variant that
        // puts REGION in its own separate WithEnvironment(callback), which even looks
        // like "one literal plus one callback" at a glance.
        Assert.Equal("eu-west", Assert.IsType<string>(literals["REGION"]));
        Assert.DoesNotContain("API_URL", literals.Keys);
        Assert.DoesNotContain("MODE", literals.Keys);

        // ...and the other direction: an all-literal implementation writes MODE and
        // API_URL through the literal overloads, so they show up in the wrong group.
        Assert.DoesNotContain("REGION", deferred.Keys);
        Assert.Contains("API_URL", deferred.Keys);
        Assert.Contains("MODE", deferred.Keys);

        // A literal is fixed when the model is built, so it is the same string under a
        // publish context too. Assert.IsType is load-bearing on both lines: it rejects
        // the variant that "improves" REGION into a ReferenceExpression, which is a
        // third mechanism and not the one the row asks for.
        var publishLiterals = await RunEnvironmentCallbacksAsync(
            worker, DistributedApplicationOperation.Publish, ct, only: Kind.Literal);
        Assert.Equal("eu-west", Assert.IsType<string>(publishLiterals["REGION"]));
    }

    [Fact]
    public async Task The_callback_reads_the_other_resources_endpoint()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var ct = TestContext.Current.CancellationToken;

        var values = await RunEnvironmentCallbacksAsync(
            model.Resource("worker"), DistributedApplicationOperation.Run, ct);

        // The mutant: writing "http://localhost:8080" instead. It produces a value that
        // LOOKS right in a local run and is wrong everywhere else, and it lands a
        // System.String here rather than an EndpointReference - so the type assertion,
        // not the value, is what rejects it.
        var endpoint = Assert.IsType<EndpointReference>(values["API_URL"]);

        // Both halves are pinned, because reading the wrong endpoint off the right
        // resource (or the right endpoint off the wrong one) is the near-miss.
        Assert.Equal("api", endpoint.Resource.Name);
        Assert.Equal("http", endpoint.EndpointName);

        // Measured and accepted: WithEnvironment("API_URL", api.GetEndpoint("http")),
        // the non-callback overload, lands the identical EndpointReference AND the same
        // exact-typed annotation, so it passes this fact and the partition above. It is
        // an equivalent spelling for THIS variable, not a cheat - the value is still
        // computed later. What no overload of the literal form can do is the fact below.
    }

    [Fact]
    public async Task Only_a_callback_can_answer_differently_per_context()
    {
        var model = ModelHarness.Build(Ex007_EnvironmentLiteralsAndCallbacks.Configure);
        var worker = model.Resource("worker");
        var ct = TestContext.Current.CancellationToken;

        var run = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Run, ct);
        var publish = await RunEnvironmentCallbacksAsync(worker, DistributedApplicationOperation.Publish, ct);

        // A literal is fixed when the model is built, so it answers "run" in a publish
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
