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
    // Increment() call made against the registered one below.
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
}
