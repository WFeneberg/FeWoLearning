using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex019_ExcludeFromManifestTests
{
    [Fact]
    public void The_dev_tool_is_a_real_resource_in_the_built_model()
    {
        var model = ModelHarness.Build(Ex019_ExcludeFromManifest.Configure);

        // Half one of the two the catalog row insists on. A resource that was never
        // added is absent from the manifest just as convincingly as an excluded one,
        // so the manifest fact below grades nothing on its own - this is what makes
        // "never wrote it" fail.
        var adminer = Assert.IsType<ContainerResource>(model.Resource("adminer"));

        // And it is genuinely wired, not a placeholder that happens to bear the name:
        // the right image, and an endpoint of its own for the dashboard to link to.
        var image = Assert.Single(adminer.Annotations.OfType<ContainerImageAnnotation>());
        Assert.Equal("adminer", image.Image);
        Assert.Single(adminer.Annotations.OfType<EndpointAnnotation>());

        // The neighbours it is being compared against.
        Assert.IsType<PostgresServerResource>(model.Resource("pg"));
        Assert.IsType<PostgresDatabaseResource>(model.Resource("orders"));
        Assert.True(model.Has("api"));
    }

    [Fact]
    public async Task The_dev_tool_is_absent_from_the_manifest_while_its_neighbours_are_there()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex019_ExcludeFromManifest.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        // Half two. Nothing here would fail if the publish had silently produced an
        // empty manifest, so the positive assertions are load-bearing: they prove the
        // publish ran and wrote the rest of the graph.
        Assert.True(resources.TryGetProperty("pg", out var pg));
        Assert.Equal("container.v0", pg.GetProperty("type").GetString());
        Assert.True(resources.TryGetProperty("api", out var api));
        Assert.Equal("container.v0", api.GetProperty("type").GetString());

        // The one that must not be there. Omitting ExcludeFromManifest publishes
        // "adminer" as a perfectly ordinary container.v0.
        Assert.False(resources.TryGetProperty("adminer", out _));
    }

    [Fact]
    public void Exclusion_is_the_mechanism_rather_than_a_run_mode_branch()
    {
        var model = ModelHarness.Build(Ex019_ExcludeFromManifest.Configure);

        // The fact that separates this row from ex020, and the reason it exists at
        // all. Measured on 13.5.3: `if (builder.ExecutionContext.IsRunMode)
        // builder.AddContainer("adminer", ...)` passes BOTH facts above - the model
        // harness builds in run mode, so the resource is there, and the publish-mode
        // builder never creates it, so the manifest lacks it. The two answers are
        // indistinguishable except here: ExcludeFromManifest leaves a
        // ManifestPublishingCallbackAnnotation behind and the branch leaves nothing.
        Assert.Single(model.Resource("adminer").Annotations.OfType<ManifestPublishingCallbackAnnotation>());

        // Measured too: a plain AddContainer carries none of these, so this is a real
        // difference between the two containers and not a property everything has.
        Assert.Empty(model.Resource("api").Annotations.OfType<ManifestPublishingCallbackAnnotation>());
    }
}
