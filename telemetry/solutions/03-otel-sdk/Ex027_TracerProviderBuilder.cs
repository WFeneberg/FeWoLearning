using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 027 — TracerProviderBuilder (otel-sdk).
// Goal:   Meet the SDK, and see that underneath it is the same ActivityListener from
//         row 015.
// Drills: Sdk.CreateTracerProviderBuilder, AddSource, the in-memory exporter,
//         provider lifetime.
// Passes: before a provider exists, the registered source returns null - nothing is
//                     listening yet;
//         with a provider built, work on the registered source exports exactly one
//                     span, named and attributed as written;
//         work on a source that was NOT registered exports nothing and returns null,
//                     even though that source exists and is used;
//         and after the provider is disposed, the registered source returns null again.
//
// Those four clauses are one idea seen from four sides: the provider IS the listener.
// It is not a background service that collects whatever happens; it is a subscription,
// and outside its lifetime your instrumentation is inert. This is why an application
// that builds its provider after the first request loses that request, why a provider
// disposed at shutdown takes the shutdown spans with it, and why a source nobody
// registered produces nothing at all rather than something unexported.
//
// AddSource takes a NAME, not an object. The source and the registration are matched by
// string, so a typo produces silence and no error - the single most common reason a
// custom ActivitySource "does not work".
//
// The in-memory exporter is a test instrument, not a teaching simplification: the same
// pipeline, with OTLP in its place, is what runs in production.
public static class Ex027_TracerProviderBuilder
{
    /// <summary>The source the provider is told about.</summary>
    public const string RegisteredSourceName = "fewolearning.telemetry.ex027";

    /// <summary>A source that exists, is used, and is never registered.</summary>
    public const string UnregisteredSourceName = "fewolearning.telemetry.ex027.unregistered";

    /// <summary>The tag every work span carries.</summary>
    public const string WorkTag = "work.kind";

    /// <summary>The name every work span carries.</summary>
    public const string WorkSpanName = "work";

    /// <summary>Registered with the provider.</summary>
    public static ActivitySource Registered { get; } = new(RegisteredSourceName);

    /// <summary>Never registered. Present so its silence can be observed.</summary>
    public static ActivitySource Unregistered { get; } = new(UnregisteredSourceName);

    /// <summary>
    /// Build a <see cref="TracerProvider"/> that listens to
    /// <see cref="RegisteredSourceName"/> and exports finished spans into
    /// <paramref name="exported"/>.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            // By NAME. The source object is never handed over, so a typo here is
            // silence with no error attached.
            .AddSource(RegisteredSourceName)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Start and stop one <see cref="WorkSpanName"/> activity on
    /// <paramref name="source"/>, tagged <see cref="WorkTag"/> with
    /// <paramref name="kind"/>, and return it. Null when nothing is listening.
    /// </summary>
    public static Activity? DoWork(ActivitySource source, string kind)
    {
        using var activity = source.StartActivity(WorkSpanName);
        activity?.SetTag(WorkTag, kind);

        return activity;
    }
}
