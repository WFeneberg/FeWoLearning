using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// The L3 harness: STARTS an application for real - containers, allocated ports,
/// resolved connection strings - runs one test body against it, and tears everything
/// down again. <see cref="ModelHarness"/> and <see cref="ManifestHarness"/> deliberately
/// stop before anything runs; this is the only place in the track where Docker is
/// touched.
///
/// Every 🐳 row goes through here. Call <see cref="ContainerGate.Require"/> as the first
/// line of the test; this type additionally refuses to start anything when the gate is
/// closed, so a forgotten Require() cannot make the default `dotnet test` pull images.
///
/// ---------------------------------------------------------------------------------
/// Why it is shaped this way - all four points measured on 2026-09-06, 13.5.3:
///
/// 1. It builds the application with plain <c>DistributedApplication.CreateBuilder</c>,
///    NOT with <c>DistributedApplicationTestingBuilder</c>. The latter's
///    <c>Create(args)</c> overload throws "No application host assembly was found.
///    Ensure that you have a project that references the 'Aspire.Hosting.AppHost'
///    package and imports the 'Aspire.AppHost.Sdk' SDK." - there is no AppHost project
///    here to point it at, and there cannot be one: an AppHost referencing exercises/
///    would collide with solutions/ under -p:UseSolutions=true, which is the whole
///    reason the two content libraries are never in the build together.
/// 2. Starting therefore needs DCP, which the AppHost SDK normally supplies. tests/
///    pulls <c>Aspire.Hosting.AppHost</c> + <c>Aspire.Hosting.Orchestration.&lt;rid&gt;</c>
///    by hand, guarded by <c>Condition="'$(Containers)' == 'true'"</c>, so the default
///    run restores and builds exactly what it did before. See the comment in
///    tests/FeWoLearning.MicroServices.Tests.csproj.
/// 3. <c>DcpPublisher:DashboardPath</c> must be set even though the dashboard is
///    disabled. Without it, StartAsync dies in Aspire's own BeforeStartEvent subscriber
///    with <c>OptionsValidationException: Property DashboardPath: The path to the Aspire
///    Dashboard binaries is missing.</c> - the same class of failure EventingHarness
///    documents for <c>DcpPublisher:CliPath</c>. Nothing ever executes the placeholder.
/// 4. <c>DcpPublisher:WaitForResourceCleanup</c> is the difference between a clean tree
///    and a slow leak. Without it the CONTAINER is removed but the per-session Docker
///    NETWORK (<c>aspire-session-network-&lt;id&gt;-FeWoLearning.MicroServices.Tests</c>)
///    survives the run, one per test, forever. With it, `docker network ls` is clean
///    after two consecutive runs. It costs ~10 s on teardown and is worth it.
///
/// Measured cost of one Postgres row on this machine (Docker 29.7.2, image already
/// pulled): ~27 s to StartAsync, ~15 s more to healthy - the entrypoint has to initdb
/// and Aspire then creates the AddDatabase child - and ~10 s to stop and clean up.
/// About 52 s end to end. The image IS pulled on demand the first time, which is
/// minutes, not seconds; see MicroServices/README.md section 4.
/// </summary>
public static class ContainerHarness
{
    /// <summary>
    /// How long one session gets, start to finish. A hung container must fail the test,
    /// not wedge the suite - and the assembly runs serially, so a wedge stops everything
    /// behind it. Five minutes is roughly six times the measured 52 s of a warm Postgres
    /// row, which leaves room for a cold `docker pull` on a slow line.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Teardown gets its own budget, deliberately NOT the session's: when the session
    /// deadline is what killed the test, its token is already cancelled, and passing it
    /// to StopAsync would skip the cleanup exactly when cleanup matters most.
    /// </summary>
    private static readonly TimeSpan CleanupTimeout = TimeSpan.FromMinutes(2);

    public sealed class Session
    {
        private readonly DistributedApplication _app;

