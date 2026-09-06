using System.Net;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex043_PrometheusScrapeEndpointTests
{
    private static async Task<(HttpStatusCode Status, string Body)> Scrape(params string[] outcomes)
    {
        await using var web = await WebProbe.StartAsync(
            Ex043_PrometheusScrapeEndpoint.ConfigureMetrics,
            _ => { },
            Ex043_PrometheusScrapeEndpoint.UseScrapeEndpoint);

        foreach (var outcome in outcomes) Ex043_PrometheusScrapeEndpoint.RecordProcessed(outcome);

        var response = await web.Client.GetAsync(Ex043_PrometheusScrapeEndpoint.ScrapePath);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task The_scrape_endpoint_answers_with_the_exposition_text()
    {
        var (status, body) = await Scrape("accepted");

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.NotEmpty(body);
    }

    [Fact]
    public async Task Adversarial_A_The_instrument_is_renamed_to_prometheus_conventions()
    {
        // Where hand-written dashboards break. Prometheus has its own naming rules and the
        // exporter rewrites your instrument to fit them: dots become underscores, and a
        // monotonic counter gains a _total suffix. So the name in your code is NOT the
        // name in the query - and looking up the one you wrote returns nothing at all,
        // not an error, just an empty graph.
        var (_, body) = await Scrape("accepted");

        Assert.Contains(Ex043_PrometheusScrapeEndpoint.ScrapedCounterName, body);
        Assert.DoesNotContain(Ex043_PrometheusScrapeEndpoint.CounterInstrument, body);
    }

    [Fact]
    public async Task The_document_declares_the_metrics_type()
    {
        var (_, body) = await Scrape("accepted");

        Assert.Contains(
            $"# TYPE {Ex043_PrometheusScrapeEndpoint.ScrapedCounterName} counter",
            body);
    }

    [Fact]
    public async Task Adversarial_C_The_document_carries_help_text_for_the_metric()
    {
        // This fact exists because the CONTAINER fact below found what the four
        // assertions around it missed. The exporter emits a # HELP line only when the
        // instrument was given a description, and promtool lints a metric without help
        // text as invalid - so the first version of this row produced a document that
        // passed every in-process check and a real Prometheus rejects.
        //
        // Which is the whole argument for the 🐳 rows in one example: a strict reader
        // finds the thing you did not know to look for.
        var (_, body) = await Scrape("accepted");

        Assert.Contains(
            $"# HELP {Ex043_PrometheusScrapeEndpoint.ScrapedCounterName}",
            body);
        Assert.Contains(Ex043_PrometheusScrapeEndpoint.CounterDescription, body);
    }

    [Fact]
    public async Task Adversarial_B_Dimensions_become_labels_and_separate_the_series()
    {
        // The paired use fact. An exporter that flattened everything into one number
        // would satisfy the naming facts above and lose the reason to have dimensions.
        var (_, body) = await Scrape("accepted", "accepted", "rejected");

        Assert.Contains($"{Ex043_PrometheusScrapeEndpoint.OutcomeTag}=\"accepted\"", body);
        Assert.Contains($"{Ex043_PrometheusScrapeEndpoint.OutcomeTag}=\"rejected\"", body);
    }

    [Fact]
    public async Task Container_promtool_accepts_the_whole_document()
    {
        // 🐳 Skipped unless the run passes -p:Containers=true.
        //
        // A far stricter reader than any assertion here: promtool validates the entire
        // grammar of the exposition format, not the three lines a test thought to check.
        // Everything above is graded without Docker.
        ContainerGate.SkipUnlessEnabled();

        var (_, body) = await Scrape("accepted", "rejected");

        var (exitCode, output) = await PromtoolContainer.CheckMetrics(body);

        Assert.True(exitCode == 0, $"promtool rejected the document:\n{output}\n\n---\n{body}");
    }
}
