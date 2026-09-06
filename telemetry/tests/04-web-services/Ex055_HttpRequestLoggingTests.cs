using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex055_HttpRequestLoggingTests
{
    /// <summary>How long the endpoint pretends to work for.</summary>
    private static readonly TimeSpan Work = TimeSpan.FromMilliseconds(60);

    private static async Task<IReadOnlyList<FakeLogRecord>> Request(string path)
    {
        using var logs = new LogProbe();

        await using var web = await WebProbe.StartAsync(
            // Deliberately NOT registering the probe as the host's ILoggerFactory: doing
            // so captured every framework record too - Hosting starting, route matching,
            // the lot - and Assert.Single then saw eleven. The middleware below uses the
            // probe directly, so nothing needs to be in DI at all.
            _ => { },
            endpoints =>
            {
                endpoints.MapGet("/search", async () =>
                {
                    await Task.Delay(Work);
                    return "results";
                });

                // The explicit return type matters: a bare `() => throw` is inferred as
                // a RequestDelegate and fails to compile with CS1593.
                endpoints.MapGet("/explode", string () =>
                    throw new InvalidOperationException("boom"));
            },
            app => app.Use(async (context, next) =>
            {
                var logger = logs.For(Ex055_HttpRequestLogging.CategoryName);

                try
                {
                    await Ex055_HttpRequestLogging.LogRequestAsync(context, _ => next(context), logger);
                }
                catch (InvalidOperationException)
                {
                    // Swallowed HERE, outside the exercise, so the test host does not
                    // fail the request - the exercise's own job is to log and rethrow.
                    context.Response.StatusCode = 500;
                }
            }));

        await web.Client.GetAsync(path);

        return logs.Records;
    }

    [Fact]
    public async Task One_record_per_request_carrying_method_path_status_and_duration()
    {
        var records = await Request("/search");

        var record = Assert.Single(records);
        Assert.Equal("GET", LogProbe.Field(record, Ex055_HttpRequestLogging.MethodField));
        Assert.Equal("/search", LogProbe.Field(record, Ex055_HttpRequestLogging.PathField));
        Assert.Equal("200", LogProbe.Field(record, Ex055_HttpRequestLogging.StatusField));
        Assert.NotNull(LogProbe.Field(record, Ex055_HttpRequestLogging.DurationField));
    }

    [Fact]
    public async Task Adversarial_A_The_duration_covers_the_whole_pipeline()
    {
        // The one that produces a graph everybody trusts and nobody should. Start the
        // stopwatch AFTER `await next(context)` and you have measured the time to write a
        // response header; start it before and stop it after, and you have measured the
        // request. The difference is invisible in the code and roughly the whole latency
        // of your service.
        var records = await Request("/search");

        var duration = double.Parse(
            LogProbe.Field(Assert.Single(records), Ex055_HttpRequestLogging.DurationField)!,
            System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(
            duration >= Work.TotalMilliseconds * 0.8,
            $"the endpoint slept {Work.TotalMilliseconds}ms and the log says {duration}ms");
    }

    [Fact]
    public async Task Adversarial_B_A_failing_request_is_still_logged()
    {
        // The same shape as row 049: a middleware that logs only on the happy path loses
        // exactly the requests worth having a log for. Put it in a finally.
        var records = await Request("/explode");

        var record = Assert.Single(records);
        Assert.Equal("/explode", LogProbe.Field(record, Ex055_HttpRequestLogging.PathField));
    }

    [Fact]
    public async Task Adversarial_C_The_query_string_never_reaches_the_log()
    {
        // A query string is caller-controlled and routinely carries a search term, an
        // email address, a password somebody put in a URL by mistake, or a token from a
        // badly built redirect. Logging it whole copies all of that into a system with a
        // different access model and a longer retention.
        //
        // Checked across EVERY field, not just the path: appending it to the message or
        // adding a helpful "QueryString" field leaks exactly as much.
        var records = await Request("/search?q=ada@example.com&token=hunter2&page=3");

        var record = Assert.Single(records);

        Assert.Equal("/search", LogProbe.Field(record, Ex055_HttpRequestLogging.PathField));
        Assert.All(record.StructuredState ?? [], pair =>
        {
            Assert.DoesNotContain("ada@example.com", pair.Value ?? string.Empty);
            Assert.DoesNotContain("hunter2", pair.Value ?? string.Empty);
        });
        Assert.DoesNotContain("hunter2", record.Message);
    }

    [Fact]
    public async Task Adversarial_D_The_parameter_that_was_allowed_is_still_recorded()
    {
        // The paired half, and the pair is the point. Logging NOTHING from the query makes
        // the access log useless for the thing it is for - "GET /search" with no terms
        // answers no question anybody has.
        //
        // So: an allowlist. Name the parameters you meant, record those, drop the rest
        // unread.
        var records = await Request("/search?q=ada@example.com&page=3");

        Assert.Equal("3", LogProbe.Field(Assert.Single(records), Ex055_HttpRequestLogging.PageField));
    }
}
