using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex045_CascadingServiceInjectionTests : BunitContext
{
    // Cascades a store that is DIFFERENT from the one registered in DI, so
    // #via-property and #via-cascade reading different values is what actually shows
    // the two mechanisms are independent - registering and cascading the very same
    // instance would make them agree by object identity alone and prove nothing.
    [Fact]
    public void With_A_Cascaded_Store_Each_Span_Reads_Its_Own_Source()
    {
        var injected = new CounterStore();
        injected.Increment();
        Services.AddSingleton(injected);

        var cascaded = new CounterStore();
        cascaded.Increment();
        cascaded.Increment();
        cascaded.Increment();

        var cut = Render<CascadingValue<CounterStore>>(p => p
            .Add(c => c.Value, cascaded)
            .AddChildContent<Ex045_CascadingServiceInjection>());

        Assert.Equal("1", cut.Find("#via-property").TextContent);
        Assert.Equal("3", cut.Find("#via-cascade").TextContent);
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
