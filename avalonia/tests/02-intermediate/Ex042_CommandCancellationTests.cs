using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex042_CommandCancellationTests
{
    // Bounded wait around any TaskCompletionSource this file gates on: if the real
    // mechanism never fires (e.g. a solution that discards the token, so the work
    // never sees cancellation and the completion source is never set) this fails
    // loudly with a named cause instead of hanging dotnet test - the same discipline
    // ex039's ExecuteWithoutBlocking uses for a different failure mode.
    private static async Task<T> WithTimeout<T>(Task<T> task, string message, CancellationToken ct)
    {
        var winner = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5), ct));
        Assert.True(winner == task, message);
        return await task;
    }

    [Fact]
    public async Task Disposing_The_Execution_Cancels_The_Token_The_Work_Body_Receives()
    {
        var started = new TaskCompletionSource();
        var cancelObserved = new TaskCompletionSource<bool>();

        var vm = new Ex042_CommandCancellationViewModel(async ct =>
        {
            started.SetResult();
            try
            {
                await Task.Delay(Timeout.Infinite, ct);
                return "completed-without-cancellation";
            }
            catch (OperationCanceledException)
            {
                cancelObserved.SetResult(true);
                throw;
            }
        });

        var sub = vm.RunCommand.Execute().Subscribe(_ => { }, _ => { });
        await WithTimeout(started.Task.ContinueWith(_ => true, TestContext.Current.CancellationToken),
            "the work body never started within 5s", TestContext.Current.CancellationToken);

        sub.Dispose();

        var observed = await WithTimeout(cancelObserved.Task,
            "the work body never observed cancellation within 5s after disposing the execution - " +
            "check that the real CancellationToken is forwarded into _work, not discarded " +
            "(e.g. via CancellationToken.None).",
            TestContext.Current.CancellationToken);

        Assert.True(observed);
    }

    // A second run on a fresh view model with a differently-shaped work body: guards
    // against a solution that only happens to work for one particular cancellation
    // pattern (e.g. one relying on a specific exception subtype).
    [Fact]
    public async Task A_Second_Differently_Shaped_Cancellable_Body_Also_Observes_Cancellation()
    {
        var started = new TaskCompletionSource();
        var cancelObserved = new TaskCompletionSource<bool>();

        var vm = new Ex042_CommandCancellationViewModel(async ct =>
        {
            started.SetResult();
            var tcs = new TaskCompletionSource<string>();
            await using (ct.Register(() =>
            {
                cancelObserved.TrySetResult(true);
                tcs.TrySetCanceled(ct);
            }))
            {
                return await tcs.Task;
            }
        });

        var sub = vm.RunCommand.Execute().Subscribe(_ => { }, _ => { });
        await WithTimeout(started.Task.ContinueWith(_ => true, TestContext.Current.CancellationToken),
            "the work body never started within 5s", TestContext.Current.CancellationToken);

        sub.Dispose();

        var observed = await WithTimeout(cancelObserved.Task,
            "ct.Register callback never fired within 5s after disposing the execution - " +
            "the real token was not forwarded.",
            TestContext.Current.CancellationToken);

        Assert.True(observed);
    }
}
