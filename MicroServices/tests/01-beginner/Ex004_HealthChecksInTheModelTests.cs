using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex004_HealthChecksInTheModelTests
{
    private static string KeyOf(ModelHarness.Result model, string resource)
    {
        // Exactly one: a resource can carry several HealthCheckAnnotations, and
        // "at least one" would let a stray extra check pass unnoticed.
        var annotations = model.Resource(resource).Annotations.OfType<HealthCheckAnnotation>().ToList();
        return Assert.Single(annotations).Key;
    }

    [Fact]
    public void Each_container_carries_exactly_one_health_check_for_its_own_path()
    {
        var model = ModelHarness.Build(Ex004_HealthChecksInTheModel.Configure);

        // The annotation is the graded artifact, never a 200 response: nothing is
        // running. WithHttpHealthCheck bakes the resource, endpoint, path and
        // expected status code into the key, so the key is where the path shows up.
        Assert.Equal("api_http_/healthz_200_check", KeyOf(model, "api"));
        Assert.Equal("admin_http_/ready_200_check", KeyOf(model, "admin"));
    }

    [Fact]
    public void The_key_varies_with_the_path_that_was_asked_for()
    {
        var model = ModelHarness.Build(Ex004_HealthChecksInTheModel.Configure);

        var api = KeyOf(model, "api");
        var admin = KeyOf(model, "admin");

        // Calling WithHttpHealthCheck() with no path at all still produces an
        // annotation - key "api_http_/_200_check" - so mere presence grades nothing.
        // These assertions reject the no-path call and the copy-paste that gives
        // both containers the same path.
        Assert.Contains("/healthz", api, StringComparison.Ordinal);
        Assert.DoesNotContain("/ready", api, StringComparison.Ordinal);
        Assert.Contains("/ready", admin, StringComparison.Ordinal);
        Assert.DoesNotContain("/healthz", admin, StringComparison.Ordinal);
    }
}
