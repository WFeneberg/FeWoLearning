using FeWoLearning.MicroServices.Exercises.Beginner;
using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Fails first when the two-library UseSolutions mechanism breaks. These facts
/// must pass in BOTH the red run and the green run - they grade the harness,
/// not an exercise.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void Tier_marker_resolves_from_whichever_library_is_referenced()
        => Assert.Equal("01-beginner", TierMarker.Tier);

    [Fact]
    public void Exactly_one_content_library_is_loaded()
    {
        var names = typeof(TierMarker).Assembly.GetName().Name;
        Assert.True(
            names is "FeWoLearning.MicroServices.Exercises" or "FeWoLearning.MicroServices.Solutions",
            $"Unexpected content assembly: {names}");
    }
}

public class HarnessMechanicsTests
{
    [Fact]
    public void ModelHarness_exposes_resources_and_connection_strings()
    {
        var model = ModelHarness.Build(b =>
        {
            var pg = b.AddPostgres("pg").AddDatabase("orders");
            b.AddContainer("worker", "busybox").WithReference(pg).WaitFor(pg);
        });

        var orders = model.Resource("orders");
        Assert.IsType<PostgresDatabaseResource>(orders);
        Assert.Equal("{pg.connectionString};Database=orders", ModelHarness.ConnectionString(orders));
        Assert.Equal(2, model.Resource("worker").Annotations.OfType<WaitAnnotation>().Count());
    }

    [Fact]
    public async Task ManifestHarness_generates_a_manifest_in_process()
    {
        using var manifest = await ManifestHarness.GenerateAsync(b => b.AddPostgres("pg").AddDatabase("orders"));

        var pg = manifest.RootElement.GetProperty("resources").GetProperty("pg");
        Assert.Equal("container.v0", pg.GetProperty("type").GetString());
        Assert.StartsWith("docker.io/library/postgres:", pg.GetProperty("image").GetString());
    }

    [Fact]
    public void ContainerGate_Require_skips_when_containers_are_off()
    {
        ContainerGate.Require();
        Assert.True(ContainerGate.Enabled, "Require() let a test through with containers off.");
    }
}
