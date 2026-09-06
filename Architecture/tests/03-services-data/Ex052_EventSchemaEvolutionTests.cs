using FeWoLearning.Architecture.Exercises.ServicesData.Ex052;

namespace FeWoLearning.Architecture.Tests.ServicesData;

public class Ex052_EventSchemaEvolutionTests
{
    private const string V1 = """{"orderId":"O-1","amount":42.50}""";
    private const string V2 = """{"orderId":"O-1","amount":42.50,"currency":"CHF"}""";
    private const string V3 = """{"orderId":"O-1","amount":42.50,"currency":"CHF","channel":"mobile"}""";

    [Fact]
    public void A_Current_Payload_Reads_Straight_Through()
    {
        var read = Ex052_EventSchemaEvolution.Read(V2);

        Assert.Equal(new OrderPlaced("O-1", 42.50m, "CHF"), read);
    }

    [Fact]
    public void Mechanism_An_Older_Payload_Is_Upcast()
    {
        // The caller cannot tell v1 from v2, which is the point: the version lives in the
        // reader, and nothing downstream ever learns that v1 existed.
        var read = Ex052_EventSchemaEvolution.Read(V1);

        Assert.Equal("O-1", read.OrderId);
        Assert.Equal(Ex052_EventSchemaEvolution.DefaultCurrency, read.Currency);
    }

    [Fact]
    public void Mechanism_A_Payload_With_An_Unknown_Field_Is_Read_Rather_Than_Rejected()
    {
        // The tolerant reader, and the fact that decides whether a rolling deployment is
        // possible at all. During any rollout, instances on the NEW code publish events
        // carrying the new field while instances on the OLD code are still consuming - so
        // if the old consumers reject what they do not recognise, every deploy is either
        // an outage or a maintenance window.
        //
        // Strict deserialisation is the wrong mechanism here and it looks like rigour: it
        // reads v1 perfectly, reads v2 perfectly, and takes the system down the first
        // time somebody adds a field.
        var read = Ex052_EventSchemaEvolution.Read(V3);

        Assert.Equal(new OrderPlaced("O-1", 42.50m, "CHF"), read);
    }

    [Fact]
    public void An_Unknown_Field_On_An_Older_Payload_Is_Also_Tolerated()
    {
        // Both evolutions at once - a field added and a field still missing - which is
        // what a long-lived event store actually contains.
        var read = Ex052_EventSchemaEvolution.Read("""{"orderId":"O-1","amount":42.50,"channel":"mobile"}""");

        Assert.Equal(Ex052_EventSchemaEvolution.DefaultCurrency, read.Currency);
    }

    [Fact]
    public void Adversarial_A_Missing_Required_Field_Is_Still_Rejected_By_Name()
    {
        // Tolerant is not credulous. An event with no id cannot be correlated,
        // deduplicated or replayed, and accepting it just moves the failure somewhere
        // with less context. "Ignore everything you do not understand" would pass every
        // fact above.
        var failure = Assert.Throws<EventSchemaException>(
            () => Ex052_EventSchemaEvolution.Read("""{"amount":42.50}"""));

        Assert.Contains("orderId", failure.Message);
    }

    [Fact]
    public void A_Missing_Amount_Is_Rejected_Too()
    {
        Assert.Throws<EventSchemaException>(
            () => Ex052_EventSchemaEvolution.Read("""{"orderId":"O-1"}"""));
    }
}
