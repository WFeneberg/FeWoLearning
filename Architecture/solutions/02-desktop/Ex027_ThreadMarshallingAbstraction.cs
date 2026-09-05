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

// Exercise 027 — ThreadMarshallingAbstraction (reference solution).
public sealed class ProgressViewModel(IUiDispatcher dispatcher)
{
    public List<string> Items { get; } = [];

    public void Report(string item)
    {
        if (dispatcher.IsOnUiThread)
        {
            // Already here: do it now. Posting instead would be thread-safe and would
            // still be wrong - the caller reads its own state on the next line and sees
            // the value from before, because the queue does not run until it returns.
            Items.Add(item);
            return;
        }

        dispatcher.Post(() => Items.Add(item));
    }
}
