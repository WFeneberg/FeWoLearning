namespace FeWoLearning.Architecture.Exercises.Desktop.Ex027;

/// <summary>
/// The UI thread, as a port. Every desktop framework has this - Dispatcher,
/// SynchronizationContext, DispatcherQueue - and every one of them is untestable
/// without a real window. Behind this interface, none of that matters.
/// </summary>
public interface IUiDispatcher
{
    bool IsOnUiThread { get; }

    void Post(Action action);
}

// Exercise 027 — ThreadMarshallingAbstraction (desktop).
// Goal:   Update UI-bound state from any thread, marshalling ONLY when marshalling is
//         actually needed.
// Drills: dispatcher as a port, conditional marshalling, testable synchronisation.
// Passes: on the UI thread  - Report applies the change IMMEDIATELY and posts nothing.
//         off the UI thread - Report posts and does NOT apply the change yet; the change
//                             lands when the dispatcher runs its queue.
//         ordering          - several off-thread reports arrive in the order they were made.
//
// The "immediately" clause is the whole exercise. Posting unconditionally is simpler,
// always correct in the thread-safety sense, and produces the bug where code that
// reports and then reads its own state sees the old value - because the update is
// sitting in a queue that will not run until the current call returns. It presents as
// "the list is empty for one frame", or as a save button that stays disabled until the
// user moves the mouse.
public sealed class ProgressViewModel(IUiDispatcher dispatcher)
{
    public List<string> Items { get; } = [];

    public void Report(string item) =>
        throw new NotImplementedException(
            "TODO: Ex027 - add the item directly when already on the UI thread, otherwise post the addition");
}
