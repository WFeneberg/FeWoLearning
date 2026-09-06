using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex042_OtlpExporterConfigurationTests
{
    /// <summary>
    /// Sets the OTEL_EXPORTER_OTLP_* variables for one test and puts them back. They are
    /// process-wide, so this only holds because the suite is serial.
    /// </summary>
    private sealed class EnvironmentOverride : IDisposable
    {
        private readonly (string Name, string? Previous)[] _saved;

        public EnvironmentOverride(params (string Name, string? Value)[] values)
        {
            _saved = values
                .Select(v => (v.Name, Environment.GetEnvironmentVariable(v.Name)))
                .ToArray();

            foreach (var (name, value) in values) Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            foreach (var (name, previous) in _saved) Environment.SetEnvironmentVariable(name, previous);
        }
    }

    [Fact]
    public void With_nothing_set_the_exporter_uses_its_documented_default()
    {
        using var env = new EnvironmentOverride(
            (Ex042_OtlpExporterConfiguration.EndpointVariable, null),
            (Ex042_OtlpExporterConfiguration.ProtocolVariable, null));

        var options = Ex042_OtlpExporterConfiguration.FromEnvironment();

        Assert.Equal(Ex042_OtlpExporterConfiguration.DefaultEndpoint, options.Endpoint.ToString());
    }

    [Fact]
    public void Adversarial_A_The_environment_moves_the_endpoint_with_no_code_change()
    {
        // The environment variables are not a convenience, they are the contract. The
        // same binary ships to a laptop with nothing listening, to a cluster with a
        // sidecar collector, and to a vendor's endpoint behind an API key - and the only
        // thing that differs is the environment it was started in. Hard-code the endpoint
        // and you have turned a deployment decision into a build decision.
        using var env = new EnvironmentOverride(
            (Ex042_OtlpExporterConfiguration.EndpointVariable, "http://collector.internal:4318"),
            (Ex042_OtlpExporterConfiguration.ProtocolVariable, "http/protobuf"));

        var options = Ex042_OtlpExporterConfiguration.FromEnvironment();

        Assert.Equal("http://collector.internal:4318/", options.Endpoint.ToString());
        Assert.Equal(OtlpExportProtocol.HttpProtobuf, options.Protocol);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void An_unset_headers_variable_parses_to_nothing(string? value)
    {
        Assert.Empty(Ex042_OtlpExporterConfiguration.ParseHeaders(value));
    }

    [Fact]
    public void Adversarial_B_The_headers_variable_is_a_LIST_and_arrives_parsed()
    {
        // Where the API key goes, which is why parsing it matters: it is a
        // comma-separated list of key=value pairs, and an exporter that treats it as one
        // opaque string sends a header nobody accepts and gets a 401 with no explanation.
        var headers = Ex042_OtlpExporterConfiguration.ParseHeaders("api-key=secret,x-tenant=acme");

        Assert.Equal(2, headers.Count);
        Assert.Equal("secret", headers["api-key"]);
        Assert.Equal("acme", headers["x-tenant"]);
    }

    [Fact]
    public async Task Container_A_real_collector_receives_the_span()
    {
        // 🐳 Skipped unless the run passes -p:Containers=true. Everything above is graded
        // without Docker; this proves the wire, it does not carry the row.
        ContainerGate.SkipUnlessEnabled();

        using var ctx = new TelemetryContext();
        await using var collector = await CollectorContainer.StartAsync();

        using (var provider = Ex042_OtlpExporterConfiguration.BuildOtlp(
            collector.TracesEndpoint, "ex042-service"))
        {
            Ex042_OtlpExporterConfiguration.DoWork("shipped-over-the-wire");
            provider.ForceFlush();
        }

        var logs = await collector.WaitForLogContaining("shipped-over-the-wire");

        Assert.Contains("shipped-over-the-wire", logs);
        Assert.Contains("ex042-service", logs);
    }
}
