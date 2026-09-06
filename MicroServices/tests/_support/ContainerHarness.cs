using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

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

        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = [],
            DisableDashboard = true
        });

        // See point 3 in the type comment: required even with the dashboard disabled.
        builder.Configuration["DcpPublisher:DashboardPath"] = "not-used-the-dashboard-is-disabled";
        // See point 4: without this the per-session Docker network outlives the run.
        builder.Configuration["DcpPublisher:WaitForResourceCleanup"] = "true";

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
}
