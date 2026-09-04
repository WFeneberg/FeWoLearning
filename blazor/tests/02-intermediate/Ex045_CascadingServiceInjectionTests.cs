using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex045_CascadingServiceInjectionTests : BunitContext
{
    [Fact]
    public void With_A_Cascaded_Store_Both_Spans_Read_The_Same_Value()
    {
        var store = new CounterStore();
        store.Increment();
        Services.AddSingleton(store);

        var cut = Render<CascadingValue<CounterStore>>(p => p
            .Add(c => c.Value, store)
            .AddChildContent<Ex045_CascadingServiceInjection>());

        Assert.Equal("1", cut.Find("#via-property").TextContent);
        Assert.Equal("1", cut.Find("#via-cascade").TextContent);
    }

    [Fact]
    public void Without_A_Cascaded_Value_Only_The_Injected_Path_Resolves()
    {
        var store = new CounterStore();
        store.Increment();
        store.Increment();
        Services.AddSingleton(store);

        var cut = Render<Ex045_CascadingServiceInjection>();

        Assert.Equal("2", cut.Find("#via-property").TextContent);
        Assert.Equal("", cut.Find("#via-cascade").TextContent);
    }

    // Non-vacuity: reading Store.Value in OnParametersSet instead of OnInitialized
    // would re-capture it on this re-render too, and this assertion would see "1"
    // instead of the frozen initial "0".
    [Fact]
    public void Advancing_The_Store_After_Init_Does_Not_Change_The_Injected_Reading()
    {
        var store = new CounterStore();
        Services.AddSingleton(store);

        var cut = Render<Ex045_CascadingServiceInjection>();
        Assert.Equal("0", cut.Find("#via-property").TextContent);

        store.Increment();
        cut.Render();

        Assert.Equal("0", cut.Find("#via-property").TextContent);
    }
}
