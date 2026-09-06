using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex048_DialogResultTests : CaliburnViewContext
{
    // ShowDialogInvokingAsync (used below) lives on CaliburnViewContext: it schedules the
    // given closer to run from inside ShowDialogAsync's own nested modal frame and bounds the
    // wait, exactly like ShowDialogAndCloseAsync - just invoking the exercise's OWN method
    // instead of calling TryCloseAsync directly, since here the exercise IS which bool? gets
    // passed to TryCloseAsync.

    [WpfFact]
    public async Task ConfirmAsync_Resolves_ShowDialogAsync_To_True()
    {
        var vm = new Ex048_ConfirmableDialogVm();

        var (result, _) = await ShowDialogInvokingAsync(vm, vm.ConfirmAsync);

        Assert.True(result);
    }

    [WpfFact]
    public async Task DeclineAsync_Resolves_ShowDialogAsync_To_False()
    {
        var vm = new Ex048_ConfirmableDialogVm();

        var (result, _) = await ShowDialogInvokingAsync(vm, vm.DeclineAsync);

        Assert.False(result);
    }

    [WpfFact]
    public async Task DismissAsync_Resolves_ShowDialogAsync_To_False_Not_Null()
    {
        var vm = new Ex048_ConfirmableDialogVm();

        var (result, _) = await ShowDialogInvokingAsync(vm, vm.DismissAsync);

        // The whole lesson: bool? suggests three outcomes, but closing with null is
        // indistinguishable from closing with false once it reaches the caller.
        Assert.True(result.HasValue, "expected a real false, not an unset/null result");
        Assert.False(result);
    }

    [WpfFact]
    public async Task DismissAsync_Genuinely_Closes_The_Dialog_Not_Just_Resolves_False()
    {
        var vm = new Ex048_ConfirmableDialogVm();

        var (_, window) = await ShowDialogInvokingAsync(vm, vm.DismissAsync);

        // Captured while the dialog was still open - proves a real Window actually hosted it,
        // not that DismissAsync merely returned a value some other way (its signature, Task
        // with no return value, cannot do that - it must genuinely reach TryCloseAsync).
        Assert.NotNull(window);
        Assert.Null(((IViewAware)vm).GetView());
    }

    [WpfFact]
    public async Task All_Three_Outcomes_Are_Correct_Regardless_Of_Which_Order_They_Run_In()
    {
        // Deliberately reversed from the order above, with a fresh view model each time - a
        // stub that (say) hardcodes one bool? for every method, or leaks state through a shared
        // static, would fail at least one of these regardless of ordering.
        var dismissVm = new Ex048_ConfirmableDialogVm();
        var (dismissResult, _) = await ShowDialogInvokingAsync(dismissVm, dismissVm.DismissAsync);

        var declineVm = new Ex048_ConfirmableDialogVm();
        var (declineResult, _) = await ShowDialogInvokingAsync(declineVm, declineVm.DeclineAsync);

        var confirmVm = new Ex048_ConfirmableDialogVm();
        var (confirmResult, _) = await ShowDialogInvokingAsync(confirmVm, confirmVm.ConfirmAsync);

        Assert.False(dismissResult);
        Assert.False(declineResult);
        Assert.True(confirmResult);
    }
}
