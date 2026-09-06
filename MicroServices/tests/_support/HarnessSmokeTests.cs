using FeWoLearning.MicroServices.Exercises.Beginner;
using Xunit.Sdk;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

    // --- the container gate's three canaries -----------------------------------
    // Three directions matter. A gate that never skips would start real containers in
    // the default run; a gate that ALWAYS skips would silently disable all 25 container
    // rows while every run still looked green; and an author who simply forgets to call
    // Require() would do the first of those by accident. One fact each. A fourth fact
    // below guards ContainerHarness's teardown rather than its gate.

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

    /// <summary>
    /// The gate's third canary, and the one that protects the DEFAULT run rather than
    /// the opt-in one. ContainerGate.Require() is a call an author has to remember to
    /// write; this is the backstop for the day somebody forgets it. With containers off
    /// ContainerHarness must refuse to build or start anything at all, so a missing
    /// Require() turns into a loud failure in the L3 row instead of `dotnet test`
    /// quietly pulling images on a machine that was promised a Docker-free run.
    ///
    /// It asserts a THROW, not a skip: skipping here would be the same silence the two
    /// canaries above exist to break. Never touches Docker - the guard is the first
    /// statement in RunAsync, before a builder is even constructed. Force(false) is
    /// what lets it grade the closed gate in BOTH modes, so `-p:Containers=true` keeps
    /// reporting zero skips rather than growing one that nobody reads.
    /// </summary>
    [Fact]
    public async Task ContainerHarness_refuses_to_start_anything_when_the_gate_is_closed()
    {
        using var forced = ContainerGate.Force(false);
        Assert.False(ContainerGate.Enabled, "ContainerGate.Force(false) did not reach Enabled.");

        var thrown = await Record.ExceptionAsync(() => ContainerHarness.RunAsync(
            _ => Assert.Fail("ContainerHarness ran configure() with containers off."),
            _ => Task.CompletedTask,
            TestContext.Current.CancellationToken));

        Assert.IsType<InvalidOperationException>(thrown);
        Assert.Contains("ContainerGate.Require()", thrown.Message);
    }

    // --- ContainerHarness's teardown contract ----------------------------------

    /// <summary>
    /// The single highest-leverage invariant in this file, because
    /// <see cref="ContainerHarness.RunAsync"/> is what all 25 container rows share:
    /// **a failing teardown must never replace a failing test.**
    ///
    /// This is a REGRESSION test, not a hypothetical. The first version of the harness
    /// tore down inside a plain `finally`, where anything thrown wins. Measured against
    /// that version, with this exact probe: the body failed with
    /// "THE REAL ASSERTION FAILURE" and what surfaced was
    /// `InvalidOperationException: TEARDOWN BLEW UP`. A genuinely broken exercise would
    /// have reported a teardown error instead of what actually went wrong, and the
    /// learner would have gone looking in the wrong place.
    ///
    /// The second half is the other direction, and it is not symmetric: when the body
    /// SUCCEEDS, a teardown failure must be reported rather than swallowed, because a
    /// StopAsync that failed is how containers, networks and volumes start surviving
    /// the run and poisoning every test behind them.
    ///
    /// It needs a real application (StartAsync needs DCP), so it is gated - but it
    /// starts NO containers: the model is empty and the only moving part is a hosted
    /// service that throws on stop.
    /// </summary>
    [Fact]
    public async Task ContainerHarness_teardown_never_replaces_the_real_failure()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var real = await Record.ExceptionAsync(() => ContainerHarness.RunAsync(
            builder => builder.Services.AddHostedService<ThrowsOnStop>(),
            _ => throw new XunitException("THE REAL ASSERTION FAILURE"),
            token,
            TimeSpan.FromMinutes(2)));

        var surfaced = Assert.IsType<XunitException>(real);
        Assert.Equal("THE REAL ASSERTION FAILURE", surfaced.Message);

        var teardownOnly = await Record.ExceptionAsync(() => ContainerHarness.RunAsync(
            builder => builder.Services.AddHostedService<ThrowsOnStop>(),
            _ => Task.CompletedTask,
            token,
            TimeSpan.FromMinutes(2)));

        var reported = Assert.IsType<InvalidOperationException>(teardownOnly);
        Assert.Contains("tearing the application down", reported.Message);
        Assert.Contains("TEARDOWN BLEW UP", reported.InnerException?.Message);
    }

    /// <summary>A hosted service whose only job is to fail <c>StopAsync</c>.</summary>
    private sealed class ThrowsOnStop : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)
            => throw new InvalidOperationException("TEARDOWN BLEW UP");
    }
}
