using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex051_JsInteropElementReferenceTests : BunitContext
{
    [Fact]
    public void Clicking_Focus_Invokes_The_Js_Function_Exactly_Once()
    {
        JSInterop.SetupVoid("app.focus", _ => true).SetVoidResult();

        var cut = Render<Ex051_JsInteropElementReference>();
        cut.Find("#focus").Click();

        JSInterop.VerifyInvoke("app.focus");
    }

    // Non-vacuity: passing the selector string "#target" instead of the captured
    // ElementReference would still satisfy the loose invocation matcher above (it
    // matches any argument), so only asserting that the recorded argument actually
    // *is* an ElementReference to this element - via bUnit's ShouldBeElementReferenceTo -
    // rejects that. Verified directly: a stub that calls
    // JS.InvokeVoidAsync("app.focus", "#target") passes VerifyInvoke above but fails
    // this fact.
    [Fact]
    public void Clicking_Focus_Passes_The_Captured_Element_Not_A_Selector_String()
    {
        JSInterop.SetupVoid("app.focus", _ => true).SetVoidResult();

        var cut = Render<Ex051_JsInteropElementReference>();
        cut.Find("#focus").Click();

        var invocation = JSInterop.VerifyInvoke("app.focus");
        var argument = Assert.Single(invocation.Arguments);
        argument.ShouldBeElementReferenceTo(cut.Find("#target"));
    }
}
