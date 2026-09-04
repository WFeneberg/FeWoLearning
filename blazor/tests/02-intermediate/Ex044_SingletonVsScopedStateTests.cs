using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex044_SingletonVsScopedStateTests : BunitContext
{
    [Fact]
    public void Combined_Reflects_Both_Stores_Initial_Values()
    {
        Services.AddScoped<ScopedCounter>();
        Services.AddSingleton<SingletonCounter>();

        var cut = Render<Ex044_SingletonVsScopedState>();

        Assert.Equal("0", cut.Find("#combined").TextContent);
    }

    // Also proves the two lifetimes resolve to two distinct instances rather than
    // one shared counter under two names: advancing the scoped one must not move
    // the singleton's own reading.
    [Fact]
    public void Advancing_The_Scoped_Store_Changes_The_Total_By_One_And_Leaves_The_Singleton_Alone()
    {
        Services.AddScoped<ScopedCounter>();
        Services.AddSingleton<SingletonCounter>();

        var cut = Render<Ex044_SingletonVsScopedState>();
        var scoped = Services.GetRequiredService<ScopedCounter>();

        scoped.Increment();
        cut.Render();

        Assert.Equal("1", cut.Find("#combined").TextContent);
        Assert.Equal("0", cut.Find("#singleton").TextContent);
    }

    [Fact]
    public void Advancing_The_Singleton_Store_Changes_The_Total_By_One_And_Leaves_The_Scoped_Alone()
    {
        Services.AddScoped<ScopedCounter>();
        Services.AddSingleton<SingletonCounter>();

        var cut = Render<Ex044_SingletonVsScopedState>();
        var singleton = Services.GetRequiredService<SingletonCounter>();

        singleton.Increment();
        cut.Render();

        Assert.Equal("1", cut.Find("#combined").TextContent);
        Assert.Equal("0", cut.Find("#scoped").TextContent);
    }
}
