using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

// bUnit's module handler (BunitJSModuleInterop) does not expose an IsDisposed-style
// property - confirmed against bunit 2.9.0's shipped API surface, where neither
// BunitJSModuleInterop nor BunitJSObjectReference (nor their JSRuntimeInvocationHandler
// base classes) declare one. Also probed directly: BunitJSModuleInterop.Invocations
// (inherited from JSRuntimeInvocationHandlerBase<T>) does NOT record the "import" call
// itself - it stayed empty even after a real import - so these facts observe the import
// through JSInterop.VerifyInvoke("import", ...) instead, per the brief's own
// pre-authorized fallback (exactly-once import, no reimport on a parameter push).
public class Ex052_JsInteropModuleTests : BunitContext
{
    // OnAfterRenderAsync is an async lifecycle method - per README §11 and the Ex050
    // precedent, a bare assertion right after Render<T>() only proves something about
    // the first frame and would reject a correct implementation that genuinely awaits
    // the import before setting _module.
    [Fact]
    public void Ready_Becomes_Yes_Once_The_Module_Is_Imported()
    {
        JSInterop.SetupModule("./app.js");

        var cut = Render<Ex052_JsInteropModule>();

        cut.WaitForAssertion(() => Assert.Equal("yes", cut.Find("#ready").TextContent));
    }

    [Fact]
    public void The_Module_Is_Imported_Exactly_Once()
    {
        JSInterop.SetupModule("./app.js");

        var cut = Render<Ex052_JsInteropModule>();

        cut.WaitForAssertion(() => JSInterop.VerifyInvoke("import", 1));
    }

    // Non-vacuity: dropping the firstRender guard (importing on every OnAfterRenderAsync
    // pass) imports a second time once this parameter push forces a re-render - verified
    // directly.
    [Fact]
    public void A_Parameter_Push_Does_Not_Reimport_The_Module()
    {
        JSInterop.SetupModule("./app.js");

        var cut = Render<Ex052_JsInteropModule>();
        cut.WaitForAssertion(() => JSInterop.VerifyInvoke("import", 1));

        // No [Parameter] on this component - an empty parameter push still forces a
        // full re-render (and another OnAfterRenderAsync(firstRender: false) pass)
        // without changing anything the component reads.
        cut.Render(_ => { });

        JSInterop.VerifyInvoke("import", 1);
    }
}
