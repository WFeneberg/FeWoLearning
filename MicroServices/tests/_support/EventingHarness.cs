using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Builds an application model and KEEPS the <see cref="DistributedApplication"/> alive,
/// so a test can publish lifecycle events by hand and watch the learner's subscriptions
/// fire. <see cref="ModelHarness"/> cannot do this: it disposes the app before it
/// returns, and it hands back resources rather than the service provider the events
/// carry.
///
/// Nothing here starts anything. Aspire never runs, no container is pulled, no
/// orchestrator exists - the test IS the orchestrator, publishing the same events in the
/// same order Aspire would. That is what makes an eventing row deterministic: there is
/// no wall clock to wait on and no race to lose.
///
/// Measured on 13.5.3, and the reason this type exists at all:
/// <list type="bullet">
/// <item><c>builder.Eventing</c> and
/// <c>app.Services.GetRequiredService&lt;IDistributedApplicationEventing&gt;()</c> are the
/// SAME instance, so a subscription made while the graph was assembled is live on the
/// built app.</item>
/// <item>Publishing <c>BeforeStartEvent</c> also runs Aspire's OWN built-in subscriber,
/// <c>InitializeDcpAnnotations</c>, which reads the DCP options and throws
/// <c>OptionsValidationException</c> ("The path to the DCP executable ... is required")
/// under a harness, before any learner subscription is reached. The two configuration
/// keys below are placeholders that satisfy the validator; nothing ever executes them,
/// because nothing is started.</item>
/// </list>
/// </summary>
public static class EventingHarness
{
    public sealed class Session : IDisposable
    {
        private readonly DistributedApplication _app;

        internal Session(DistributedApplication app, IDistributedApplicationBuilder builder)
        {
            _app = app;
            Eventing = app.Services.GetRequiredService<IDistributedApplicationEventing>();
            Model = app.Services.GetRequiredService<DistributedApplicationModel>();
            Resources = builder.Resources.ToList();
        }

        public IDistributedApplicationEventing Eventing { get; }
        public DistributedApplicationModel Model { get; }
        public IServiceProvider Services => _app.Services;
        public IReadOnlyList<IResource> Resources { get; }

        public IResource Resource(string name)
            => Resources.SingleOrDefault(r => r.Name == name)
               ?? throw new InvalidOperationException(
                   $"No resource named '{name}'. Model contains: " +
                   string.Join(", ", Resources.Select(r => $"{r.Name}({r.GetType().Name})")));

        /// <summary>Publishes one event exactly as the orchestrator would.</summary>
        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
            where T : IDistributedApplicationEvent
            => Eventing.PublishAsync(@event, cancellationToken);

        public void Dispose() => _app.Dispose();
    }

    public static Session Build(
        Action<IDistributedApplicationBuilder> configure, bool publishMode = false)
    {
        var args = publishMode
            ? new[] { "--operation", "publish", "--output-path",
                      Path.Combine(ManifestHarness.Root, Guid.NewGuid().ToString("N")[..12]) }
            : [];

        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = args,
            DisableDashboard = true
        });

        // See the type comment: without these two, publishing BeforeStartEvent dies in
        // Aspire's own built-in subscriber before a learner subscription is reached.
        builder.Configuration["DcpPublisher:CliPath"] = "not-used-nothing-is-started";
        builder.Configuration["DcpPublisher:DashboardPath"] = "not-used-nothing-is-started";

        configure(builder);
        return new Session(builder.Build(), builder);
    }
}
