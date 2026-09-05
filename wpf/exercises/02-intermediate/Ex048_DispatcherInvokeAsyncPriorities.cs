// Exercise 048 - Dispatcher.InvokeAsync and execution order across priorities (intermediate).
// Goal:   A Dispatcher does not drain its queue first-in-first-out - it always runs everything
//         pending at its highest waiting priority before touching anything lower, no matter
//         what order things were queued in. Queue several callbacks at several different
//         priorities, then let the dispatcher drain, and the order they actually ran in is
//         priority order, not call order.
// Drills: Dispatcher.InvokeAsync (which returns a DispatcherOperation whose own .Task you can
//         await), queuing every item BEFORE awaiting any of them - the only way "order across
//         priorities" is a deterministic fact instead of a race between queuing and draining -
//         and awaiting several DispatcherOperations together via Task.WhenAll.
// Passes: dotnet test --filter FullyQualifiedName~Ex048_

using System.Windows.Threading;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex048_DispatcherPriorityQueue
{
    /// <summary>
    /// Queues every <c>(Priority, Callback)</c> pair in <paramref name="items"/> onto
    /// <paramref name="dispatcher"/> - via <c>dispatcher.InvokeAsync(item.Callback, item.Priority)</c>,
    /// in the order given, collecting each call's <see cref="DispatcherOperation.Task"/> WITHOUT
    /// awaiting any of them yet, so every item is already queued, at its own real priority,
    /// before the dispatcher gets a chance to run any of them. Only once every item has been
    /// queued this way should the returned task await all of those collected tasks together
    /// (<c>Task.WhenAll</c>) and let the dispatcher actually drain.
    /// </summary>
    public static Task RunAllAsync(Dispatcher dispatcher, IReadOnlyList<(DispatcherPriority Priority, Action Callback)> items) =>
        throw new NotImplementedException("TODO: Ex048 - for each item, call dispatcher.InvokeAsync(item.Callback, item.Priority) and collect its .Task, queuing every item before awaiting any of them; then Task.WhenAll the collected tasks and await that");
}
