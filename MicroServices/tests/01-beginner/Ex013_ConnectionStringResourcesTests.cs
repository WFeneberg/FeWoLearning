using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex013_ConnectionStringResourcesTests
{
    [Fact]
    public void The_external_store_is_a_connection_string_resource_with_a_referenced_secret()
    {
        var model = ModelHarness.Build(Ex013_ConnectionStringResources.Configure);

        // The exact type is the grade, and it rejects three different wrong answers:
        //   * AddContainer("legacy", "oracle...") - Aspire would try to run the thing
        //     the row says it does not host;
        //   * AddParameter("legacy") - a ParameterResource is not
        //     IResourceWithConnectionString, so no consumer can WithReference it;
        //   * AddConnectionString("legacy") and AddConnectionString("legacy",
        //     "LEGACY_CONNECTION") - measured on 13.5.3, BOTH of those return an
        //     internal ConnectionStringParameterResource that publishes as
        //     parameter.v0 rather than the value.v0 this row is about. That type is
        //     internal, so a test cannot even name it (CS0122) - IsType against the
        //     public ConnectionStringResource is how it gets rejected.
        var legacy = Assert.IsType<ConnectionStringResource>(model.Resource("legacy"));

        // The expression is entirely learner-written, unlike a hosted store's, so it is
        // pinned whole. The interesting half is the tail: {legacy-password.value} is a
        // REFERENCE to the secret parameter, and the mutant that writes the password
        // into the literal string - "...;Password=hunter2" - fails right here, before
        // it ever reaches the manifest.
        Assert.Equal(
            "Data Source=legacy-oracle.corp:1521/ORCL;User Id=reporting;Password={legacy-password.value}",
            ModelHarness.ConnectionString(legacy));

        // And nothing was started for it: no image, no endpoint, no environment. This
        // is the whole point of the row - "a store Aspire does not host".
        Assert.Empty(legacy.Annotations.OfType<ContainerImageAnnotation>());
        Assert.Empty(legacy.Annotations.OfType<EndpointAnnotation>());
    }

    [Fact]
    public async Task The_manifest_separates_value_v0_from_container_v0()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex013_ConnectionStringResources.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");
        var legacy = resources.GetProperty("legacy");
        var cache = resources.GetProperty("cache");

        // value.v0 is "a connection string and nothing else". container.v0 is an image
        // with an environment and bindings. Asserting the pair, not just the first
        // half, is what makes this a comparison rather than a spot check - a model
        // that lost its container entirely would still have a value.v0 in it.
        Assert.Equal("value.v0", legacy.GetProperty("type").GetString());
        Assert.Equal("container.v0", cache.GetProperty("type").GetString());
        Assert.False(legacy.TryGetProperty("image", out _));
        Assert.Equal("redis:latest", cache.GetProperty("image").GetString());

        // The connection string publishes inline, still unresolved where the secret is.
        Assert.Equal(
            "Data Source=legacy-oracle.corp:1521/ORCL;User Id=reporting;Password={legacy-password.value}",
            legacy.GetProperty("connectionString").GetString());

        // ...and the secret half is its own parameter.v0, marked secret. The mutant
        // that declares the parameter without secret: true publishes an inputs.value
        // with no "secret" key and fails here.
        var password = resources.GetProperty("legacy-password");
        Assert.Equal("parameter.v0", password.GetProperty("type").GetString());
        Assert.True(password.GetProperty("inputs").GetProperty("value").GetProperty("secret").GetBoolean());
    }

    [Fact]
    public async Task The_consumer_references_it_exactly_as_it_would_a_hosted_store()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex013_ConnectionStringResources.Configure,
            TestContext.Current.CancellationToken);

        var env = manifest.RootElement
            .GetProperty("resources").GetProperty("reporting").GetProperty("env");

        // {legacy.connectionString} - a reference the deployment target resolves, the
        // same shape a Postgres database emits in ex002. That is the payoff of
        // modelling the external store at all.
        //
        // The mutant this rejects is the one that skips AddConnectionString and wires
        // the consumer up by hand:
        //
        //     builder.AddContainer("reporting", "busybox")
        //            .WithEnvironment("ConnectionStrings__legacy",
        //                "Data Source=legacy-oracle.corp:1521/ORCL;User Id=reporting;Password=hunter2");
        //
        // Measured: that publishes the literal string into env instead, so this
        // assertion fails - and the model it builds has no resource named "legacy" at
        // all, so the first fact fails too.
        Assert.Equal(
            "{legacy.connectionString}",
            env.GetProperty("ConnectionStrings__legacy").GetString());
    }
}
