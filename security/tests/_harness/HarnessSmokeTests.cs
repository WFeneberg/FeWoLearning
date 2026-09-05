using System.Net;
using Bunit;
using FeWoLearning.Security.Exercises.Support;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests;

// The canary for a package bump breaking a harness, the same role uno/'s
// HarnessSmokeTests plays. These three are the ONLY tests green on a red run.
public class HarnessSmokeTests
{
    [Fact]
    public async Task Web_Harness_Serves_A_Request()
    {
        await using var harness = await WebHarness.StartAsync(
            services: null,
            configure: SmokeProbe.Configure,
            ct: TestContext.Current.CancellationToken);

        var response = await harness.Client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ok", Assert.Single(response.Headers.GetValues("X-Smoke")));
        Assert.Equal("pong", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Blazor_Harness_Renders_A_Component()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<SmokeGreeter>(p => p.Add(c => c.Name, "world"));

        Assert.Equal("Hello, world", cut.Find("#smoke").TextContent);
    }

    [WpfFact]
    public void Wpf_Harness_Runs_Sta_And_Resolves_A_Default_Control_Template()
    {
        // The apartment state is what [WpfFact] itself buys; assert it, so a
        // StaFact regression is named rather than showing up as a cast exception
        // somewhere in block 04.
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());

        var button = SmokeProbe.MakeButton();

        button.Measure(new System.Windows.Size(200, 50));
        WpfPump.Pump();

        // Template plus DesiredSize, measured BEFORE any Arrange. Do not assert
        // ActualWidth after Arrange: a FrameworkElement defaults to
        // HorizontalAlignment.Stretch and fills whatever rect it is given, so
        // ActualWidth > 0 holds even with an empty template - the assertion cannot
        // fail, which makes it worthless as a canary. DesiredSize comes from Measure
        // and is 0x0 when template resolution breaks. This is the idiom
        // wpf/tests/_harness/HarnessSmokeTests.cs already uses and has verified.
        Assert.NotNull(button.Template);
        Assert.True(button.DesiredSize.Width > 0, "A templated Button must measure wider than 0.");
        Assert.True(button.DesiredSize.Height > 0, "A templated Button must measure taller than 0.");
    }
}
