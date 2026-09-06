// Exercise 062 - Execute On UI Thread (intermediate).
// Goal:   Learn Execute's three marshalling statics directly, and the ONE PlatformProvider fact
//         that decides what they actually do: Execute.OnUIThread/OnUIThreadAsync/BeginOnUIThread
//         all forward to PlatformProvider.Current - under the harness's view context
//         (XamlPlatformProvider, capturing a real Dispatcher), a call made from a background
//         thread genuinely marshals the callback back onto the UI thread; under the plain
//         core context's DefaultPlatformProvider, the SAME call runs the callback INLINE, on
//         whatever thread called it - no marshal at all.
// Drills: starting real background work (Task.Run), calling each of Execute.OnUIThread,
//         Execute.OnUIThreadAsync and Execute.BeginOnUIThread from inside it, and reporting BOTH
//         the background thread's own id and the id the callback actually ran on - so a test can
//         tell "marshalled" (the two ids differ, and the callback's matches the real UI thread)
//         apart from "ran inline" (the two ids are the same).
// Passes: dotnet test --filter FullyQualifiedName~Ex062_
//
// Measured on this machine (Caliburn.Micro 5.0.258), calling Execute.OnUIThread from a
// ThreadPool thread: under XamlPlatformProvider the call BLOCKS that pool thread until the
// callback has actually run on the real UI thread (a synchronous Dispatcher.Invoke under the
// hood, not a fire-and-forget BeginInvoke) - by the time the call returns, the callback has
// already run. Under DefaultPlatformProvider the SAME call runs the callback immediately, on
// that same pool thread, and returns just as fast - there is no UI thread to marshal to at all.
// Execute.OnUIThreadAsync behaves the same as OnUIThread with respect to WHICH thread runs the
// callback, but is awaitable rather than blocking. Execute.BeginOnUIThread is fire-and-forget
// (returns void) - its callback still lands on the UI thread under XamlPlatformProvider, just
// without the caller ever being told when.

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex062_ExecuteOnUIThread
{
    /// <summary>Starts a background Task.Run that calls Execute.OnUIThread, reporting both the
    /// background thread's own id and whatever thread the callback actually ran on.</summary>
    public Task<(int BackgroundThreadId, int CallbackThreadId)> RunOnUIThreadFromBackgroundAsync()
    {
        var tcs = new TaskCompletionSource<(int, int)>();
        Task.Run(() =>
        {
            var backgroundThreadId = Environment.CurrentManagedThreadId;
            var callbackThreadId = -1;
            Execute.OnUIThread(() => callbackThreadId = Environment.CurrentManagedThreadId);
            tcs.SetResult((backgroundThreadId, callbackThreadId));
        });
        return tcs.Task;
    }

    /// <summary>Same shape, using the AWAITABLE Execute.OnUIThreadAsync instead of OnUIThread.</summary>
    public Task<(int BackgroundThreadId, int CallbackThreadId)> RunOnUIThreadAsyncFromBackgroundAsync()
    {
        var tcs = new TaskCompletionSource<(int, int)>();
        Task.Run(async () =>
        {
            var backgroundThreadId = Environment.CurrentManagedThreadId;
            var callbackThreadId = -1;
            await Execute.OnUIThreadAsync(() =>
            {
                callbackThreadId = Environment.CurrentManagedThreadId;
                return Task.CompletedTask;
            });
            tcs.SetResult((backgroundThreadId, callbackThreadId));
        });
        return tcs.Task;
    }

    /// <summary>Same shape again, using Execute.BeginOnUIThread - fire-and-forget, so the only
    /// way to learn the callback ran (and on which thread) is to signal it yourself.</summary>
    public Task<(int BackgroundThreadId, int CallbackThreadId)> RunBeginOnUIThreadFromBackgroundAsync()
    {
        var tcs = new TaskCompletionSource<(int, int)>();
        Task.Run(() =>
        {
            var backgroundThreadId = Environment.CurrentManagedThreadId;
            Execute.BeginOnUIThread(() => tcs.SetResult((backgroundThreadId, Environment.CurrentManagedThreadId)));
        });
        return tcs.Task;
    }

    /// <summary>Calls Execute.OnUIThread directly, on WHATEVER thread calls this method - no
    /// background thread involved at all.</summary>
    public int RunOnUIThreadFromCallingThread()
    {
        var callbackThreadId = -1;
        Execute.OnUIThread(() => callbackThreadId = Environment.CurrentManagedThreadId);
        return callbackThreadId;
    }
}
