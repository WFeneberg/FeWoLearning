using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex047_OptionsPatternComponentTests : BunitContext
{
    // Non-vacuity: hard-coding "Hello" in Greeting passes the default-options fact
    // below but fails this one, since the registered options use a different prefix.
    [Fact]
    public void Uses_The_Injected_Options_Prefix()
    {
        Services.AddSingleton<IOptions<Ex047_OptionsPatternComponent.GreetingOptions>>(
            Options.Create(new Ex047_OptionsPatternComponent.GreetingOptions { Prefix = "Moin" }));

        var cut = Render<Ex047_OptionsPatternComponent>(p => p.Add(c => c.Name, "Ada"));

        Assert.Equal("Moin, Ada!", cut.Find("#greeting").TextContent);
    }

    [Fact]
    public void Falls_Back_To_The_Default_Options_Prefix()
    {
        Services.AddSingleton<IOptions<Ex047_OptionsPatternComponent.GreetingOptions>>(
            Options.Create(new Ex047_OptionsPatternComponent.GreetingOptions()));

        var cut = Render<Ex047_OptionsPatternComponent>(p => p.Add(c => c.Name, "Ada"));

        Assert.Equal("Hello, Ada!", cut.Find("#greeting").TextContent);
    }

    [Fact]
    public void Changing_Name_Updates_The_Greeting()
    {
        Services.AddSingleton<IOptions<Ex047_OptionsPatternComponent.GreetingOptions>>(
            Options.Create(new Ex047_OptionsPatternComponent.GreetingOptions { Prefix = "Moin" }));

        var cut = Render<Ex047_OptionsPatternComponent>(p => p.Add(c => c.Name, "Ada"));
        cut.Render(p => p.Add(c => c.Name, "Ben"));

        Assert.Equal("Moin, Ben!", cut.Find("#greeting").TextContent);
    }
}
