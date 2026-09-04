// Exercise 063 - Dispatcher Seam (intermediate).
// Goal:   Touch the UI thread from a view model without making the view model untestable.
// Drills: the has-access guard (run inline when you already are on the thread, enqueue
//         otherwise), an interface as the seam, and an adapter over the real
//         DispatcherQueue.
// Passes: dotnet test --filter FullyQualifiedName~Ex063_
//
// A view model that calls DispatcherQueue.GetForCurrentThread() directly can only be
// tested where that returns something - which in practice means "in the app". One
// interface later, both branches are testable and the Uno-specific part shrinks to the
// adapter at the bottom of this file.
//
// The headless harness dispatches inline and reports HasThreadAccess as true always (see
// uno/README.md), so the seam is not a convenience here: it is the only way to exercise
// the enqueue branch at all.

using Microsoft.UI.Dispatching;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>The seam: everything a view model needs to know about the UI thread.</summary>
public interface IUiDispatcher
{
    /// <summary>Whether the caller is already on the UI thread.</summary>
    bool HasThreadAccess { get; }

    /// <summary>Queues <paramref name="work"/> for the UI thread.</summary>
    void Enqueue(Action work);
}

public static class Ex063_DispatcherSeam
{
    /// <summary>
    /// Runs <paramref name="work"/> on the UI thread and reports how: inline when the
    /// caller already has access, queued when it does not.
    /// </summary>
    /// <returns>True when the work ran inline, false when it was queued.</returns>
    public static bool RunOnUi(IUiDispatcher dispatcher, Action work)
    {
        if (!dispatcher.HasThreadAccess)
        {
            dispatcher.Enqueue(work);
            return false;
        }

        // The guard is not an optimisation. Enqueueing unconditionally turns every
        // synchronous update into one that lands a frame later, which is how a read taken
        // straight after a write ends up seeing the old value.
        work();
        return true;
    }
}

/// <summary>
/// The Uno-specific half: everything above this line is testable without a UI thread.
/// </summary>
public sealed class UnoUiDispatcher : IUiDispatcher
{
    private readonly DispatcherQueue _queue;

    public UnoUiDispatcher(DispatcherQueue queue) => _queue = queue;

    /// <summary>
    /// The queue for the calling thread, or null off the UI thread. The null case cannot be
    /// reached in the headless harness - every thread there reports thread access - so the
    /// guard is written for the app and documented rather than tested.
    /// </summary>
    public static UnoUiDispatcher? ForCurrentThread() =>
        // Null on a thread with no queue. Wrapping the null instead would move the failure
        // to the first Enqueue, far away from the mistake.
        DispatcherQueue.GetForCurrentThread() is { } queue ? new UnoUiDispatcher(queue) : null;

    public bool HasThreadAccess => _queue.HasThreadAccess;

    public void Enqueue(Action work) => _queue.TryEnqueue(() => work());
}
