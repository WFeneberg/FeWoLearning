using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex050_JsInteropReturnValueTests : BunitContext
{
    [Fact]
    public void Renders_What_Js_Returned()
    {
        JSInterop.Setup<string>("app.load").SetResult("payload");

        var cut = Render<Ex050_JsInteropReturnValue>();

        Assert.Equal("payload", cut.Find("#loaded").TextContent);
    }

    [Fact]
    public void Invokes_App_Load_Exactly_Once()
    {
        JSInterop.Setup<string>("app.load").SetResult("payload");

        Render<Ex050_JsInteropReturnValue>();

        JSInterop.VerifyInvoke("app.load");
    }

    // Non-vacuity: a hard-coded return (e.g. always "payload") would pass the fact
    // above by coincidence, so a second, independently configured render is needed
    // to pin down that the *returned* value drives markup, not a lucky literal.
    [Fact]
    public void A_Differently_Configured_Js_Call_Renders_Its_Own_Result()
    {
        JSInterop.Setup<string>("app.load").SetResult("something-else");

        var cut = Render<Ex050_JsInteropReturnValue>();

        Assert.Equal("something-else", cut.Find("#loaded").TextContent);
    }
}
