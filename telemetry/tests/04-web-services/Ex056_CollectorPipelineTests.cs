using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex056_CollectorPipelineTests
{
    private const string Email = "ada@example.com";

    [Fact]
    public void The_application_records_the_attribute_it_should_not_be_storing()
    {
        // The situation, not the mistake to fix here. Everything the row is about happens
        // after this point.
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex056_CollectorPipeline.SourceName);

        Ex056_CollectorPipeline.DoCheckout(Email, "O-42");

        var span = probe.Single();
        Assert.Equal(Ex056_CollectorPipeline.SpanName, span.DisplayName);
        Assert.Equal(Email, span.GetTagItem(Ex056_CollectorPipeline.SensitiveAttribute)?.ToString());
        Assert.Equal("O-42", span.GetTagItem(Ex056_CollectorPipeline.OrderAttribute)?.ToString());
    }

    [Fact]
    public void Adversarial_A_The_configuration_deletes_the_attribute_in_the_traces_pipeline()
    {
        // Declaring the processor is not the same as using it: a collector happily starts
        // with a processor defined and never referenced, and drops nothing. The pipeline
        // list is what makes it run.
        var config = Ex056_CollectorPipeline.CollectorConfig();

        Assert.Contains("attributes", config);
        Assert.Contains(Ex056_CollectorPipeline.SensitiveAttribute, config);
        Assert.Contains("delete", config);
        Assert.Contains("processors: [attributes]", config);
    }

    [Fact]
    public void Adversarial_B_The_configuration_removes_a_field_rather_than_the_data()
    {
        // A collector rule that drops the whole span would satisfy "the email is gone"
        // perfectly and lose every checkout in the system. The order attribute is never
        // named, so nothing in the config can be removing it.
        var config = Ex056_CollectorPipeline.CollectorConfig();

        Assert.DoesNotContain(Ex056_CollectorPipeline.OrderAttribute, config);
        Assert.DoesNotContain("filter", config);
    }

    [Fact]
    public async Task Container_A_real_collector_strips_the_attribute_and_keeps_the_span()
    {
        // 🐳 Skipped unless the run passes -p:Containers=true.
        //
        // The only fact here that proves the configuration is real: everything above
        // grades an artifact, and a YAML document is a claim until something parses it.
        ContainerGate.SkipUnlessEnabled();

        using var ctx = new TelemetryContext();
        await using var collector = await CollectorContainer.StartAsync(
            Ex056_CollectorPipeline.CollectorConfig());

        using (var provider = Ex056_CollectorPipeline.BuildOtlp(
            collector.TracesEndpoint, "ex056-service"))
        {
            Ex056_CollectorPipeline.DoCheckout(Email, "O-42");
            provider.ForceFlush();
        }

        var logs = await collector.WaitForLogContaining(Ex056_CollectorPipeline.SpanName);

        // The span arrived, the order survived, and the email is gone - removed by a rule
        // this service knows nothing about, which would apply just as well to forty others.
        Assert.Contains(Ex056_CollectorPipeline.SpanName, logs);
        Assert.Contains("O-42", logs);
        Assert.DoesNotContain(Email, logs);
    }
}
