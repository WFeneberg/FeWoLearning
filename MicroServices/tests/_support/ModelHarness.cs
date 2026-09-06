using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Builds an Aspire application model in-process and hands back its resources.
/// Measured at ~1.4 s and starts NO containers - this is the workhorse of the
/// track's L1 assertions.
/// </summary>
public static class ModelHarness
{
    public sealed class Result
    {
        private readonly IReadOnlyList<IResource> _resources;

        internal Result(IReadOnlyList<IResource> resources) => _resources = resources;

        public IReadOnlyList<IResource> Resources => _resources;

        /// <summary>Resource by name, with a failure message that lists what IS there.</summary>
        public IResource Resource(string name)
            => _resources.SingleOrDefault(r => r.Name == name)
               ?? throw new InvalidOperationException(
                   $"No resource named '{name}'. Model contains: " +
                   string.Join(", ", _resources.Select(r => $"{r.Name}({r.GetType().Name})")));

        public bool Has(string name) => _resources.Any(r => r.Name == name);
    }

    public static Result Build(Action<IDistributedApplicationBuilder> configure)
    {
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = [],
            DisableDashboard = true
        });
        configure(builder);
        using var app = builder.Build();
        return new Result(builder.Resources.ToList());
    }

    /// <summary>
    /// The same model, built with the PUBLISH execution context instead of the run
    /// one, so that <c>builder.ExecutionContext.IsPublishMode</c> is true while a
    /// resource graph is being assembled. It passes the same "--operation publish"
    /// arguments <see cref="ManifestHarness"/> uses and then STOPS at Build(): there
    /// is no RunAsync, so no publisher executes and - measured on 13.5.3 - the
    /// output path is never even created.
    ///
    /// This is what a row about mode-dependent modelling needs and neither other
    /// harness provides: <see cref="Build"/> is run mode, and ManifestHarness gives
    /// back the published artifact rather than the graph behind it.
    ///
    /// The output path is a per-call GUID under <see cref="ManifestHarness.Root"/>
    /// even though nothing writes to it. That is deliberate: "Aspire does not create
    /// this directory" is a measured fact about 13.5.3, not a promise, and a future
    /// version that created it eagerly would leak a directory per call. Under that
    /// root it is swept with everything else; under a fixed path of its own it would
    /// not be.
    /// </summary>
    public static Result BuildForPublish(Action<IDistributedApplicationBuilder> configure)
    {
        var outputPath = Path.Combine(ManifestHarness.Root, Guid.NewGuid().ToString("N")[..12]);
        var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
        {
            Args = ["--operation", "publish", "--output-path", outputPath],
            DisableDashboard = true
        });
        configure(builder);
        using var app = builder.Build();
        return new Result(builder.Resources.ToList());
    }

    /// <summary>
    /// The connection-string EXPRESSION, not a resolved value. It differs per
    /// database flavour, which is what lets a test prove the learner wired up
    /// PostgreSQL rather than merely some container (spec section 8.2).
    /// </summary>
    public static string ConnectionString(IResource resource)
        => resource is IResourceWithConnectionString cs
            ? cs.ConnectionStringExpression.ValueExpression
            : throw new InvalidOperationException(
                $"Resource '{resource.Name}' ({resource.GetType().Name}) has no connection string.");
}
