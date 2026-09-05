using FeWoLearning.Architecture.Exercises.Web;
using FeWoLearning.Architecture.Exercises.Web.Ex001.Domain;
using FeWoLearning.Architecture.Exercises.Web.Ex001.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex001_LayeredCompositionRootTests
{
    [Fact]
    public void Use_The_Composition_Root_Wires_A_Working_Application()
    {
        using var provider = Ex001_LayeredCompositionRoot.Build();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<InvoiceService>();
        service.Issue("INV-1", 42.50m);

        Assert.Equal(42.50m, service.AmountOf("INV-1"));
    }

    [Fact]
    public void Mechanism_The_Port_Is_Bound_To_The_Infrastructure_Adapter()
    {
        // Asserting only that the round-trip above works would be satisfied by an
        // InvoiceService that new'd up its own dictionary and never used a port at
        // all. This asserts the binding itself.
        using var provider = Ex001_LayeredCompositionRoot.Build();

        var store = provider.GetRequiredService<IInvoiceStore>();

        Assert.IsType<InMemoryInvoiceStore>(store);
    }

    [Fact]
    public void Mechanism_The_Store_Is_A_Singleton_So_State_Crosses_Scopes()
    {
        // A transient store passes the round-trip fact above and still loses every
        // invoice the moment a second scope opens. Lifetime is part of composition.
        using var provider = Ex001_LayeredCompositionRoot.Build();

        using (var writing = provider.CreateScope())
            writing.ServiceProvider.GetRequiredService<InvoiceService>().Issue("INV-2", 10m);

        using var reading = provider.CreateScope();
        var amount = reading.ServiceProvider.GetRequiredService<InvoiceService>().AmountOf("INV-2");

        Assert.Equal(10m, amount);
    }

    [Fact]
    public void Fitness_Clean_Domain_Types_Are_Not_Reported()
    {
        // Paired with the two facts below. On its own it is satisfied by a checker
        // that returns an empty list, which is exactly why it is never alone.
        var violations = Ex001_LayeredCompositionRoot.FindDependencyDirectionViolations();

        Assert.DoesNotContain(nameof(InvoiceService), violations);
        Assert.DoesNotContain(nameof(Invoice), violations);
    }

    [Fact]
    public void Fitness_A_Domain_Type_Taking_An_Infrastructure_Constructor_Parameter_Is_Reported()
    {
        var violations = Ex001_LayeredCompositionRoot.FindDependencyDirectionViolations();

        Assert.Contains(nameof(LeakyByConstructor), violations);
    }

    [Fact]
    public void Fitness_A_Domain_Type_Holding_An_Infrastructure_Field_Is_Reported()
    {
        // The plausible-wrong catch: a checker that inspects only constructor
        // parameters is an earnest, working implementation that passes every fact
        // above and fails this one.
        var violations = Ex001_LayeredCompositionRoot.FindDependencyDirectionViolations();

        Assert.Contains(nameof(LeakyByField), violations);
    }
}
