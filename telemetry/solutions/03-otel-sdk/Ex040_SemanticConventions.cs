using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 040 — SemanticConventions (otel-sdk).
// Goal:   Use the names everything else already agrees on, and get their TYPES right
//         too.
// Drills: the stable HTTP server attributes, error.type, span status, the v1.x names
//         that no longer work.
// Passes: a served request carries http.request.method, url.path, server.address and
//                     http.response.status_code;
//         the status code is an INTEGER, not a string;
//         none of the superseded v1.x names (http.method, http.status_code, http.host)
//                     appears;
//         and a failing request additionally sets error.type and an Error span status,
//                     while a successful one sets neither.
//
// This row is not about learning a list. It is about what happens when you do not use it:
// nothing. No error, no warning, no failing test - your spans arrive, they carry your
// data, and every dashboard, alert and service map in the product is built on the
// conventional names, so none of them sees a thing. The data is present and unfindable,
// which is the worst of both.
//
// The second clause is the detail that survives a rename and still breaks things.
// "404" as a string sorts as text: a backend cannot ask for status_code >= 500, cannot
// bucket 4xx against 5xx, and cannot chart an error rate. The convention specifies the
// type, and getting it wrong produces a span that looks perfectly correct in a viewer.
//
// The third clause is the one that catches people who learned this five years ago. The
// HTTP conventions were renamed wholesale when they stabilised: http.method became
// http.request.method, http.status_code became http.response.status_code, http.host
// became server.address. Old spelling, same silence.
public static class Ex040_SemanticConventions
{
    /// <summary>The stable names. Spelling is the whole point.</summary>
    public const string HttpRequestMethod = "http.request.method";

    /// <inheritdoc cref="HttpRequestMethod"/>
    public const string HttpResponseStatusCode = "http.response.status_code";

    /// <inheritdoc cref="HttpRequestMethod"/>
    public const string ServerAddress = "server.address";

    /// <inheritdoc cref="HttpRequestMethod"/>
    public const string UrlPath = "url.path";

    /// <inheritdoc cref="HttpRequestMethod"/>
    public const string ErrorType = "error.type";

    /// <summary>The v1.x spellings, kept here only so the tests can prove their absence.</summary>
    public static readonly string[] SupersededNames = ["http.method", "http.status_code", "http.host"];

    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex040";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Record one served request as a <see cref="ActivityKind.Server"/> span named
    /// "<c>{method} {path}</c>", carrying the four conventional attributes.
    ///
    /// When <paramref name="statusCode"/> is 500 or above, also set
    /// <see cref="ErrorType"/> to the status code rendered as a string - which is what
    /// the convention asks for when there is no more specific error - and set the span
    /// status to <see cref="ActivityStatusCode.Error"/>.
    /// </summary>
    public static Activity? RecordServerRequest(string method, string path, string host, int statusCode)
    {
        using var span = Source.StartActivity($"{method} {path}", ActivityKind.Server);

        span?.SetTag(HttpRequestMethod, method);
        span?.SetTag(UrlPath, path);
        span?.SetTag(ServerAddress, host);

        // An INT, not a string. "404" as text cannot be compared, bucketed or charted,
        // and the span looks perfectly correct in a viewer either way.
        span?.SetTag(HttpResponseStatusCode, statusCode);

        if (statusCode >= 500)
        {
            span?.SetTag(ErrorType, statusCode.ToString());
            span?.SetStatus(ActivityStatusCode.Error);
        }

        return span;
    }
}
