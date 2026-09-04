using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex010_ScreenGuardCloseTests : CaliburnCoreContext
{
    [Fact]
    public async Task Fresh_Screen_With_No_Unsaved_Changes_Can_Close_By_Default()
    {
        var vm = new Ex010_ScreenGuardClose();

        var canClose = await vm.CanCloseAsync();

        Assert.True(canClose);
    }

    [Fact]
    public async Task Unsaved_Changes_With_No_Confirmation_Delegate_Refuses_To_Close()
    {
        var vm = new Ex010_ScreenGuardClose { HasUnsavedChanges = true };

        var canClose = await vm.CanCloseAsync();

        Assert.False(canClose);
    }

    [Fact]
    public async Task Unsaved_Changes_Genuinely_Awaits_The_Confirmation_Before_Deciding()
    {
        var vm = new Ex010_ScreenGuardClose { HasUnsavedChanges = true };
        var confirmation = new TaskCompletionSource<bool>();
        vm.ConfirmDiscardAsync = () => confirmation.Task;

        var closeTask = vm.CanCloseAsync();

        // Not resolved yet - a real implementation must suspend on the confirmation, not
        // just wrap a synchronous decision in an already-completed Task.
        Assert.False(closeTask.IsCompleted);

        confirmation.SetResult(true);
        var canClose = await closeTask;

        Assert.True(canClose);
    }

    [Fact]
    public async Task Unsaved_Changes_Reports_False_When_The_User_Declines()
    {
        var vm = new Ex010_ScreenGuardClose
        {
            HasUnsavedChanges = true,
            ConfirmDiscardAsync = () => Task.FromResult(false),
        };

        var canClose = await vm.CanCloseAsync();

        Assert.False(canClose);
    }

    [Fact]
    public async Task No_Unsaved_Changes_Never_Consults_The_Confirmation_Delegate()
    {
        var wasAsked = false;
        var vm = new Ex010_ScreenGuardClose
        {
            HasUnsavedChanges = false,
            ConfirmDiscardAsync = () => { wasAsked = true; return Task.FromResult(true); },
        };

        var canClose = await vm.CanCloseAsync();

        Assert.True(canClose);
        // Assert the negative: nothing to discard means nothing to ask about.
        Assert.False(wasAsked);
    }
}
