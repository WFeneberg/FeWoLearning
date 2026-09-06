using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex036_OtelLogsPipelineTests
{
    private static List<LogRecord> Ship(
        bool includeFormattedMessage = false,
        params (string OrderId, string City)[] shipments)
    {
        var exported = new List<LogRecord>();

        using (var factory = Ex036_OtelLogsPipeline.Build(exported, includeFormattedMessage))
        {
            var logger = factory.CreateLogger(Ex036_OtelLogsPipeline.CategoryName);
            foreach (var (orderId, city) in shipments) Ex036_OtelLogsPipeline.LogShipped(logger, orderId, city);
        }

        return exported;
    }

    [Fact]
    public void The_record_carries_the_fields_and_the_template_as_attributes()
    {
        // No opt-in of any kind: the structured state and the {OriginalFormat} entry are
        // simply there. ParseStateValues is not needed for a normal message template.
        var record = Assert.Single(Ship(shipments: ("O-42", "Vienna")));

        Assert.Equal("O-42", LogRecordReadout.Attribute(record, "OrderId"));
        Assert.Equal("Vienna", LogRecordReadout.Attribute(record, "City"));
        Assert.Equal(
            Ex036_OtelLogsPipeline.Template,
            LogRecordReadout.Attribute(record, "{OriginalFormat}"));
    }

    [Fact]
    public void Adversarial_A_Body_is_the_template_and_not_the_rendered_sentence()
    {
        // The surprise, and it is good news. Everyone reads "Body" as "the message", and
        // an OTLP viewer does show it as the message - but what the SDK puts there is the
        // constant template. So grouping, alerting and searching by event work on Body
        // directly, while the part that varies lives in Attributes where it is queryable.
        var record = Assert.Single(Ship(shipments: ("O-42", "Vienna")));

        Assert.Equal(Ex036_OtelLogsPipeline.Template, record.Body);
        Assert.DoesNotContain("Vienna", record.Body ?? string.Empty);
    }

    [Fact]
    public void Adversarial_B_Two_calls_with_different_data_share_one_body()
    {
        // The consequence of the above, stated as the thing a backend needs. Interpolate
        // at the call site and every shipment becomes its own event type; use the
        // template and there is one event with two instances.
        var records = Ship(shipments: [("O-42", "Vienna"), ("O-43", "Graz")]);

        Assert.Equal(2, records.Count);
        Assert.Equal(records[0].Body, records[1].Body);
        Assert.NotEqual(
            LogRecordReadout.Attribute(records[0], "OrderId"),
            LogRecordReadout.Attribute(records[1], "OrderId"));
    }

    [Fact]
    public void Adversarial_C_The_rendered_sentence_is_absent_unless_the_pipeline_asked_for_it()
    {
        // The cost. Rendering is an allocation per record that nothing downstream needs -
        // a backend can render it from Body and Attributes whenever a human looks - so
        // the SDK does not do it unless told. Turning it on "to be safe" is paying for a
        // string nobody reads.
        var without = Assert.Single(Ship(shipments: ("O-42", "Vienna")));
        var with = Assert.Single(Ship(includeFormattedMessage: true, shipments: ("O-42", "Vienna")));

        Assert.Null(without.FormattedMessage);
        Assert.Equal("Order O-42 shipped to Vienna", with.FormattedMessage);

        // And the template survives either way.
        Assert.Equal(Ex036_OtelLogsPipeline.Template, with.Body);
    }

    [Fact]
    public void The_record_carries_its_category_level_and_event()
    {
        var record = Assert.Single(Ship(shipments: ("O-42", "Vienna")));

        Assert.Equal(Ex036_OtelLogsPipeline.CategoryName, record.CategoryName);
        Assert.Equal(LogLevel.Information, record.LogLevel);
        Assert.Equal(Ex036_OtelLogsPipeline.Shipped.Id, record.EventId.Id);
        Assert.Equal(Ex036_OtelLogsPipeline.Shipped.Name, record.EventId.Name);
    }
}
