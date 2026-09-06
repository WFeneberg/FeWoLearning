using System.Net;
using System.Text.Json;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex057_BackendIngestionTests
{
    private static readonly DateTimeOffset At = new(2026, 9, 6, 10, 0, 0, TimeSpan.Zero);

    private const string Template = "Order {OrderId} shipped to {City}";

    private static JsonElement Parse(string line) => JsonDocument.Parse(line).RootElement.Clone();

    private static JsonElement Shipment(string orderId = "O-42", int amount = 19) =>
        Parse(Ex057_BackendIngestion.ToClefLine(
            At,
            Template,
            new Dictionary<string, object?> { ["OrderId"] = orderId, ["City"] = "Vienna", ["Amount"] = amount }));

    [Fact]
    public void The_line_carries_the_timestamp_and_the_template()
    {
        var line = Shipment();

        Assert.Equal(
            At,
            DateTimeOffset.Parse(line.GetProperty(Ex057_BackendIngestion.TimestampField).GetString()!));
        Assert.Equal(Template, line.GetProperty(Ex057_BackendIngestion.MessageTemplateField).GetString());
    }

    [Fact]
    public void Adversarial_A_The_template_is_not_rendered_and_the_fields_stand_alone()
    {
        // Row 001 said a message template keeps its fields queryable and left "queryable"
        // as an assertion. Here is the wire, and the reason: OrderId is a member of its
        // own, and the template still says {OrderId}.
        //
        // Interpolate at the call site and both halves vanish - the template becomes a
        // sentence and the field never exists.
        var line = Shipment();

        var template = line.GetProperty(Ex057_BackendIngestion.MessageTemplateField).GetString()!;
        Assert.Contains("{OrderId}", template);
        Assert.DoesNotContain("O-42", template);

        Assert.Equal("O-42", line.GetProperty("OrderId").GetString());
        Assert.Equal("Vienna", line.GetProperty("City").GetString());
    }

    [Fact]
    public void Adversarial_B_A_numeric_property_stays_a_number()
    {
        // Row 040's integer status code, on the wire this time. "19" as a string cannot be
        // compared, summed or charted, and a backend will not tell you why your threshold
        // never fires.
        var line = Shipment(amount: 19);

        var amount = line.GetProperty("Amount");
        Assert.Equal(JsonValueKind.Number, amount.ValueKind);
        Assert.Equal(19, amount.GetInt32());
    }

    [Fact]
    public void Adversarial_C_A_property_named_like_a_control_field_is_escaped()
    {
        // The format's own sharp edge. CLEF reserves the names beginning with @, so a
        // property genuinely called "@type" has to be written "@@type" - or it is read as
        // a control field and your data silently becomes metadata.
        var line = Parse(Ex057_BackendIngestion.ToClefLine(
            At, "just a message", new Dictionary<string, object?> { ["@type"] = "invoice" }));

        Assert.Equal("invoice", line.GetProperty("@@type").GetString());
        Assert.False(line.TryGetProperty("@type", out _));
    }

    [Fact]
    public void A_batch_is_newline_delimited_and_not_an_array()
    {
        var batch = Ex057_BackendIngestion.ToClefBatch([
            Ex057_BackendIngestion.ToClefLine(At, "first", new Dictionary<string, object?>()),
            Ex057_BackendIngestion.ToClefLine(At, "second", new Dictionary<string, object?>()),
        ]);

        Assert.DoesNotContain('[', batch);
        Assert.Equal(
            2,
            batch.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
    }

    [Fact]
    public async Task Container_A_real_backend_accepts_the_batch_and_answers_a_field_query()
    {
        // 🐳 Skipped unless the run passes -p:Containers=true.
        //
        // The fact that makes "queryable" stop being a word. Seq is asked
        // "OrderId = 'O-42'" and hands the event back - which is only possible because
        // OrderId arrived as a field. Interpolate at the call site and the same query
        // returns nothing: not an error, an empty result that looks exactly like "it did
        // not happen".
        await using var seq = await SeqContainer.StartAsync();

        var batch = Ex057_BackendIngestion.ToClefBatch([
            Ex057_BackendIngestion.ToClefLine(
                DateTimeOffset.UtcNow,
                Template,
                new Dictionary<string, object?>
                {
                    ["OrderId"] = "O-42", ["City"] = "Vienna", ["Amount"] = 19,
                }),
        ]);

        Assert.Equal(HttpStatusCode.Created, await seq.IngestAsync(batch));

        var found = await seq.QueryAsync("OrderId = 'O-42'");

        Assert.Contains("O-42", found);
        Assert.Contains("Vienna", found);

        // And a query for something that was never a field finds nothing, which is the
        // same result an interpolated message would give for OrderId.
        var missing = await seq.QueryAsync("OrderId = 'O-99'", timeoutSeconds: 3);
        Assert.DoesNotContain("O-99", missing);
    }
}