        internal Session(DistributedApplication app, IReadOnlyList<IResource> resources, CancellationToken cancellationToken)
        {
            _app = app;
            Resources = resources;
            CancellationToken = cancellationToken;
        }

        /// <summary>The running application. Started; not yet stopped.</summary>
        public DistributedApplication Application => _app;

        /// <summary>
        /// The session deadline, linked to the test's own token. Pass it to everything -
        /// a query that blocks forever against a half-started database is the failure
        /// mode this exists for.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        public IReadOnlyList<IResource> Resources { get; }

        public IResource Resource(string name)
            => Resources.SingleOrDefault(r => r.Name == name)
               ?? throw new InvalidOperationException(
                   $"No resource named '{name}'. Model contains: " +
                   string.Join(", ", Resources.Select(r => $"{r.Name}({r.GetType().Name})")));

        /// <summary>
        /// Blocks until the named resource reports healthy. This is the harness half of
        /// the row-034 "WaitFor it": a consumer in the model waits through a
        /// WaitAnnotation, and a test - which is not in the model - waits through this.
        /// For a Postgres AddDatabase child, healthy means the server finished its
        /// entrypoint (init scripts included) AND Aspire created the database.
        /// </summary>
        public Task WaitForHealthyAsync(string name)
            => _app.Services.GetRequiredService<ResourceNotificationService>()
                   .WaitForResourceHealthyAsync(name, CancellationToken);

        /// <summary>
        /// The RESOLVED connection string - a real host, the port DCP allocated, the
        /// generated password - as opposed to the <c>{pg.bindings.tcp.host}</c>
        /// expression every L1 assertion sees.
        /// </summary>
        public async Task<string> ConnectionStringAsync(string name)
            => await _app.GetConnectionStringAsync(name, CancellationToken)
               ?? throw new InvalidOperationException($"Resource '{name}' produced no connection string.");
    }

    /// <summary>
    /// Builds the model, starts it, runs <paramref name="body"/>, and tears the
    /// application down again - the teardown happens whether the body passed, failed or
    /// timed out.
    /// </summary>
    public static async Task RunAsync(
        Action<IDistributedApplicationBuilder> configure,
        Func<Session, Task> body,
        CancellationToken cancellationToken = default,
        TimeSpan? timeout = null)
    {
        if (!ContainerGate.Enabled)
        {
            throw new InvalidOperationException(
                "ContainerHarness.RunAsync was reached with container tests OFF. Every L3 test "
                + "must call ContainerGate.Require() as its first line; this guard exists so a "
                + "missing call cannot make the default `dotnet test` start Docker containers.");
        }

        var builder = CreateBuilder();

        configure(builder);
        var resources = builder.Resources.ToList();

        var app = builder.Build();
        var started = false;
        var stopwatch = Stopwatch.StartNew();

        // The session's own failure is CAPTURED rather than thrown, so that teardown can
        // run to completion and still not be able to replace it. A `finally` block cannot
        // express that: anything it throws wins, and a test whose real assertion failed
        // would report a teardown error instead of what actually went wrong.
        Exception? sessionFailure = null;
        try
        {
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            deadline.CancelAfter(timeout ?? DefaultTimeout);

            try
            {
                await app.StartAsync(deadline.Token);
                started = true;
                await body(new Session(app, resources, deadline.Token));
            }
            catch (OperationCanceledException) when (deadline.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"The container session exceeded {(timeout ?? DefaultTimeout).TotalSeconds:0} s "
                    + $"(it had {(started ? "started" : "NOT finished starting")} after {stopwatch.Elapsed}). "
                    + "Is Docker running, and is the image already pulled?");
            }
        }
        catch (Exception exception)
        {
            sessionFailure = exception;
        }

        var teardownFailure = await TearDownAsync(app, started);

        // Order matters and is the whole point: the real failure always wins.
        if (sessionFailure is not null)
        {
            ExceptionDispatchInfo.Capture(sessionFailure).Throw();
        }

