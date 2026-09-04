using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex050_JsInteropReturnValueTests : BunitContext
{
    // OnInitializedAsync is an async lifecycle method - per README §11, a bare
    // assertion right after Render<T>() only proves something about the first
    // frame, and would reject a correct implementation that genuinely awaits
    // (verified: inserting a real `await Task.Delay(30)` before an otherwise
    // correct interop call fails all three facts in this class without
    // WaitForAssertion). Wrapped so a real await doesn't fail a fact that should
    // only care about the eventual, settled state.
    [Fact]
    public void Renders_What_Js_Returned()
    {
        JSInterop.Setup<string>("app.load").SetResult("payload");

        var cut = Render<Ex050_JsInteropReturnValue>();

        cut.WaitForAssertion(() => Assert.Equal("payload", cut.Find("#loaded").TextContent));
    }

    [Fact]
    public void Invokes_App_Load_Exactly_Once()
    {
        JSInterop.Setup<string>("app.load").SetResult("payload");

        var cut = Render<Ex050_JsInteropReturnValue>();

        cut.WaitForAssertion(() => JSInterop.VerifyInvoke("app.load"));
    }

    // Non-vacuity: a hard-coded return (e.g. always "payload") would pass the fact
    // above by coincidence, so a second, independently configured render is needed
    // to pin down that the *returned* value drives markup, not a lucky literal.
    [Fact]
    public void A_Differently_Configured_Js_Call_Renders_Its_Own_Result()
    {
        JSInterop.Setup<string>("app.load").SetResult("something-else");

        var cut = Render<Ex050_JsInteropReturnValue>();

        cut.WaitForAssertion(() => Assert.Equal("something-else", cut.Find("#loaded").TextContent));
    }
}
