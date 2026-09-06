using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 054 — ErrorStatusAndProblemDetails (web-services).
// Goal:   Make one failure produce one coherent story across all three signals, and a
//         response a caller can act on.
// Drills: exception handling middleware, span status, ProblemDetails, log/trace
//         correlation.
// Passes: a failing request answers 500 with a problem+json body carrying a title and a
//                     status;
//         its server span ends with status Error and an exception event;
//         a log record is written whose trace id is that span's, so the three signals
//                     point at each other;
//         the response body contains no exception message, type or stack trace;
//         and a successful request produces none of it - 200, no error status, no error
//                     log.
//
// The fourth clause is the one with a CVE attached to the version of it that goes wrong.
// An unhandled exception rendered into the response hands an attacker your stack trace,
// your file paths, your ORM, sometimes your connection string. The caller needs to know
// that it failed and what to do; everything else belongs in the telemetry, which is
// behind your authentication rather than in front of it.
//
// So the split is: the RESPONSE says what the caller can act on, the TELEMETRY says what
// you can debug. ProblemDetails is a format for the first half, and the trace id is what
// lets a support engineer walk from a customer's screenshot to the second.
//
// The fifth clause is the paired half, and it catches the implementation that just marks
// everything Error and logs everything as a failure - which is not observability, it is
// noise that trains everyone to ignore the alert.
public static class Ex054_ErrorStatusAndProblemDetails
{
    /// <summary>The category the handler logs under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex054";

    /// <summary>The route that works.</summary>
    public const string WorkingRoute = "/orders/{id}";

    /// <summary>The route that throws.</summary>
    public const string FailingRoute = "/orders/explode";

    /// <summary>What the handler puts in the problem's title.</summary>
    public const string ProblemTitle = "The request could not be completed.";

    /// <summary>What the failing endpoint throws.</summary>
    public const string SecretExceptionMessage = "connection string Server=db;Password=hunter2 is unreachable";

    /// <summary>
    /// Register tracing for the ASP.NET Core instrumentation into
    /// <paramref name="exportedSpans"/>, and logging into
    /// <paramref name="exportedLogs"/>.
    /// </summary>
    public static void ConfigureTelemetry(
        IServiceCollection services,
        ICollection<Activity> exportedSpans,
        ICollection<LogRecord> exportedLogs) =>
        throw new NotImplementedException(
            "TODO: Ex054 - register tracing and logging into these two exporters");

    /// <summary>
    /// Map <see cref="WorkingRoute"/> - which returns the order - and
    /// <see cref="FailingRoute"/>, which throws an
    /// <see cref="InvalidOperationException"/> carrying
    /// <see cref="SecretExceptionMessage"/>.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder endpoints) =>
        throw new NotImplementedException("TODO: Ex054 - map a working endpoint and a failing one");

    /// <summary>
    /// The middleware that turns an escaping exception into a response.
    ///
    /// It must: mark <see cref="Activity.Current"/> with
    /// <see cref="ActivityStatusCode.Error"/> and record the exception on it; write one
    /// Error log record through <paramref name="logger"/>, passing the exception as the
    /// exception argument; answer 500 with a <c>application/problem+json</c> body whose
    /// <c>title</c> is <see cref="ProblemTitle"/> and whose <c>status</c> is 500 - and
    /// which mentions nothing about the exception itself.
    /// </summary>
    public static async Task HandleAsync(HttpContext context, RequestDelegate next, ILogger logger) =>
        throw new NotImplementedException(
            "TODO: Ex054 - record the failure in all three signals and answer with a safe problem document");
}
