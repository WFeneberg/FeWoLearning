using FeWoLearning.Architecture.Exercises.Desktop.Ex028;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex028_TelemetryBoundaryTests
{
    private sealed class RecordingTelemetry : ITelemetry
    {
        public List<(string EventName, IReadOnlyList<LogField> Fields)> Events { get; } = [];

        public void Record(string eventName, IReadOnlyList<LogField> fields) =>
            Events.Add((eventName, fields));
    }

    private static readonly Order Sample = new("O-1", 1234.56m, "Ada");

    private static (RecordingTelemetry Telemetry, OrderService Service) Build()
    {
        var telemetry = new RecordingTelemetry();
        return (telemetry, new OrderService(telemetry));
    }

    [Fact]
    public void Placing_An_Order_Records_One_Named_Event()
    {
        var (telemetry, service) = Build();

        service.Place(Sample);

        var recorded = Assert.Single(telemetry.Events);
        Assert.Equal("order.placed", recorded.EventName);
    }

    [Fact]
    public void Mechanism_Each_Value_Is_Its_Own_Addressable_Field()
    {
        // Not a rendered sentence. Record("order placed: O-1 for 1234.56") reads
        // beautifully in a console and is useless the moment anyone wants to sum the
        // amounts, alert above a threshold, or group by customer - all of which become
        // string parsing.
        var (telemetry, service) = Build();

        service.Place(Sample);

        var fields = telemetry.Events[0].Fields;

        Assert.Equal(["amount", "customer", "orderId"], fields.Select(f => f.Name).OrderBy(n => n));
        Assert.Equal("O-1", fields.Single(f => f.Name == "orderId").Value);
        Assert.Equal("Ada", fields.Single(f => f.Name == "customer").Value);
    }

    [Fact]
    public void Adversarial_The_Amount_Arrives_As_A_Decimal_Not_As_Text()
    {
        // Formatting in the domain bakes this machine's culture into the record, and a
        // sum over "1.234,56" and "1,234.56" is not a sum, it is an incident. Asserting
        // only that a field NAMED amount exists is satisfied by a formatted string.
        var (telemetry, service) = Build();

        service.Place(Sample);

        var amount = telemetry.Events[0].Fields.Single(f => f.Name == "amount");

        Assert.IsType<decimal>(amount.Value);
        Assert.Equal(1234.56m, amount.Value);
    }

    [Fact]
    public void Adversarial_There_Is_No_Extra_Pre_Rendered_Field()
    {
        // Emitting the structured fields AND a formatted "message" alongside them passes
        // every fact above while putting the domain right back in the formatting
        // business - and doubling the size of every record for a rendering only a human
        // will ever read.
        var (telemetry, service) = Build();

        service.Place(Sample);

        Assert.Equal(3, telemetry.Events[0].Fields.Count);
    }
}
