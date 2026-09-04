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

    // This is the actual lifetime lesson the catalog row promises - Combined itself
    // cannot show it (it would read identically under AddScoped, AddSingleton, or
    // AddTransient for both). A second DI scope is what makes AddScoped vs
    // AddSingleton observable: the scoped registration hands back a different
    // instance per scope, the singleton registration does not. Also renders the
    // component (rather than only probing the container) so this fact still exercises
    // Combined - and so still fails red on the unimplemented stub - like every other
    // fact in this class.
    [Fact]
    public void The_Scoped_Store_Differs_Across_Scopes_While_The_Singleton_Does_Not()
    {
        Services.AddScoped<ScopedCounter>();
        Services.AddSingleton<SingletonCounter>();

        Render<Ex044_SingletonVsScopedState>();

        var rootScoped = Services.GetRequiredService<ScopedCounter>();
        var rootSingleton = Services.GetRequiredService<SingletonCounter>();

        using var innerScope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var innerScoped = innerScope.ServiceProvider.GetRequiredService<ScopedCounter>();
        var innerSingleton = innerScope.ServiceProvider.GetRequiredService<SingletonCounter>();

        Assert.NotSame(rootScoped, innerScoped);
        Assert.Same(rootSingleton, innerSingleton);
    }
}
