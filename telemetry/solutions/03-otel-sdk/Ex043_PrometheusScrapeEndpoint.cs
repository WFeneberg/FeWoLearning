using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 043 — PrometheusScrapeEndpoint (otel-sdk). 🐳
// Goal:   Expose metrics the other way round - not pushed to a collector, but sitting
//         on a URL waiting to be read.
// Drills: the Prometheus exporter, the text exposition format, name mangling.
// Passes: GET /metrics answers 200 with the exposition text;
//         the counter "orders.processed" appears as "orders_processed_total" - dots
//                     become underscores and a monotonic counter gains the _total
//                     suffix;
//         its dimensions appear as labels, and its # TYPE line says counter;
//         and 🐳 a real promtool accepts the whole document.
//
// Pull is the older model and it has not lost. A scrape endpoint has no queue to fill, no
// exporter to configure and no credentials to leak; the process holds its numbers and
// something else decides when to look. What it costs is reachability - Prometheus has to
// be able to open a connection to every replica - which is why push wins in a serverless
// or heavily-firewalled world and pull wins almost everywhere else.
//
// The second clause is where hand-written dashboards break. Prometheus has its own naming
// rules and the exporter rewrites your instrument to fit them: dots to underscores, a
// unit suffix if you declared one, and _total on anything monotonic. So the name in your
// code is NOT the name in the query, and looking up the one you wrote returns nothing at
// all - not an error, just an empty graph.
//
// The 🐳 fact is skipped unless the run passes -p:Containers=true. It pipes the document
// through promtool inside a real Prometheus image, which is a far stricter reader than
// any assertion here: it validates the whole grammar, not the three lines a test thought
// to check.
public static class Ex043_PrometheusScrapeEndpoint
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex043";

    /// <summary>What the code calls the instrument.</summary>
    public const string CounterInstrument = "orders.processed";

    /// <summary>What Prometheus will call it.</summary>
    public const string ScrapedCounterName = "orders_processed_total";

    /// <summary>The dimension that becomes a label.</summary>
    public const string OutcomeTag = "outcome";

    /// <summary>
    /// The instrument's description, and it is not decoration: the exporter emits a
    /// # HELP line only when there is one, and promtool LINTS a metric without help text
    /// as invalid. Measured 2026-09-06 - all four of this row's in-process facts passed
    /// against a document a real Prometheus rejects.
    /// </summary>
    public const string CounterDescription = "Orders that finished processing.";

    /// <summary>Where the scrape endpoint lives.</summary>
    public const string ScrapePath = "/metrics";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Register metrics for <see cref="MeterName"/> with the Prometheus exporter.
    /// </summary>
    private static readonly Counter<long> Processed =
        // The description is what produces the # HELP line, and without it promtool
        // rejects the whole document.
        Meter.CreateCounter<long>(CounterInstrument, unit: null, description: CounterDescription);

    public static void ConfigureMetrics(IServiceCollection services) =>
        services.AddOpenTelemetry().WithMetrics(metrics => metrics
            .AddMeter(MeterName)
            .AddPrometheusExporter());

    /// <summary>
    /// Put the scraping endpoint into the pipeline, answering on
    /// <see cref="ScrapePath"/>.
    /// </summary>
    public static void UseScrapeEndpoint(IApplicationBuilder app) =>
        app.UseOpenTelemetryPrometheusScrapingEndpoint();

    /// <summary>
    /// Add 1 to a <see cref="long"/> counter named <see cref="CounterInstrument"/>,
    /// described by <see cref="CounterDescription"/> and tagged <see cref="OutcomeTag"/>.
    /// One instrument, created once.
    /// </summary>
    public static void RecordProcessed(string outcome) =>
        Processed.Add(1, new KeyValuePair<string, object?>(OutcomeTag, outcome));
}
