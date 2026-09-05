using ReactiveUI;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex046_InteractionDialogTests
{
    // The discriminator this whole exercise is about: a solution that never routes
    // through ConfirmDeletion (e.g. sets LastResult directly instead) never throws
    // here, because Interaction<,>.Handle(...) only throws UnhandledInteractionException
    // when it is genuinely reached with no handler registered. Measured on this
    // machine: it throws synchronously, not via a faulted task that silently vanishes.
    [Fact]
    public async Task Calling_DeleteAsync_With_No_Handler_Registered_Throws_Unhandled_And_Leaves_LastResult_Untouched()
    {
        var vm = new Ex046_InteractionDialogViewModel();

        // Nothing has run yet - see the arrange-phase note in Ex041's tests for why
        // this assertion belongs before the act, not after.
        Assert.Null(vm.LastResult);

        await Assert.ThrowsAsync<UnhandledInteractionException<string, bool>>(() => vm.DeleteAsync());

        Assert.Null(vm.LastResult);
    }

    // Drives the input through and checks it actually reached the handler - a
    // solution that hard-codes true/false without touching ConfirmDeletion at all
    // would already have failed the test above, but this also rules out a solution
    // that calls Handle with the wrong (or no) input.
    [Fact]
    public async Task A_Registered_Handler_Receives_The_Prompt_And_Its_Output_Becomes_LastResult()
    {
        var vm = new Ex046_InteractionDialogViewModel();
        string? receivedInput = null;
        vm.ConfirmDeletion.RegisterHandler(ctx =>
        {
            receivedInput = ctx.Input;
            ctx.SetOutput(true);
        });

        await vm.DeleteAsync();

        Assert.Equal("Delete this item?", receivedInput);
        Assert.Equal(true, vm.LastResult);
    }

    // A second, distinctly-valued handler on a fresh view model: guards against a
    // solution that hard-codes one boolean instead of genuinely relaying the
    // handler's answer.
    [Fact]
    public async Task A_Second_Distinctly_Valued_Handler_Also_Surfaces_Correctly()
    {
        var vm = new Ex046_InteractionDialogViewModel();
        vm.ConfirmDeletion.RegisterHandler(ctx => ctx.SetOutput(false));

        await vm.DeleteAsync();

        Assert.Equal(false, vm.LastResult);
    }
}