        // Only when the session itself succeeded is a broken teardown worth reporting -
        // and then it must be reported, because a StopAsync that failed is how containers,
        // networks and volumes start surviving the run.
        if (teardownFailure is not null)
        {
            throw new InvalidOperationException(
                "The container session's body succeeded, but tearing the application down "
                + "did not. Check `docker ps -a`, `docker network ls` and `docker volume ls` "
                + "for leftovers before trusting any later run in this suite.",
                teardownFailure);
        }
    }

    /// <summary>
    /// The one builder shape this harness ever constructs - used by
    /// <see cref="RunAsync"/> and by the shared per-flavour servers alike, so that the
    /// two DCP configuration keys documented in points 3 and 4 of the type comment
    /// cannot drift apart between the two paths.
    /// </summary>
    private static IDistributedApplicationBuilder CreateBuilder()
    {
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = [],
            DisableDashboard = true
        });

        // See point 3 in the type comment: required even with the dashboard disabled.
        builder.Configuration["DcpPublisher:DashboardPath"] = "not-used-the-dashboard-is-disabled";
        // See point 4: without this the per-session Docker network outlives the run.
        builder.Configuration["DcpPublisher:WaitForResourceCleanup"] = "true";

        return builder;
    }

    /// <summary>
    /// Stops and disposes the application on <see cref="CleanupTimeout"/>, swallowing
    /// nothing and throwing nothing: the first failure is handed back to the caller,
    /// which decides whether it is allowed to surface.
    ///
    /// Three things here are deliberate, and all three exist because this method is what
    /// 25 container rows share.
    /// <list type="number">
    ///   <item>The budget is its OWN, never the session token. When the session deadline
    ///   is what killed the test, that token is already cancelled, and passing it here
    ///   would skip cleanup exactly when cleanup matters most.</item>
    ///   <item><c>DisposeAsync</c> is bounded too, not just <c>StopAsync</c>. The assembly
    ///   runs serially, so an unbounded dispose does not fail one test - it wedges every
    ///   test behind it, which is precisely what the session deadline exists to prevent.
    ///   <c>WaitAsync</c> abandons a hung dispose rather than cancelling it (there is no
    ///   cancellable overload); that is the right trade, because a dispose that has
    ///   already hung is not going to clean up either way and the suite must keep moving.</item>
    ///   <item>Dispose is attempted even when Stop threw, and the FIRST failure is the one
    ///   returned - a stop failure explains a dispose failure far more often than the
    ///   other way round.</item>
    /// </list>
    /// </summary>
    private static async Task<Exception?> TearDownAsync(DistributedApplication app, bool started)
    {
        Exception? failure = null;
        using var cleanup = new CancellationTokenSource(CleanupTimeout);

        if (started)
        {
            try
            {
                await app.StopAsync(cleanup.Token);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        }

        try
        {
            await app.DisposeAsync().AsTask().WaitAsync(cleanup.Token);
        }
        catch (Exception exception)
        {
            failure ??= exception;
        }

        return failure;
    }

    // ---------------------------------------------------------------------------------
    // The shared per-flavour server. Added 2026-09-07, measured; see README section 6.
    //
    // RunAsync above starts a WHOLE APPLICATION per test, which is what a row like ex034
    // needs, because that row grades the learner's own resource graph. Most container
    // rows do not: ex038 and ex040 only want "a real SQL Server with an empty database
    // on it", and paying ~1 m 25 s each for two SQL Server instances that differ in
    // nothing is the difference between a four-minute container lane and a
    // half-hour one at 25 rows.
    //
    // So: ONE server per flavour for the whole assembly, and a fresh DATABASE per test.
    // A database is the isolation boundary that matters - its own catalogue, its own
    // tables, its own __EFMigrationsHistory - and creating one costs milliseconds where
    // creating a server costs a minute. HarnessSmokeTests proves the isolation rather
    // than assuming it: two databases from the SAME server, the same table name in both,
    // and neither can see the other's rows.
    //
    // Three properties this path shares with RunAsync, deliberately and by using the
    // same code rather than by resembling it:
    //   * the builder comes from CreateBuilder(), so the two DCP keys cannot drift;
    //   * teardown goes through TearDownAsync, so the independent cleanup budget, the
    //     "attempt both StopAsync and DisposeAsync" rule and the "keep the first
    //     failure" rule are literally the same lines of code;
    //   * the gate is checked before anything is built.
    //
    // What is NOT shared with RunAsync, on purpose: the server's start is bounded by its
    // OWN deadline and never by the calling test's cancellation token. The server
    // outlives the test that happened to trigger it, so letting that test's token cancel
    // a half-started server would leave the next test to find a broken one.
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// The database flavours the shared-server path knows how to create a database on.
    /// Adding one means teaching <see cref="StartServerAsync"/> how to add the resource
    /// and <see cref="CreateDatabaseAsync"/> how to bring a fresh database into being in
    /// its dialect - CREATE DATABASE for the two relational ones, a first collection for
    /// Mongo, which has no such statement; nothing else changes.
    /// </summary>
    public enum DatabaseFlavour
    {
        SqlServer,
        Postgres,

        /// <summary>
        /// MongoDB. The odd one out, and deliberately so: Mongo has no CREATE DATABASE -
        /// a database springs into existence the first time something is written to it -
        /// so <see cref="CreateDatabaseAsync"/> creates a probe COLLECTION instead. That
        /// keeps the two properties the other two flavours get from CREATE DATABASE: the
        /// database really exists before the test sees it, and the round trip is the
        /// cheapest possible liveness check on a server that may have died mid-suite.
        /// </summary>
        MongoDb
    }

    /// <summary>A fresh, empty database on the assembly's shared server for its flavour.</summary>
    public sealed class SharedDatabase
    {
        internal SharedDatabase(DatabaseFlavour flavour, string name, string connectionString, string serverConnectionString)
        {
            Flavour = flavour;
            Name = name;
            ConnectionString = connectionString;
            ServerConnectionString = serverConnectionString;
        }

        public DatabaseFlavour Flavour { get; }

        /// <summary>The database's own name - unique per call, so two tests never collide.</summary>
        public string Name { get; }

        /// <summary>
        /// The RESOLVED connection string for this database: a real host, the port DCP
        /// allocated for the shared server, the password Aspire generated, and
        /// <see cref="Name"/> as the catalogue.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// The shared server's own connection string, without a catalogue. Two
        /// SharedDatabase instances of one flavour carry the SAME value here and
        /// different <see cref="ConnectionString"/>s - which is what "one server, many
        /// databases" means, and what the isolation canary asserts.
        /// </summary>
        public string ServerConnectionString { get; }
    }

    private sealed class ServerSession
    {
        internal required DistributedApplication Application { get; init; }
        internal required string ConnectionString { get; init; }
    }

    /// <summary>
    /// How long the FIRST caller of a flavour waits for that flavour's server. Its own
    /// budget rather than the test's token - see the block comment above.
    /// </summary>
    private static readonly TimeSpan ServerStartTimeout = TimeSpan.FromMinutes(5);

    private static readonly SemaphoreSlim ServerLock = new(1, 1);
    private static readonly Dictionary<DatabaseFlavour, ServerSession> Servers = [];

    /// <summary>
    /// Hands back a fresh, empty database on the assembly's shared <paramref name="flavour"/>
    /// server, starting that server on first use.
    /// </summary>
    /// <param name="purpose">
    /// A short identifier that ends up in the database name, so that a stray connection
    /// in a log or in <c>sys.databases</c> says which row created it. Non-identifier
    /// characters are stripped.
    /// </param>
    public static async Task<SharedDatabase> DatabaseAsync(
        DatabaseFlavour flavour, string purpose, CancellationToken cancellationToken = default)
    {
        // The same guard RunAsync carries, for the same reason: a 🐳 row that forgot
        // ContainerGate.Require() must not be able to pull an image in the default run.
        if (!ContainerGate.Enabled)
        {
            throw new InvalidOperationException(
                "ContainerHarness.DatabaseAsync was reached with container tests OFF. Every L3 test "
                + "must call ContainerGate.Require() as its first line; this guard exists so a "
                + "missing call cannot make the default `dotnet test` start Docker containers.");
        }

        var server = await ServerAsync(flavour);
        var name = DatabaseName(purpose);

        try
        {
            var connectionString = await CreateDatabaseAsync(
                flavour, server.ConnectionString, name, cancellationToken);

            return new SharedDatabase(flavour, name, connectionString, server.ConnectionString);
        }
        catch (Exception failure) when (!cancellationToken.IsCancellationRequested)
        {
            // The filter matters as much as the body. Without it, a test that was
            // CANCELLED - its own token, or the session deadline - would tear down a
            // perfectly healthy shared server and report "the container died mid-suite",
            // sending the next author to hunt a Docker problem that does not exist. When
            // the caller's token is the one that fired, the cancellation propagates
            // untouched and the server stays up for the next row.
            //
            // Creating the database is the liveness check. It is the first thing every caller
            // does and it is the cheapest possible probe, so a server that died mid-suite
            // - OOM-killed, docker restarted, the daemon bounced - surfaces HERE rather
            // than as an unexplained failure three rows later.
            //
            // Without this the session would stay cached forever and every later
            // container row would fail with a raw SqlException out of CREATE DATABASE,
            // naming a host and a port and nothing about why. Evicting it means the next
            // row starts a fresh server instead, which is the behaviour a per-test
            // application had for free.
            //
            // This method still THROWS rather than retrying: a genuinely broken Docker
            // must not be able to buy itself a five-minute restart per row, and the
            // caller's own failure is the honest signal. Retrying is the elaborate
            // version of this and was deliberately not written.
            await EvictAsync(flavour);

            throw new InvalidOperationException(
                $"The shared {flavour} server did not hand out a fresh database [{name}], so it has "
                + "been evicted and the next container test will start a fresh one. This usually "
                + "means the container died mid-suite - check `docker ps -a` for an exited "
                + $"'shared-{flavour.ToString().ToLowerInvariant()}'.", failure);
        }
    }

    /// <summary>
    /// Drops one flavour's server from the cache and tears it down, so that the next
    /// caller starts a new one. Teardown goes through <see cref="TearDownAsync"/> like
    /// everything else, and its failure is deliberately swallowed: this method only ever
    /// runs while a REAL failure is already on its way up, and that failure must win.
    /// </summary>
    private static async Task EvictAsync(DatabaseFlavour flavour)
    {
        ServerSession? session;
        await ServerLock.WaitAsync();
        try
        {
            Servers.Remove(flavour, out session);
        }
        finally
        {
            ServerLock.Release();
        }

        if (session is not null)
        {
            await TearDownAsync(session.Application, started: true);
        }
    }

    private static async Task<ServerSession> ServerAsync(DatabaseFlavour flavour)
    {
        // The assembly runs serially (TestParallelism.cs), so this lock is never
        // contended in practice. It is here so that the invariant "at most one server per
        // flavour ever starts" is a property of this method rather than of the test
        // runner's configuration.
        await ServerLock.WaitAsync();
        try
        {
            if (Servers.TryGetValue(flavour, out var existing))
            {
                return existing;
            }

            var session = await StartServerAsync(flavour);

            // Cached only on success. A failed start leaves nothing behind, so the next
            // 🐳 test retries rather than inheriting a half-built server - and fails
            // loudly on its own terms if Docker is genuinely unreachable.
            Servers[flavour] = session;
            return session;
        }
        finally
        {
            ServerLock.Release();
        }
    }

    private static async Task<ServerSession> StartServerAsync(DatabaseFlavour flavour)
    {
        var builder = CreateBuilder();
        var resourceName = flavour switch
        {
            DatabaseFlavour.SqlServer => AddSqlServerServer(builder),
            DatabaseFlavour.Postgres => AddPostgresServer(builder),
            DatabaseFlavour.MongoDb => AddMongoDbServer(builder),
            _ => throw new ArgumentOutOfRangeException(nameof(flavour), flavour, null)
        };

        var app = builder.Build();
        var started = false;
        var stopwatch = Stopwatch.StartNew();

        // Its OWN deadline, not the caller's token. See the block comment above.
        using var deadline = new CancellationTokenSource(ServerStartTimeout);

        try
        {
            await app.StartAsync(deadline.Token);
            started = true;

            await app.Services.GetRequiredService<ResourceNotificationService>()
                     .WaitForResourceHealthyAsync(resourceName, deadline.Token);

            var connectionString = await app.GetConnectionStringAsync(resourceName, deadline.Token)
                ?? throw new InvalidOperationException(
                    $"The shared {flavour} server produced no connection string.");

            return new ServerSession { Application = app, ConnectionString = connectionString };
        }
        catch (Exception failure)
        {
            // Nothing is cached, so this application is now unreachable: tear it down
            // here or it leaks. Same contract as everywhere else - the ORIGINAL failure
            // wins, and a teardown failure never replaces it.
            var teardownFailure = await TearDownAsync(app, started);

            if (failure is OperationCanceledException && deadline.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"The shared {flavour} server exceeded {ServerStartTimeout.TotalSeconds:0} s "
                    + $"(it had {(started ? "started" : "NOT finished starting")} after {stopwatch.Elapsed}). "
                    + "Is Docker running, and is the image already pulled?", teardownFailure);
            }

            ExceptionDispatchInfo.Capture(failure).Throw();
            throw; // unreachable; keeps the compiler happy
        }
    }

    private static string AddSqlServerServer(IDistributedApplicationBuilder builder)
    {
        // No AddDatabase child on purpose. The shared server hands out databases through
        // CREATE DATABASE at test time; an Aspire database child would be one fixed
        // database for the whole assembly, which is the thing this path exists to avoid.
        builder.AddSqlServer("shared-sqlserver");
        return "shared-sqlserver";
    }

    private static string AddPostgresServer(IDistributedApplicationBuilder builder)
    {
        builder.AddPostgres("shared-postgres");
        return "shared-postgres";
    }

    private static string AddMongoDbServer(IDistributedApplicationBuilder builder)
    {
        // No AddDatabase child, for the same reason the other two have none: an Aspire
        // database child would be ONE fixed database for the whole assembly. The server
        // resource's connection string carries no database path segment at all, which is
        // exactly what CreateDatabaseAsync needs in order to point MongoUrlBuilder at a
        // fresh name per test.
        builder.AddMongoDB("shared-mongodb");
        return "shared-mongodb";
    }

    private static string DatabaseName(string purpose)
    {
        var cleaned = new string(purpose.Where(char.IsLetterOrDigit).ToArray());
        if (cleaned.Length == 0) cleaned = "db";
        if (cleaned.Length > 20) cleaned = cleaned[..20];

        // A GUID suffix rather than a counter: a counter would make two runs of the same
        // row collide on a persistent server, and would tempt a future author into
        // "reusing" a database. Sixteen hex characters keeps the whole name inside
        // PostgreSQL's 63-byte identifier limit with room to spare.
        return $"{cleaned}_{Guid.NewGuid():N}"[..(cleaned.Length + 17)];
    }

    private static async Task<string> CreateDatabaseAsync(
        DatabaseFlavour flavour, string serverConnectionString, string name, CancellationToken cancellationToken)
    {
        switch (flavour)
        {
            case DatabaseFlavour.SqlServer:
            {
                // The server connection string carries no Initial Catalog, so this lands
                // in master, which is where CREATE DATABASE has to run.
                await using var connection = new SqlConnection(serverConnectionString);
                await connection.OpenAsync(cancellationToken);
                await using var command = new SqlCommand($"CREATE DATABASE [{name}]", connection);
                await command.ExecuteNonQueryAsync(cancellationToken);

                return new SqlConnectionStringBuilder(serverConnectionString)
                {
                    InitialCatalog = name
                }.ConnectionString;
            }

            case DatabaseFlavour.Postgres:
            {
                await using var connection = new NpgsqlConnection(serverConnectionString);
                await connection.OpenAsync(cancellationToken);
                await using var command = new NpgsqlCommand($"CREATE DATABASE \"{name}\"", connection);
                await command.ExecuteNonQueryAsync(cancellationToken);

                return new NpgsqlConnectionStringBuilder(serverConnectionString)
                {
                    Database = name
                }.ConnectionString;
            }

            case DatabaseFlavour.MongoDb:
            {
                // Mongo has no CREATE DATABASE. Creating a collection is what actually
                // brings a database into being, so the probe collection is not decoration:
                // without it the database does not exist until the learner's code writes,
                // and a dead server would not surface here.
                //
                // MongoUrlBuilder, not string surgery. The database name is a PATH SEGMENT
                // in the middle of a URI (README section 6 says so about the model's
                // connection expression, and it is just as true of the resolved one), and
                // Aspire's generated password can contain characters that need
                // percent-encoding - measured: a password containing '}' comes back as
                // %7D. Both are reasons not to concatenate.
                var url = new MongoUrlBuilder(serverConnectionString) { DatabaseName = name }
                          .ToMongoUrl();

                var client = new MongoClient(url);
                var database = client.GetDatabase(name);
                await database.CreateCollectionAsync(ProbeCollectionName, cancellationToken: cancellationToken);

                // The round trip, so that a server that accepted the create but cannot be
                // read from fails HERE rather than three rows later.
                _ = await database.ListCollectionNames().ToListAsync(cancellationToken);

                return url.ToString();
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(flavour), flavour, null);
        }
    }

    /// <summary>
    /// The collection <see cref="CreateDatabaseAsync"/> creates to bring a Mongo database
    /// into existence. Named so that a row asserting on <c>ListCollectionNames</c> can
    /// exclude it knowingly rather than be surprised by it.
    /// </summary>
    public const string ProbeCollectionName = "_harness_probe";

    /// <summary>
    /// Stops every shared server. Called once, after the last test in the assembly, by
    /// <see cref="HarnessLifetime"/> - there is no other hook that runs late enough, and
    /// a per-test <c>finally</c> is exactly what this path exists to avoid.
    ///
    /// Teardown goes through the same <see cref="TearDownAsync"/> every RunAsync session
    /// uses, so the contract is identical rather than merely similar. Failures are
    /// collected and rethrown together: a server that would not stop is a leak, and a
    /// leak must be loud.
    /// </summary>
    public static async Task ShutdownSharedServersAsync()
    {
        List<Exception>? failures = null;

        // Snapshot and clear first, so that a failure here cannot leave a stopped server
        // in the dictionary for some later caller to hand out databases on.
        await ServerLock.WaitAsync();
        List<KeyValuePair<DatabaseFlavour, ServerSession>> sessions;
        try
        {
            sessions = Servers.ToList();
            Servers.Clear();
        }
        finally
        {
            ServerLock.Release();
        }

        foreach (var (flavour, session) in sessions)
        {
            var failure = await TearDownAsync(session.Application, started: true);
            if (failure is not null)
            {
                failures ??= [];
                failures.Add(new InvalidOperationException(
                    $"The shared {flavour} server did not tear down. Check `docker ps -a`, "
                    + "`docker network ls` and `docker volume ls` for leftovers.", failure));
            }
        }

        if (failures is not null)
        {
            throw failures.Count == 1 ? failures[0] : new AggregateException(failures);
        }
    }

    /// <summary>
    /// Harness-only. Whether a shared server for <paramref name="flavour"/> is currently
    /// running - the canary facts use it to assert that the second caller did NOT start
    /// a second one.
    /// </summary>
    internal static bool HasSharedServer(DatabaseFlavour flavour)
        // Unsynchronised on purpose: the assembly runs serially (TestParallelism.cs), so
        // no test can be mutating this dictionary while a canary reads it.
        => Servers.ContainsKey(flavour);
}
