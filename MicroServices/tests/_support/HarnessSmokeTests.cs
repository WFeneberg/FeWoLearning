using FeWoLearning.MicroServices.Exercises.Beginner;
using Xunit.Sdk;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Data.SqlClient;
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

        // The shared-server path added on 2026-09-07 is a SECOND way into Docker, so it
        // carries the same guard and this canary grades both. Without this half, a
        // container row that used DatabaseAsync and forgot Require() would start a real
        // SQL Server in the default `dotnet test` and nothing would have noticed.
        // Whether a shared server is already up depends on which tests ran first, so the
        // claim is that the closed gate changed NOTHING - not that nothing exists.
        var serverBefore = ContainerHarness.HasSharedServer(ContainerHarness.DatabaseFlavour.SqlServer);

        var shared = await Record.ExceptionAsync(() => ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "gate", TestContext.Current.CancellationToken));

        Assert.IsType<InvalidOperationException>(shared);
        Assert.Contains("ContainerGate.Require()", shared.Message);
        Assert.Equal(serverBefore, ContainerHarness.HasSharedServer(ContainerHarness.DatabaseFlavour.SqlServer));
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

    // --- the shared per-flavour server -----------------------------------------

    /// <summary>
    /// The isolation claim, PROVED rather than asserted. One SQL Server serves every
    /// container row in the assembly, so "a fresh database per test" has to be a real
    /// boundary and not a naming convention.
    ///
    /// Two databases are taken from the same flavour, the SAME table name is created in
    /// each with a different sentinel row, and neither may see the other's. The second
    /// half is the part that would be missing from an assertion-free claim: both
    /// connection strings must name the same host and port - i.e. the same container -
    /// so this is genuinely two databases on one server rather than two servers that
    /// happen to work.
    /// </summary>
    [Fact]
    public async Task SharedServer_hands_out_ISOLATED_databases_on_ONE_server()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var first = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "canaryA", token);
        var second = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "canaryB", token);

        // One server, two catalogues.
        Assert.NotEqual(first.Name, second.Name);
        Assert.Equal(first.ServerConnectionString, second.ServerConnectionString);
        Assert.Equal(
            new SqlConnectionStringBuilder(first.ConnectionString).DataSource,
            new SqlConnectionStringBuilder(second.ConnectionString).DataSource);
        Assert.Equal(first.Name, new SqlConnectionStringBuilder(first.ConnectionString).InitialCatalog);

        // ...and that server is a real container on a DCP-allocated port, not 1433 and
        // not a placeholder. The one place in the track where this is now worth
        // asserting: it grades the harness, which is what hands every row its string.
        var dataSource = new SqlConnectionStringBuilder(first.ConnectionString).DataSource;

        // NAMED placeholders, never "no brace at all" - README section 4 documents this
        // for Postgres and it caught this canary too, intermittently: Aspire generates the
        // SA password from a character set that includes "{", so roughly one run in ten
        // produces a perfectly resolved connection string with a brace in the password.
        Assert.DoesNotContain("{shared-sqlserver", first.ConnectionString);
        Assert.DoesNotContain(".connectionString}", first.ConnectionString);
        Assert.Contains(",", dataSource);
        Assert.NotEqual(1433, int.Parse(dataSource.Split(',')[1]));

        // The same table name in both, with values invented microseconds ago.
        var a = $"A-{Guid.NewGuid():N}";
        var b = $"B-{Guid.NewGuid():N}";
        await ExecuteAsync(first.ConnectionString,
            $"CREATE TABLE [probe] ([value] nvarchar(64) NOT NULL); INSERT INTO [probe] VALUES (N'{a}')", token);
        await ExecuteAsync(second.ConnectionString,
            $"CREATE TABLE [probe] ([value] nvarchar(64) NOT NULL); INSERT INTO [probe] VALUES (N'{b}')", token);

        // Neither sees the other. If the databases were not real boundaries, the CREATE
        // TABLE above would already have failed with "there is already an object named
        // 'probe'" - so this fact grades the boundary twice over.
        Assert.Equal(a, await ScalarTextAsync(first.ConnectionString, "SELECT [value] FROM [probe]", token));
        Assert.Equal(b, await ScalarTextAsync(second.ConnectionString, "SELECT [value] FROM [probe]", token));
        Assert.Equal(1L, await ScalarAsync(first.ConnectionString, "SELECT COUNT_BIG(*) FROM [probe]", token));
    }

    /// <summary>
    /// What sharing a server CHANGED, and therefore what the teardown canary above no
    /// longer covers on its own.
    ///
    /// With one application per test, a failing test tore its own server down and the
    /// next test got a clean one; that is the property
    /// <see cref="ContainerHarness_teardown_never_replaces_the_real_failure"/> is about.
    /// With a shared server, a failing test must leave the server RUNNING and usable -
    /// the failure is the test's, not the session's. So: fail inside a shared database,
    /// then take another one and use it.
    ///
    /// It also pins the thing that would otherwise be invisible: no second server was
    /// started to service the second request.
    /// </summary>
    [Fact]
    public async Task SharedServer_survives_a_failing_test_without_starting_a_second_server()
    {
        ContainerGate.Require();
        var token = TestContext.Current.CancellationToken;

        var doomed = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "canaryFail", token);

        var failure = await Record.ExceptionAsync(async () =>
        {
            await ExecuteAsync(doomed.ConnectionString, "CREATE TABLE [half] ([x] int)", token);
            throw new XunitException("A TEST FAILED HERE");
        });
        Assert.IsType<XunitException>(failure);
        Assert.True(ContainerHarness.HasSharedServer(ContainerHarness.DatabaseFlavour.SqlServer));

        // The next row gets a working, empty database on the very same server.
        var next = await ContainerHarness.DatabaseAsync(
            ContainerHarness.DatabaseFlavour.SqlServer, "canaryNext", token);
        Assert.Equal(doomed.ServerConnectionString, next.ServerConnectionString);
        Assert.Equal(0L, await ScalarAsync(next.ConnectionString,
            "SELECT COUNT_BIG(*) FROM sys.tables WHERE name = N'half'", token));
    }

    private static async Task ExecuteAsync(string connectionString, string sql, CancellationToken ct)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(ct);
    }

    private static async Task<object?> ScalarAsync(string connectionString, string sql, CancellationToken ct)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand(sql, connection);
        return await command.ExecuteScalarAsync(ct);
    }

    private static async Task<string?> ScalarTextAsync(string connectionString, string sql, CancellationToken ct)
        => (await ScalarAsync(connectionString, sql, ct)) as string;

    // --- ManifestHarness's shared publish ---------------------------------------

    /// <summary>
    /// One publish per distinct model, however many facts ask for it.
    ///
    /// The number this protects is measured in README section 6: a publish costs a fixed
    /// amount whatever the model contains, so the only thing worth managing at L2 is how
    /// many happen. This fact watches the counter directly - three calls for two distinct
    /// models must cost exactly two publishes - because a cache that silently stopped
    /// caching would show up nowhere else until someone profiled the suite again.
    ///
    /// It also pins the property that keeps every existing call site correct: the
    /// JsonDocument handed back is the CALLER's, so disposing it must not damage the
    /// shared output behind it.
    /// </summary>
    [Fact]
    public async Task ManifestHarness_publishes_ONCE_per_model_however_many_facts_ask()
    {
        var token = TestContext.Current.CancellationToken;
        var before = ManifestHarness.PublishCount;

        using (var first = await ManifestHarness.GenerateAsync(SharedProbeModel, token))
        {
            Assert.Equal("container.v0",
                first.RootElement.GetProperty("resources").GetProperty("probe").GetProperty("type").GetString());
        }

        // ...and the caller disposed that document. A second ask must still work, and
        // must not have paid for another publish.
        using var second = await ManifestHarness.GenerateAsync(SharedProbeModel, token);
        Assert.Equal("container.v0",
            second.RootElement.GetProperty("resources").GetProperty("probe").GetProperty("type").GetString());

        var afterSameModel = ManifestHarness.PublishCount;
        Assert.Equal(before + 1, afterSameModel);

        // A DIFFERENT model must still cost one - the cache keys on the delegate, and a
        // cache that returned this manifest for that model would be far worse than a
        // slow one.
        using var other = await ManifestHarness.GenerateAsync(OtherProbeModel, token);
        Assert.True(other.RootElement.GetProperty("resources").TryGetProperty("elsewhere", out _));
        Assert.False(other.RootElement.GetProperty("resources").TryGetProperty("probe", out _));
        Assert.Equal(afterSameModel + 1, ManifestHarness.PublishCount);
    }

    /// <summary>
    /// The other half of the sharing contract, and the dangerous half.
    ///
    /// A CLOSURE over mutable state is harmless - each instance is a different Target, so
    /// it misses the cache. A **static** Configure reading **static mutable state** is
    /// not: its key is a stable (MethodInfo, null), so the second call HITS and would be
    /// handed the first call's manifest with nothing said. That pattern is live in this
    /// assembly - TestParallelism.cs names ex023's scenario flags and ex025's hook log -
    /// so this is a trap the next sixty rows can walk into, not a hypothetical.
    ///
    /// This fact IS the mutant, twice over: a static Configure whose model depends on a
    /// static flag, called twice with the flag flipped, first with a STRUCTURAL
    /// difference and then with a VALUE-ONLY one. Both must throw rather than answer, and
    /// the message must point at the escape hatch - which the middle part exercises,
    /// because a guard that only forbids is half a fix.
    ///
    /// What this fact does NOT cover, deliberately, is documented on
    /// ManifestHarness.VerifyUnchanged: a value computed inside a CALLBACK
    /// (`WithEnvironment`, `WithArgs`) is invisible to the fingerprint, because catching
    /// it would mean the guard invoking learner-authored callbacks. The RULE is the
    /// protection there; this is a net under the rest.
    /// </summary>
    [Fact]
    public async Task ManifestHarness_REFUSES_to_share_a_stale_publish_with_a_changed_model()
    {
        var token = TestContext.Current.CancellationToken;
        try
        {
            StaleProbeFlag = false;
            using (var first = await ManifestHarness.GenerateAsync(StaleProbeModel, token))
            {
                Assert.True(first.RootElement.GetProperty("resources").TryGetProperty("plain", out _));
            }

            // Same delegate, different model. Without the guard this returns the manifest
            // above and every assertion about "flagged" fails somewhere far away.
            StaleProbeFlag = true;
            var thrown = await Record.ExceptionAsync(() => ManifestHarness.GenerateAsync(StaleProbeModel, token));

            var refused = Assert.IsType<InvalidOperationException>(thrown);
            Assert.Contains("StaleProbeModel", refused.Message);
            Assert.Contains("PublishAsync", refused.Message);

            // ...and the documented way out really works: PublishAsync shares nothing, so
            // it sees the model as it is now.
            using var output = await ManifestHarness.PublishAsync(StaleProbeModel, token);
            Assert.True(output.Manifest.RootElement.GetProperty("resources").TryGetProperty("flagged", out _));

            // ---- and the harder half: a VALUE-only change ---------------------------
            // Same resources, same types, same annotation kinds - only an image tag
            // differs. The first version of the fingerprint recorded annotation TYPE
            // names only and this slipped straight through; measured by reverting to it,
            // the second call below returns the stale manifest and this fact fails on a
            // null exception. It is caught now because the fingerprint renders every
            // annotation property whose type is a string, primitive, enum or IResource.
            StaleProbeFlag = false;
            using (var beforeChange = await ManifestHarness.GenerateAsync(StaleValueProbeModel, token))
            {
                Assert.Equal("busybox:one",
                    beforeChange.RootElement.GetProperty("resources").GetProperty("tagged")
                                .GetProperty("image").GetString());
            }

            StaleProbeFlag = true;
            var valueChange = await Record.ExceptionAsync(
                () => ManifestHarness.GenerateAsync(StaleValueProbeModel, token));

            var refusedValue = Assert.IsType<InvalidOperationException>(valueChange);
            Assert.Contains("StaleValueProbeModel", refusedValue.Message);
            Assert.Contains("PublishAsync", refusedValue.Message);
        }
        finally
        {
            // Static state in a serial assembly is still static state.
            StaleProbeFlag = false;
        }
    }

    private static bool StaleProbeFlag;

    /// <summary>A static Configure over static mutable state - the hazard, deliberately.</summary>
    private static void StaleProbeModel(IDistributedApplicationBuilder builder)
        => builder.AddContainer(StaleProbeFlag ? "flagged" : "plain", "busybox");

    /// <summary>
    /// The same hazard, but structurally IDENTICAL between the two states: same resource,
    /// same type, same annotations, only a tag differs. This is what a fingerprint over
    /// annotation type names cannot see.
    /// </summary>
    private static void StaleValueProbeModel(IDistributedApplicationBuilder builder)
        => builder.AddContainer("tagged", "busybox")
                  .WithImageTag(StaleProbeFlag ? "two" : "one");

    private static void SharedProbeModel(IDistributedApplicationBuilder builder)
        => builder.AddContainer("probe", "busybox");

    private static void OtherProbeModel(IDistributedApplicationBuilder builder)
        => builder.AddContainer("elsewhere", "busybox");
}
