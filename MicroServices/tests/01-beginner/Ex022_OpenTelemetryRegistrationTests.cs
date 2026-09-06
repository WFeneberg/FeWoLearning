using System.Diagnostics;
using FeWoLearning.MicroServices.Exercises.Beginner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex022_OpenTelemetryRegistrationTests
{
    /// <summary>
    /// The learner's registration, plus an in-memory exporter on each signal so the test
    /// can read what actually made it through. The exporters are added AFTER the
    /// learner's call on purpose: a second AddOpenTelemetry() on the same collection
    /// configures the same providers, so this cannot accidentally supply the AddSource /
    /// AddMeter the exercise is about.
    /// </summary>
    private sealed class Capture : IDisposable
    {
        public IHost Host { get; }
        public List<Activity> Spans { get; } = [];
        public List<Metric> Metrics { get; } = [];

        private readonly TracerProvider _tracer;
        private readonly MeterProvider _meter;

        public Capture()
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
            builder.Logging.ClearProviders();

            builder.AddOrdersTelemetry();

            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddInMemoryExporter(Spans))
                .WithMetrics(metrics => metrics.AddInMemoryExporter(Metrics));

            Host = builder.Build();

            // Resolving the providers is what installs the ActivityListener and the
            // MeterListener. Nothing is recorded before this line.
            _tracer = Host.Services.GetRequiredService<TracerProvider>();
            _meter = Host.Services.GetRequiredService<MeterProvider>();
        }

        public void Flush()
        {
            _tracer.ForceFlush();
            _meter.ForceFlush();
        }

        public void Dispose() => Host.Dispose();
    }

    [Fact]
    public void The_registered_source_produces_a_span_that_reaches_the_exporter()
    {
        using var capture = new Capture();

        using (var activity = Ex022_OpenTelemetryRegistration.Orders.StartActivity("place-order"))
        {
            // The sharpest fact in the row, and it is not about export at all: with no
            // listener, StartActivity returns null and there is no span to export, tag
            // or correlate. AddSource is what creates the listener.
            Assert.NotNull(activity);
        }

        capture.Flush();

        var span = Assert.Single(capture.Spans);
        Assert.Equal("place-order", span.OperationName);
        Assert.Equal(Ex022_OpenTelemetryRegistration.OrdersSourceName, span.Source.Name);
    }

    [Fact]
    public void A_source_nobody_registered_produces_no_span_at_all()
    {
        using var capture = new Capture();

        using (var activity = Ex022_OpenTelemetryRegistration.Legacy.StartActivity("legacy-call"))
        {
            // The negative half, and the only thing that rejects AddSource("*"). Measured
            // on OpenTelemetry 1.18.0: under a wildcard registration this Activity is
            // NOT null and the span below arrives - so a row with only the positive fact
            // grades "some tracing happened", not "the learner named their source".
            Assert.Null(activity);
        }

        Ex022_OpenTelemetryRegistration.LegacyCalls.Add(1);
        capture.Flush();

        Assert.DoesNotContain(capture.Spans,
            s => s.Source.Name == Ex022_OpenTelemetryRegistration.LegacySourceName);
        Assert.DoesNotContain(capture.Metrics,
            m => m.MeterName == Ex022_OpenTelemetryRegistration.LegacySourceName);
    }

    [Fact]
    public void The_registered_meter_exports_its_counter()
    {
        using var capture = new Capture();

        Ex022_OpenTelemetryRegistration.OrdersPlaced.Add(3);
        capture.Flush();

        var metric = Assert.Single(capture.Metrics,
            m => m.MeterName == Ex022_OpenTelemetryRegistration.OrdersSourceName);
        Assert.Equal("orders.placed", metric.Name);
    }

    [Fact]
    public void Nothing_here_is_graded_through_the_OTEL_environment_keys()
    {
        // Stated as a fact so it cannot quietly stop being true. Aspire sets
        // OTEL_EXPORTER_OTLP_ENDPOINT and OTEL_SERVICE_NAME on every resource itself, so
        // an implementation that only writes configuration - and never calls AddSource
        // or AddMeter - must still fail this row. Here the configuration is set FOR the
        // learner, and the spans still have to arrive on their own merits.
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:4317";
        builder.Configuration["OTEL_SERVICE_NAME"] = "orders";

        builder.AddOrdersTelemetry();

        var spans = new List<Activity>();
        builder.Services.AddOpenTelemetry().WithTracing(t => t.AddInMemoryExporter(spans));

        using var host = builder.Build();
        var tracer = host.Services.GetRequiredService<TracerProvider>();

        using (Ex022_OpenTelemetryRegistration.Orders.StartActivity("place-order")) { }
        tracer.ForceFlush();

        Assert.Single(spans);
    }
}
