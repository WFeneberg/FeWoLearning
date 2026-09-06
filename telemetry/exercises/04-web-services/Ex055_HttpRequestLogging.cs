using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 055 — HttpRequestLogging (web-services).
// Goal:   Write the access log yourself, and find the two places everyone gets it wrong.
// Drills: where to measure a duration, what a query string is allowed to contain.
// Passes: one record per request, carrying the method, the path, the status and a
//                     duration;
//         the duration covers the WHOLE pipeline including everything downstream, not
//                     just the middleware's own frame;
//         a failing request is still logged, with its status, rather than vanishing with
//                     the exception;
//         the recorded path carries NO query string;
//         and a named query parameter judged safe is recorded on its own, so the log is
//                     still useful.
//
// The second clause is the one that produces a graph everybody trusts and nobody should.
// Start the stopwatch after `await next(context)` and you have measured the time to write
// a response header; start it before and stop it after, and you have measured the request.
// The difference is invisible in the code and roughly the whole latency of your service.
//
// The third is the same shape as row 049: a middleware that logs only on the happy path
// loses exactly the requests worth having a log for. Put it in a finally.
//
// The fourth and fifth are a pair, and the pair is the point. A query string is
// caller-controlled and routinely carries a search term, an email address, a password
// somebody put in a URL by mistake, or a token from a badly built redirect. Logging it
// whole copies all of that into a system with a different access model and a longer
// retention - the same argument row 051 makes about parameter values.
//
// And logging NOTHING from it makes the access log useless for the thing it is for, since
// "GET /search" with no terms answers no question anybody has. So: an allowlist. Name the
// parameters you meant, record those, and drop the rest unread.
public static class Ex055_HttpRequestLogging
{
    /// <summary>The category the access log is written under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex055";

    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex055";

    /// <summary>The one query parameter this service considers safe to record.</summary>
    public const string SafeQueryParameter = "page";

    /// <summary>The field carrying the request method.</summary>
    public const string MethodField = "Method";

    /// <summary>The field carrying the path, without its query.</summary>
    public const string PathField = "Path";

    /// <summary>The field carrying the response status.</summary>
    public const string StatusField = "StatusCode";

    /// <summary>The field carrying how long the whole pipeline took.</summary>
    public const string DurationField = "DurationMs";

    /// <summary>The field carrying the one safe query parameter, when present.</summary>
    public const string PageField = "Page";

    /// <summary>The constant template every access-log record uses.</summary>
    public const string Template =
        "{Method} {Path} responded {StatusCode} in {DurationMs}ms";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// The access-log middleware.
    ///
    /// Write exactly one Information record per request using <see cref="Template"/>,
    /// with <see cref="MethodField"/>, <see cref="PathField"/>,
    /// <see cref="StatusField"/> and <see cref="DurationField"/> - plus
    /// <see cref="PageField"/> when the request carries a
    /// <see cref="SafeQueryParameter"/> query parameter.
    ///
    /// The duration covers the whole of <paramref name="next"/>. The record is written
    /// whether or not <paramref name="next"/> throws, and an exception is then allowed to
    /// continue on its way.
    ///
    /// The path is <see cref="HttpRequest.Path"/> and nothing else - no query string,
    /// under any field.
    /// </summary>
    public static async Task LogRequestAsync(HttpContext context, RequestDelegate next, ILogger logger) =>
        throw new NotImplementedException(
            "TODO: Ex055 - log one record per request, timing the whole pipeline and keeping the query out");
}
