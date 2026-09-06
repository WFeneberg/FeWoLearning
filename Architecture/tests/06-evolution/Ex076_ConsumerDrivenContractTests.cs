using FeWoLearning.Architecture.Exercises.Evolution.Ex076;

namespace FeWoLearning.Architecture.Tests.Evolution;

public class Ex076_ConsumerDrivenContractTests
{
    private static readonly ConsumerContract Billing = new("billing",
    [
        new FieldExpectation("id", "string", Required: true),
        new FieldExpectation("total", "number", Required: true),
    ]);

    private static readonly ConsumerContract Shipping = new("shipping",
    [
        new FieldExpectation("id", "string", Required: true),
        new FieldExpectation("address", "string", Required: true),
        new FieldExpectation("giftMessage", "string", Required: false),
    ]);

    private static readonly ConsumerContract[] All = [Billing, Shipping];

    [Fact]
    public void A_Response_Meeting_Every_Contract_Is_Clean()
    {
        var response = """{"id":"o-1","total":42.5,"address":"1 Main St"}""";

        Assert.Empty(Ex076_ConsumerDrivenContract.Verify(response, All));
    }

    [Fact]
    public void A_Removed_Required_Field_Is_A_Violation_Naming_Who_Needed_It()
    {
        // Naming the consumer is what makes the failure actionable: the provider's
        // pipeline is red, and the message says who to talk to.
        var response = """{"id":"o-1","total":42.5}""";

        var violation = Assert.Single(Ex076_ConsumerDrivenContract.Verify(response, All));

        Assert.Equal("shipping", violation.Consumer);
        Assert.Equal("address", violation.Path);
    }

    [Fact]
    public void A_Retyped_Field_Is_A_Violation_Even_Though_It_Is_Still_There()
    {
        // "45" and 45 are not the same value to a parser, and the consumer's is the one
        // that will throw. A check that only asks whether the field exists calls this
        // change safe.
        var response = """{"id":"o-1","total":"42.5","address":"1 Main St"}""";

        var violation = Assert.Single(Ex076_ConsumerDrivenContract.Verify(response, All));

        Assert.Equal("billing", violation.Consumer);
        Assert.Equal("total", violation.Path);
        Assert.Contains("number", violation.Reason);
    }

    [Fact]
    public void A_Missing_Optional_Field_Is_Not_A_Violation()
    {
        var response = """{"id":"o-1","total":42.5,"address":"1 Main St"}""";

        Assert.DoesNotContain(Ex076_ConsumerDrivenContract.Verify(response, All), v => v.Path == "giftMessage");
    }

    [Fact]
    public void Mechanism_A_Field_Nobody_Asked_For_Is_Not_A_Violation()
    {
        // The clause that decides whether anybody keeps running this. A check demanding an
        // exact match fails on every new field, including the ones nobody reads - so the
        // provider learns the suite cries wolf, and by the time it fails for a real reason
        // it has been ignored or deleted. A contract is what consumers DEPEND on, not what
        // the provider happens to return.
        var response = """{"id":"o-1","total":42.5,"address":"1 Main St","loyaltyTier":"gold","createdAt":"2026-01-01"}""";

        Assert.Empty(Ex076_ConsumerDrivenContract.Verify(response, All));
    }

    [Fact]
    public void Every_Affected_Consumer_Is_Reported_Separately()
    {
        // One change can break several consumers, and the provider needs the whole list -
        // fixing one and redeploying to discover the next is how a morning disappears.
        var response = """{"total":42.5}""";

        var violations = Ex076_ConsumerDrivenContract.Verify(response, All);

        Assert.Contains(violations, v => v is { Consumer: "billing", Path: "id" });
        Assert.Contains(violations, v => v is { Consumer: "shipping", Path: "id" });
        Assert.Contains(violations, v => v is { Consumer: "shipping", Path: "address" });
        Assert.Equal(3, violations.Count);
    }

    [Fact]
    public void No_Contracts_Means_Nothing_To_Break()
    {
        Assert.Empty(Ex076_ConsumerDrivenContract.Verify("""{"anything":1}""", []));
    }
}
