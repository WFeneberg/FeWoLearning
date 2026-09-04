using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex053_JsInteropUnmatchedInvocationTests : BunitContext
{
    // Strict is bUnit's default JSInterop.Mode - no Setup exists for "app.unplanned",
    // so the mock rejects the call instead of silently accepting it.
    [Fact]
    public void An_Unplanned_Call_Throws_In_The_Default_Strict_Mode()
    {
        var cut = Render<Ex053_JsInteropUnmatchedInvocation>();

        Assert.Throws<JSRuntimeUnhandledInvocationException>(() => cut.Find("#unplanned").Click());
    }

    // Non-vacuity: without switching JSInterop.Mode, this exact same click throws
    // instead of completing - verified directly.
    [Fact]
    public void An_Unplanned_Call_Is_Recorded_Once_The_Mock_Is_Loose()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<Ex053_JsInteropUnmatchedInvocation>();
        cut.Find("#unplanned").Click();

        JSInterop.VerifyInvoke("app.unplanned");
    }
}
