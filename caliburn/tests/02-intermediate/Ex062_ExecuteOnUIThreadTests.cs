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
    public async Task Execute_OnUIThread_Runs_Directly_When_Called_From_The_UI_Thread_And_Still_Marshals_When_Called_From_A_Pool_Thread()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var subject = new Ex062_ExecuteOnUIThread();

        var directCallbackThreadId = subject.RunOnUIThreadFromCallingThread();
        Assert.Equal(uiThreadId, directCallbackThreadId);

        // A stub that never really calls Execute.OnUIThread at all - just
        // "return Environment.CurrentManagedThreadId;" - would pass the assertion above for
        // free, because the UI thread IS the calling thread there. Calling the SAME method from
        // a pool thread tells them apart: the cheat returns the pool thread's own id, while a
        // real Execute.OnUIThread marshals the callback back onto the UI thread before this
        // (synchronous) method returns, so the result is uiThreadId either way.
        var marshalledCallbackThreadId = await BoundedAsync(
            Task.Run(() => subject.RunOnUIThreadFromCallingThread()),
            "Execute.OnUIThread to marshal a pool-thread call back onto the UI thread");
        Assert.Equal(uiThreadId, marshalledCallbackThreadId);
    }
}
