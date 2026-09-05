using FeWoLearning.MicroServices.Exercises.Beginner;
using Xunit.Sdk;
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
        using var manifest = await ManifestHarness.GenerateAsync(
            b => b.AddPostgres("pg").AddDatabase("orders"), TestContext.Current.CancellationToken);

        var pg = manifest.RootElement.GetProperty("resources").GetProperty("pg");
        Assert.Equal("container.v0", pg.GetProperty("type").GetString());
        Assert.StartsWith("docker.io/library/postgres:", pg.GetProperty("image").GetString());
    }

    [Fact]
    public async Task ManifestHarness_hands_back_the_generated_Bicep_too()
    {
        using var output = await ManifestHarness.PublishAsync(b =>
        {
            b.AddAzureContainerAppEnvironment("aca");
            b.AddAzureStorage("storage");
        }, TestContext.Current.CancellationToken);

        // The manifest is NOT the only in-process artifact; the Azure rows (093, 094,
        // 099, 100) grade real generated Bicep, so the harness must be able to read it.
        var bicep = output.BicepFiles;
        Assert.NotEmpty(bicep);

        var storage = Assert.Single(bicep,
            f => f.Key.Contains("storage", StringComparison.OrdinalIgnoreCase)
                 && f.Key.EndsWith(".module.bicep", StringComparison.Ordinal));
        Assert.Contains("Microsoft.Storage/storageAccounts", storage.Value, StringComparison.Ordinal);

        Assert.Equal("azure.bicep.v0",
            output.Manifest.RootElement.GetProperty("resources").GetProperty("storage")
                  .GetProperty("type").GetString());
    }

    [Fact]
    public async Task PublishOutput_deletes_its_directory_on_dispose()
    {
        // The one way this harness could leak temp directories.
        string dir;
        using (var output = await ManifestHarness.PublishAsync(
                   b => b.AddContainer("api", "nginx"), TestContext.Current.CancellationToken))
        {
            dir = output.Directory;
            Assert.True(Directory.Exists(dir));
        }
        Assert.False(Directory.Exists(dir), $"PublishOutput left {dir} behind.");
    }

    // --- the container gate's two canaries -------------------------------------
    // Both directions matter. A gate that never skips would start real containers in
    // the default run; a gate that ALWAYS skips would silently disable all 25 container
    // rows while every run still looked green. One fact each.

    [Fact]
    public void ContainerGate_Require_skips_when_containers_are_off()
    {
        ContainerGate.Require();
        Assert.True(ContainerGate.Enabled, "Require() let a test through with containers off.");
    }

    [Fact]
    public void ContainerGate_Require_lets_the_test_through_when_containers_are_on()
    {
        using var forced = ContainerGate.Force(true);
        Assert.True(ContainerGate.Enabled, "ContainerGate.Force(true) did not reach Enabled.");

        // A plain try/catch, deliberately NOT Record.Exception: xunit v3's Record
        // re-throws skip exceptions, which would report THIS fact as skipped instead
        // of failing it - exactly the silence this canary exists to break.
        Exception? thrown = null;
        try { ContainerGate.Require(); }
        catch (Exception ex) { thrown = ex; }

        if (thrown is SkipException)
        {
            Assert.Fail(
                "ContainerGate.Require() skipped with containers ON: the gate is stuck closed. " +
                "Every one of the 25 container-backed rows would silently stop running while " +
                "the suite still reported green.");
        }
        Assert.Null(thrown);
    }
}
