using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex048_FactoryInjectedComponentTests : BunitContext
{
    [Fact]
    public void Value_Reflects_The_Resolved_Store()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex048_FactoryInjectedComponent>();

        Assert.Equal("0", cut.Find("#value").TextContent);
    }

    // Registered scoped (not singleton) - both lifetimes hand back the very same
    // instance to every resolution within this test's single DI scope either way,
    // so this fact cannot distinguish "resolve fresh every call" from "resolve once
    // and keep the reference" (CounterStore is a mutable reference type: whichever
    // reference you hold, its live .Value is what you see). What this fact *does*
    // reject is an implementation that never asks the container at all - e.g.
    // `new CounterStore()` - since a fabricated instance never observes the
    // Increment() call made against the registered one below. But it can only catch
    // that fabrication *when a real registration happens to exist to compare
    // against*; the stronger, container-agnostic version of the same claim -
    // resolution always goes through Services, full stop - is what
    // Rendering_Without_A_Registered_Store_Throws below actually proves.
    [Fact]
    public void Advancing_The_Store_And_Rerendering_Reflects_The_New_Value()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex048_FactoryInjectedComponent>();
        var store = Services.GetRequiredService<CounterStore>();
        store.Increment();
        cut.Render();

        Assert.Equal("1", cut.Find("#value").TextContent);
    }

    // This is the fact that actually carries the "resolve from the container, not a
    // fabricated instance" weight the two facts above cannot: `new CounterStore()`
    // never asks Services for anything, so it would render successfully with nothing
    // registered - only a real Services.GetRequiredService<CounterStore>() call
    // throws when the container has no registration for it. Verified directly:
    // Bunit's render pipeline surfaces InvalidOperationException("No service for
    // type 'FeWoLearning.Blazor.Support.CounterStore' has been registered.") from
    // exactly this call site when CounterStore is left unregistered.
    [Fact]
    public void Rendering_Without_A_Registered_Store_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Render<Ex048_FactoryInjectedComponent>());
    }
}
