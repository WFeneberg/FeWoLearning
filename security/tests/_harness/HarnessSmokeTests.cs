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
    public void Wpf_Harness_Applies_A_Control_Template()
    {
        var box = SmokeProbe.MakeTextBox();

        box.Measure(new System.Windows.Size(200, 50));
        box.Arrange(new System.Windows.Rect(0, 0, 200, 50));
        WpfPump.Pump();

        Assert.True(box.ActualWidth > 0);
        Assert.Equal("smoke", box.Text);
    }
}
