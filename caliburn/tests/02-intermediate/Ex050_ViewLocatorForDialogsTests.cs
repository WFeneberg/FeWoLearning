using System.Windows;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex050_ViewLocatorForDialogsTests : CaliburnViewContext
{
    // ShowDialogAndCloseAsync (used below) lives on CaliburnViewContext.

    [WpfFact]
    public void A_Window_Derived_View_Resolves_To_A_Window()
    {
        Assert.True(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_WindowShapedViewModel)));
    }

    [WpfFact]
    public void A_UserControl_View_Does_Not_Resolve_To_A_Window()
    {
        Assert.False(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_PlainViewModel)));
    }

    [WpfFact]
    public void An_Arbitrary_FrameworkElement_View_Also_Does_Not_Resolve_To_A_Window()
    {
        // A stub that checked `is UserControl` instead of `is Window` (the wrong half of the
        // condition to test) would wrongly say true for a plain UserControl and never notice
        // it had the check backwards until faced with a THIRD shape like this one.
        Assert.False(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_GridShapedViewModel)));
    }

    [WpfFact]
    public async Task A_Window_Derived_View_Is_Used_As_The_Dialogs_Own_Hosting_Window()
    {
        // Ties this test to the exercise's own method too - the real dialog behaviour below is
        // pure Caliburn/WPF and would hold regardless of ResolvesToAWindow, so without this line
        // the test would pass even against an untouched stub.
        Assert.True(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_WindowShapedViewModel)));

        var vm = new Ex050_WindowShapedViewModel();
        var (_, window) = await ShowDialogAndCloseAsync(vm, true);

        // The hosting window IS the located view instance - not merely the same type.
        Assert.IsType<Ex050_WindowShapedView>(window);
    }

    [WpfFact]
    public async Task A_UserControl_View_Is_Wrapped_In_A_Bare_Window_Holding_It_As_Content()
    {
        Assert.False(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_PlainViewModel)));

        var vm = new Ex050_PlainViewModel();
        var (_, window) = await ShowDialogAndCloseAsync(vm, true);

        Assert.Equal(typeof(Window), window.GetType()); // bare Window, not a subclass
        Assert.IsType<Ex050_PlainView>(window.Content);
    }

    [WpfFact]
    public async Task After_Either_Shape_Closes_GetView_Returns_Null()
    {
        Assert.True(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_WindowShapedViewModel)));
        Assert.False(Ex050_ViewLocatorForDialogs.ResolvesToAWindow(typeof(Ex050_PlainViewModel)));

        var windowShapedVm = new Ex050_WindowShapedViewModel();
        await ShowDialogAndCloseAsync(windowShapedVm, true);
        Assert.Null(((IViewAware)windowShapedVm).GetView());

        var plainVm = new Ex050_PlainViewModel();
        await ShowDialogAndCloseAsync(plainVm, true);
        Assert.Null(((IViewAware)plainVm).GetView());
    }
}
