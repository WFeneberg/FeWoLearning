using FeWoLearning.Architecture.Exercises.Desktop.Ex027;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex027_ThreadMarshallingAbstractionTests
{
    /// <summary>
    /// No thread anywhere. "Am I on the UI thread" is a question the port answers, so
    /// the test simply says yes or no and runs the queue by hand.
    /// </summary>
    private sealed class FakeDispatcher(bool onUiThread) : IUiDispatcher
    {
        public Queue<Action> Pending { get; } = new();

        public bool IsOnUiThread => onUiThread;

        public void Post(Action action) => Pending.Enqueue(action);

        public void Pump()
        {
            while (Pending.Count > 0)
                Pending.Dequeue()();
        }
    }

    [Fact]
    public void Mechanism_On_The_Ui_Thread_The_Change_Is_Applied_Immediately()
    {
        // The whole exercise. Posting unconditionally is simpler, is thread-safe, and
        // produces the bug where code that reports and then reads its own state sees the
        // old value - the update is in a queue that will not run until the current call
        // returns. It shows up as "the list is empty for one frame".
        var dispatcher = new FakeDispatcher(onUiThread: true);
        var viewModel = new ProgressViewModel(dispatcher);

        viewModel.Report("step 1");

        Assert.Equal(["step 1"], viewModel.Items);
        Assert.Empty(dispatcher.Pending);
    }

    [Fact]
    public void Mechanism_Off_The_Ui_Thread_The_Change_Is_Posted_Rather_Than_Applied()
    {
        // The other half. Applying directly from a background thread is the bug this
        // whole abstraction exists to prevent, and it is invisible until the day the
        // collection is bound to something.
        var dispatcher = new FakeDispatcher(onUiThread: false);
        var viewModel = new ProgressViewModel(dispatcher);

        viewModel.Report("step 1");

        Assert.Empty(viewModel.Items);
        Assert.Single(dispatcher.Pending);
    }

    [Fact]
    public void A_Posted_Change_Lands_When_The_Dispatcher_Runs()
    {
        var dispatcher = new FakeDispatcher(onUiThread: false);
        var viewModel = new ProgressViewModel(dispatcher);

        viewModel.Report("step 1");
        dispatcher.Pump();

        Assert.Equal(["step 1"], viewModel.Items);
    }

    [Fact]
    public void Posted_Changes_Keep_The_Order_They_Were_Reported_In()
    {
        // A progress list that arrives shuffled is worse than one that arrives late.
        var dispatcher = new FakeDispatcher(onUiThread: false);
        var viewModel = new ProgressViewModel(dispatcher);

        viewModel.Report("one");
        viewModel.Report("two");
        viewModel.Report("three");
        dispatcher.Pump();

        Assert.Equal(["one", "two", "three"], viewModel.Items);
    }
}
