using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex040_SemanticConventionsTests
{
    private static Activity Record(int statusCode)
    {
        var exported = new List<Activity>();

        using var provider = Ex040_SemanticConventions.Build(exported);
        var span = Ex040_SemanticConventions.RecordServerRequest("GET", "/orders/42", "api.example.com", statusCode);

        Assert.NotNull(span);
        return span;
    }

    [Fact]
    public void The_request_is_described_under_the_stable_names()
    {
        using var ctx = new TelemetryContext();

        var span = Record(200);

        Assert.Equal("GET", span.GetTagItem(Ex040_SemanticConventions.HttpRequestMethod)?.ToString());
        Assert.Equal("/orders/42", span.GetTagItem(Ex040_SemanticConventions.UrlPath)?.ToString());
        Assert.Equal("api.example.com", span.GetTagItem(Ex040_SemanticConventions.ServerAddress)?.ToString());
        Assert.Equal(ActivityKind.Server, span.Kind);
        Assert.Equal("GET /orders/42", span.DisplayName);
    }

    [Fact]
    public void Adversarial_A_The_status_code_is_an_integer_and_not_a_string()
    {
        // The detail that survives a rename and still breaks things. "404" as a string
        // sorts as text: a backend cannot ask for status_code >= 500, cannot bucket 4xx
        // against 5xx, and cannot chart an error rate. The convention specifies the type,
        // and getting it wrong produces a span that looks perfectly correct in a viewer.
        using var ctx = new TelemetryContext();

        var span = Record(200);

        var status = span.GetTagItem(Ex040_SemanticConventions.HttpResponseStatusCode);
        Assert.Equal(200, Assert.IsType<int>(status));
    }

    [Fact]
    public void Adversarial_B_None_of_the_superseded_v1_names_appears()
    {
        // For whoever learned this five years ago. The HTTP conventions were renamed
        // wholesale when they stabilised - http.method became http.request.method,
        // http.status_code became http.response.status_code, http.host became
        // server.address. Old spelling, same silence: the span arrives, carries your
        // data, and every dashboard built on the conventional names sees nothing.
        using var ctx = new TelemetryContext();

        var span = Record(200);

        foreach (var superseded in Ex040_SemanticConventions.SupersededNames)
        {
            Assert.Null(span.GetTagItem(superseded));
        }
    }

    [Fact]
    public void Adversarial_C_A_failure_sets_error_type_and_the_error_status()
    {
        using var ctx = new TelemetryContext();

        var span = Record(503);

        Assert.Equal(503, span.GetTagItem(Ex040_SemanticConventions.HttpResponseStatusCode));
        Assert.Equal("503", span.GetTagItem(Ex040_SemanticConventions.ErrorType)?.ToString());
        Assert.Equal(ActivityStatusCode.Error, span.Status);
    }

    [Fact]
    public void Adversarial_D_A_success_sets_neither()
    {
        // The paired half. Setting error.type unconditionally - or leaving the status
        // Error because it was easier - makes every request look like a failure, and the
        // error rate on every dashboard reads 100%.
        using var ctx = new TelemetryContext();

        var span = Record(200);

        Assert.Null(span.GetTagItem(Ex040_SemanticConventions.ErrorType));
        Assert.NotEqual(ActivityStatusCode.Error, span.Status);
    }
}
