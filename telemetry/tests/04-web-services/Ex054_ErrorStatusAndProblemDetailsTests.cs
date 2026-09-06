using System.Diagnostics;
using System.Net;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex054_ErrorStatusAndProblemDetailsTests
{
    private sealed record Result(
        HttpStatusCode Status,
        string Body,
        string? ContentType,
        List<Activity> Spans,
        List<LogRecord> Logs);

    private static async Task<Result> Request(string path)
    {
        var spans = new List<Activity>();
        var logs = new List<LogRecord>();

        await using var web = await WebProbe.StartAsync(
            services => Ex054_ErrorStatusAndProblemDetails.ConfigureTelemetry(services, spans, logs),
            Ex054_ErrorStatusAndProblemDetails.MapEndpoints,
            app => app.Use(async (context, next) =>
            {
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger(Ex054_ErrorStatusAndProblemDetails.CategoryName);

                await Ex054_ErrorStatusAndProblemDetails.HandleAsync(
                    context, _ => next(context), logger);
            }));

        var response = await web.Client.GetAsync(path);
        web.Services.GetRequiredService<TracerProvider>().ForceFlush();

        return new Result(
            response.StatusCode,
            await response.Content.ReadAsStringAsync(),
            response.Content.Headers.ContentType?.MediaType,
            spans,
            logs);
    }

    [Fact]
    public async Task A_failing_request_answers_a_problem_document()
    {
        using var ctx = new TelemetryContext();

        var result = await Request(Ex054_ErrorStatusAndProblemDetails.FailingRoute);

        Assert.Equal(HttpStatusCode.InternalServerError, result.Status);
        Assert.Equal("application/problem+json", result.ContentType);
        Assert.Contains(Ex054_ErrorStatusAndProblemDetails.ProblemTitle, result.Body);
        Assert.Contains("500", result.Body);
    }

    [Fact]
    public async Task Adversarial_A_The_response_leaks_nothing_about_the_exception()
    {
        // The clause with a CVE attached to the version of it that goes wrong. An
        // unhandled exception rendered into the response hands an attacker your stack
        // trace, your file paths, your ORM, sometimes your connection string.
        //
        // The caller needs to know that it failed and what to do; everything else belongs
        // in the telemetry, which is behind your authentication rather than in front of it.
        using var ctx = new TelemetryContext();

        var result = await Request(Ex054_ErrorStatusAndProblemDetails.FailingRoute);

        Assert.DoesNotContain("hunter2", result.Body);
        Assert.DoesNotContain(Ex054_ErrorStatusAndProblemDetails.SecretExceptionMessage, result.Body);
        Assert.DoesNotContain(nameof(InvalidOperationException), result.Body);
        Assert.DoesNotContain("at ", result.Body);
    }

    [Fact]
    public async Task Adversarial_B_The_failure_reaches_the_span_and_the_log_together()
    {
        // The other half of the split: the RESPONSE says what the caller can act on, the
        // TELEMETRY says what you can debug. The trace id is what lets a support engineer
        // walk from a customer's screenshot to the second.
        using var ctx = new TelemetryContext();

        var result = await Request(Ex054_ErrorStatusAndProblemDetails.FailingRoute);

        var span = Assert.Single(result.Spans);
        Assert.Equal(ActivityStatusCode.Error, span.Status);
        Assert.Contains(span.Events, e => e.Name == "exception");

        var log = Assert.Single(result.Logs, r => r.LogLevel >= LogLevel.Error);
        Assert.Equal(span.TraceId, log.TraceId);
        Assert.NotNull(log.Exception);
    }

    [Fact]
    public async Task Adversarial_C_A_successful_request_produces_none_of_it()
    {
        // The paired half, and it catches the implementation that marks everything Error
        // and logs everything as a failure - which is not observability, it is noise that
        // trains everyone to ignore the alert.
        using var ctx = new TelemetryContext();

        var result = await Request("/orders/42");

        Assert.Equal(HttpStatusCode.OK, result.Status);
        var span = Assert.Single(result.Spans);
        Assert.NotEqual(ActivityStatusCode.Error, span.Status);
        Assert.DoesNotContain(span.Events, e => e.Name == "exception");
        Assert.DoesNotContain(result.Logs, r => r.LogLevel >= LogLevel.Error);
    }
}
