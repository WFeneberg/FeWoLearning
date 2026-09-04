using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex063_DispatcherSeamTests : UnoTestContext
{
    /// <summary>
    /// The fake the seam exists for: it can claim to be off the UI thread, which the real
    /// harness dispatcher never does.
    /// </summary>
    private sealed class FakeDispatcher(bool hasThreadAccess) : IUiDispatcher
    {
        public List<Action> Queued { get; } = [];

        public bool HasThreadAccess { get; } = hasThreadAccess;

        public void Enqueue(Action work) => Queued.Add(work);

        public void Pump()
        {
            foreach (var work in Queued)
            {
                work();
            }

            Queued.Clear();
        }
    }

    [Fact]
    public void With_Access_The_Work_Runs_Inline()
    {
        var dispatcher = new FakeDispatcher(hasThreadAccess: true);
        var ran = false;

        var inline = Ex063_DispatcherSeam.RunOnUi(dispatcher, () => ran = true);

        Assert.True(inline);
        Assert.True(ran);
        Assert.Empty(dispatcher.Queued);
    }

    [Fact]
    public void Without_Access_The_Work_Is_Queued()
    {
        var dispatcher = new FakeDispatcher(hasThreadAccess: false);
        var ran = false;

        var inline = Ex063_DispatcherSeam.RunOnUi(dispatcher, () => ran = true);

        Assert.False(inline);
        Assert.False(ran);
        Assert.Single(dispatcher.Queued);
    }

    [Fact]
    public void Queued_Work_Runs_When_The_Thread_Gets_To_It()
    {
        var dispatcher = new FakeDispatcher(hasThreadAccess: false);
        var ran = false;
        Ex063_DispatcherSeam.RunOnUi(dispatcher, () => ran = true);

        dispatcher.Pump();

        Assert.True(ran);
    }

    [Fact]
    public void Inline_Work_Is_Visible_Immediately_After_The_Call()
    {
        var dispatcher = new FakeDispatcher(hasThreadAccess: true);
        var value = 0;

        Ex063_DispatcherSeam.RunOnUi(dispatcher, () => value = 42);

        // This is why the guard is not just an optimisation: enqueueing unconditionally
        // makes every read straight after a write see the old value.
        Assert.Equal(42, value);
    }

    [Fact]
    public void Several_Pieces_Of_Queued_Work_Keep_Their_Order()
    {
        var dispatcher = new FakeDispatcher(hasThreadAccess: false);
        var order = new List<int>();

        Ex063_DispatcherSeam.RunOnUi(dispatcher, () => order.Add(1));
        Ex063_DispatcherSeam.RunOnUi(dispatcher, () => order.Add(2));
        dispatcher.Pump();

        Assert.Equal([1, 2], order);
    }

    [Fact]
    public void The_Uno_Adapter_Wraps_The_Current_Queue()
    {
        var adapter = UnoUiDispatcher.ForCurrentThread();

        // The harness installs a dispatcher for the test thread, so there is one to wrap -
        // and it reports access, which is exactly why the fake above is needed for the
        // other branch.
        Assert.NotNull(adapter);
        Assert.True(adapter!.HasThreadAccess);
    }

    [Fact]
    public void The_Uno_Adapter_Runs_Enqueued_Work()
    {
        var adapter = UnoUiDispatcher.ForCurrentThread()!;
        var ran = false;

        adapter.Enqueue(() => ran = true);

        // The harness dispatches inline, so this is observable straight away. In an app it
        // would happen on the next turn of the UI thread.
        Assert.True(ran);
    }

    [Fact]
    public async Task Every_Thread_Looks_Like_The_Ui_Thread_In_This_Harness()
    {
        var onPool = await Task.Run(() => UnoUiDispatcher.ForCurrentThread());

        // In an app, DispatcherQueue.GetForCurrentThread() returns null on a thread that
        // has none, and the adapter's null-guard is what keeps that from turning into a
        // NullReferenceException at the first Enqueue. The harness installs one dispatcher
        // and reports thread access from everywhere (see uno/README.md), so that branch
        // cannot be reached here - which is the whole argument for the seam above.
        Assert.NotNull(onPool);
        Assert.True(onPool!.HasThreadAccess);
    }
}
