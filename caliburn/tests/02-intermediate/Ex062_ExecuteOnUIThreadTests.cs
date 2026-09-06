using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex062_ExecuteOnUIThreadTests : CaliburnViewContext
{
    [WpfFact]
    public async Task OnUIThread_Marshals_Background_Work_Back_Onto_The_Real_UI_Thread()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        var (backgroundThreadId, callbackThreadId) = await BoundedAsync(
            subject.RunOnUIThreadFromBackgroundAsync(), "Execute.OnUIThread to marshal back to the UI thread");

        // A stub that never really leaves the calling thread (e.g. skips Task.Run and just
        // invokes the callback inline) would show backgroundThreadId == callbackThreadId here.
        Assert.NotEqual(backgroundThreadId, callbackThreadId);
        Assert.Equal(uiThreadId, callbackThreadId);
    }

    [WpfFact]
    public async Task OnUIThreadAsync_Also_Marshals_Background_Work_Back_Onto_The_Real_UI_Thread()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        var (backgroundThreadId, callbackThreadId) = await BoundedAsync(
            subject.RunOnUIThreadAsyncFromBackgroundAsync(), "Execute.OnUIThreadAsync to marshal back to the UI thread");

        Assert.NotEqual(backgroundThreadId, callbackThreadId);
        Assert.Equal(uiThreadId, callbackThreadId);
    }

    [WpfFact]
    public async Task BeginOnUIThread_Fire_And_Forget_Still_Lands_Its_Callback_On_The_UI_Thread()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        // BeginOnUIThread itself returns void - a stub that forgets to signal completion at all
        // would hang here instead of failing, hence the bounded wait.
        var (backgroundThreadId, callbackThreadId) = await BoundedAsync(
            subject.RunBeginOnUIThreadFromBackgroundAsync(), "Execute.BeginOnUIThread's callback to run");

        Assert.NotEqual(backgroundThreadId, callbackThreadId);
        Assert.Equal(uiThreadId, callbackThreadId);
    }

    [WpfFact]
    public async Task Under_The_Default_Platform_Provider_The_Same_Call_Runs_Inline_Instead_Of_Marshalling()
    {
        // Cleaned up automatically - CaliburnViewContext resets PlatformProvider.Current per test.
        PlatformProvider.Current = new DefaultPlatformProvider();
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        var (backgroundThreadId, callbackThreadId) = await BoundedAsync(
            subject.RunOnUIThreadFromBackgroundAsync(), "Execute.OnUIThread to run (inline, under DefaultPlatformProvider)");

        // No marshal at all under this provider - the callback runs on the SAME background
        // thread that called it, never back on the UI thread.
        Assert.Equal(backgroundThreadId, callbackThreadId);
        Assert.NotEqual(uiThreadId, backgroundThreadId);
    }

    [WpfFact]
    public void Execute_OnUIThread_Called_From_The_UI_Thread_Itself_Just_Runs_There_Directly()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        var callbackThreadId = subject.RunOnUIThreadFromCallingThread();

        Assert.Equal(uiThreadId, callbackThreadId);
    }
}
