using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex049_JsInteropInvokeTests : BunitContext
{
    // Non-vacuity: calling JS with an argument that does not match this strict-mode
    // setup (or omitting Payload) throws before _state is ever assigned, so this
    // fact - along with both below - fails on that mistake too, verified directly.
    [Fact]
    public void Clicking_Save_Records_State_As_Saved()
    {
        JSInterop.SetupVoid("app.save", "hi").SetVoidResult();

        var cut = Render<Ex049_JsInteropInvoke>(p => p.Add(c => c.Payload, "hi"));
        cut.Find("#save").Click();

        cut.WaitForAssertion(() => Assert.Equal("saved", cut.Find("#state").TextContent));
    }

    // Non-vacuity: setting _state without ever calling JS leaves no invocation for
    // VerifyInvoke to find, so this fact fails on a stub that skips the JS call.
    [Fact]
    public void Clicking_Save_Invokes_The_Js_Function_Exactly_Once()
    {
        JSInterop.SetupVoid("app.save", "hi").SetVoidResult();

        var cut = Render<Ex049_JsInteropInvoke>(p => p.Add(c => c.Payload, "hi"));
        cut.Find("#save").Click();

        JSInterop.VerifyInvoke("app.save");
    }

    // Non-vacuity: calling JS with the wrong argument (or none at all) never
    // matches this strict-mode setup, so the invocation this fact inspects would
    // not exist - the exercise's own throw is what makes this red for now.
    [Fact]
    public void Clicking_Save_Passes_The_Payload_As_The_Argument()
    {
        JSInterop.SetupVoid("app.save", "hi").SetVoidResult();

        var cut = Render<Ex049_JsInteropInvoke>(p => p.Add(c => c.Payload, "hi"));
        cut.Find("#save").Click();

        var invocation = JSInterop.VerifyInvoke("app.save");
        Assert.Equal("hi", Assert.Single(invocation.Arguments));
    }
}
