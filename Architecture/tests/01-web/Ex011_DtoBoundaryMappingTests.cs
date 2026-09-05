using FeWoLearning.Architecture.Exercises.Web;
using FeWoLearning.Architecture.Exercises.Web.Ex011.Domain;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex011_DtoBoundaryMappingTests
{
    private static Customer Ada()
    {
        var customer = new Customer("C-1", "Ada", "Lovelace", 5000m);
        customer.Annotate("risk", "flagged for review");
        return customer;
    }

    [Fact]
    public void Use_The_Projection_Carries_The_Public_Fields()
    {
        var dto = Ex011_DtoBoundaryMapping.ToDto(Ada());

        Assert.Equal("C-1", dto.Id);
        Assert.Equal(5000m, dto.CreditLimit);
    }

    [Fact]
    public void The_Projection_Does_Real_Work_Rather_Than_Copying_One_Field()
    {
        // DisplayName exists in neither the domain nor as a single field, so a mapper
        // that copies properties across by name cannot produce it.
        var dto = Ex011_DtoBoundaryMapping.ToDto(Ada());

        Assert.Equal("Ada Lovelace", dto.DisplayName);
    }

    [Fact]
    public void Fitness_A_Clean_Contract_Is_Not_Reported()
    {
        // Paired with the two below - alone, an empty list satisfies it.
        var leaks = Ex011_DtoBoundaryMapping.FindDomainLeaks();

        Assert.DoesNotContain("CustomerDto", leaks);
    }

    [Fact]
    public void Fitness_A_Contract_Exposing_An_Aggregate_Is_Reported()
    {
        var leaks = Ex011_DtoBoundaryMapping.FindDomainLeaks();

        Assert.Contains("LeakyCustomerDto", leaks);
    }

    [Fact]
    public void Fitness_A_Contract_Leaking_Through_A_Generic_Argument_Is_Reported()
    {
        // The plausible-wrong catch. IReadOnlyList<Note> is not itself a domain type,
        // and a direct namespace comparison says so quite correctly - while the
        // contract has nevertheless published Note to every consumer, so renaming
        // Note.Author is now a breaking change nobody will see coming.
        var leaks = Ex011_DtoBoundaryMapping.FindDomainLeaks();

        Assert.Contains("LeakyNotesDto", leaks);
    }
}
