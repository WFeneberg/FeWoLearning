using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using OpenTelemetry;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex028_ResourceAttributesTests
{
    /// <summary>
    /// Sets OTEL_RESOURCE_ATTRIBUTES for the duration of one test and puts it back.
    /// The variable is process-wide, so this only holds because the suite is serial.
    /// </summary>
    private sealed class EnvironmentOverride : IDisposable
    {
        private readonly string? _previous;

        public EnvironmentOverride(string? value)
        {
            _previous = Environment.GetEnvironmentVariable(
                Ex028_ResourceAttributes.ResourceAttributesVariable);

            Environment.SetEnvironmentVariable(
                Ex028_ResourceAttributes.ResourceAttributesVariable, value);
        }

        public void Dispose() =>
            Environment.SetEnvironmentVariable(
                Ex028_ResourceAttributes.ResourceAttributesVariable, _previous);
    }

    private static string? Attribute(
        IEnumerable<KeyValuePair<string, object>> attributes, string key) =>
        attributes.FirstOrDefault(a => a.Key == key).Value?.ToString();

    [Fact]
    public void The_resource_identifies_the_service_its_version_and_this_instance()
    {
        using var env = new EnvironmentOverride(null);

        var resource = Ex028_ResourceAttributes.BuildResource("pod-7");

        Assert.Equal(Ex028_ResourceAttributes.ServiceName,
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceNameKey));
        Assert.Equal(Ex028_ResourceAttributes.ServiceVersion,
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceVersionKey));
        Assert.Equal("pod-7",
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceInstanceIdKey));
    }

    [Fact]
    public void Adversarial_A_The_service_name_is_not_the_SDKs_fallback()
    {
        // Omit service.name and the SDK substitutes "unknown_service:<processname>".
        // That is not an error, not a warning, and exactly what you find in the
        // dropdown three weeks later when you go looking for your service.
        using var env = new EnvironmentOverride(null);

        var resource = Ex028_ResourceAttributes.BuildResource("pod-7");

        var name = Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceNameKey);
        Assert.NotNull(name);
        Assert.DoesNotContain("unknown_service", name);
    }

    [Fact]
    public void Adversarial_B_The_environment_can_add_attributes_the_code_never_mentions()
    {
        // What keeps deployment concerns out of the source. The operator sets the
        // variable on the container; nothing is rebuilt, and nothing in the code has to
        // know that staging exists.
        using var env = new EnvironmentOverride("deployment.environment=staging,team=platform");

        var resource = Ex028_ResourceAttributes.BuildResource("pod-7");

        Assert.Equal("staging", Attribute(resource.Attributes, "deployment.environment"));
        Assert.Equal("platform", Attribute(resource.Attributes, "team"));
    }

    [Fact]
    public void Adversarial_C_The_environment_does_not_displace_what_the_code_set()
    {
        // The paired half. A detector that REPLACES rather than merges would satisfy
        // Adversarial_B perfectly and lose the service's own identity the moment an
        // operator set anything at all.
        using var env = new EnvironmentOverride("deployment.environment=staging");

        var resource = Ex028_ResourceAttributes.BuildResource("pod-7");

        Assert.Equal("staging", Attribute(resource.Attributes, "deployment.environment"));
        Assert.Equal(Ex028_ResourceAttributes.ServiceName,
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceNameKey));
        Assert.Equal("pod-7",
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceInstanceIdKey));
    }

    [Fact]
    public void The_built_provider_carries_that_same_resource()
    {
        // A resource the code assembles and then forgets to attach is a resource that
        // reaches no backend. GetResource is what proves the wiring, and it is public
        // precisely so this can be checked.
        using var ctx = new TelemetryContext();
        using var env = new EnvironmentOverride(null);
        var exported = new List<Activity>();

        using var provider = Ex028_ResourceAttributes.Build(exported, "pod-7");

        var resource = provider.GetResource();
        Assert.Equal(Ex028_ResourceAttributes.ServiceName,
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceNameKey));
        Assert.Equal("pod-7",
            Attribute(resource.Attributes, Ex028_ResourceAttributes.ServiceInstanceIdKey));
    }
}
