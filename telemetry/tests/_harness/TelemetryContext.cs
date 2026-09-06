using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Resets the diagnostics process-globals around one test. Construct it first in any
/// test that touches <see cref="Activity"/>, <see cref="Baggage"/> or a propagator;
/// the reset runs on both construction and disposal, so a test cannot inherit a
/// neighbour's leak nor leave one behind.
///
/// This is the second half of the parallelism story. <c>AssemblyInfo.cs</c> stops two
/// tests running at once; this stops one test's leftovers reaching the next.
/// </summary>
public sealed class TelemetryContext : IDisposable
{
    private static readonly ActivityIdFormat PristineIdFormat;
    private static readonly bool PristineForceIdFormat;
    private static readonly TextMapPropagator PristinePropagator;

    // An EXPLICIT static constructor, never field initializers. A field initializer
    // leaves the type `beforefieldinit`, which lets the runtime defer initialization
    // until the first read of that field - and that read happens AFTER the instance
    // constructor's reset, so the snapshot captures the already-reset values.
    // Measured on this machine while building caliburn/'s CaliburnCoreContext.
    static TelemetryContext()
    {
        PristineIdFormat = Activity.DefaultIdFormat;
        PristineForceIdFormat = Activity.ForceDefaultIdFormat;
        PristinePropagator = Propagators.DefaultTextMapPropagator;
    }

    public TelemetryContext() => Reset();

    public void Dispose() => Reset();

    private static void Reset()
    {
        Activity.Current = null;
        Activity.DefaultIdFormat = PristineIdFormat;
        Activity.ForceDefaultIdFormat = PristineForceIdFormat;
        Sdk.SetDefaultTextMapPropagator(PristinePropagator);
        Baggage.Current = default;
    }
}
