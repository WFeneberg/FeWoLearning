using System.Threading.Channels;
using FeWoLearning.Uno.Exercises.Expert;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex093_AsyncStreamViewModelTests : UnoTestContext
{
    /// <summary>
    /// A stream the test feeds. An unbounded channel rather than a task per item: the
    /// producer does not have to exist before the test pushes, which is what made the
    /// first version of this fixture racy.
    /// </summary>
    private sealed class ControlledStream
    {
        private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

        public void Push(string item) => _channel.Writer.TryWrite(item);

        public void Complete() => _channel.Writer.TryComplete();

        public IAsyncEnumerable<string> Enumerate() => _channel.Reader.ReadAllAsync();
    }

    private static async IAsyncEnumerable<string> Fixed(params string[] items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }

    private static async IAsyncEnumerable<string> Failing(string firstItem, Exception error)
    {
        yield return firstItem;
        await Task.Yield();
        throw error;
    }

    [Fact]
    public async Task A_Finished_Stream_Ends_Completed()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();

        await viewModel.ConsumeAsync(Fixed("a", "b"));

        Assert.Equal(Ex093_StreamState.Completed, viewModel.State);
        Assert.Equal(["a", "b"], viewModel.Items);
    }

    [Fact]
    public async Task Items_Arrive_In_Order()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();

        await viewModel.ConsumeAsync(Fixed("first", "second", "third"));

        Assert.Equal(["first", "second", "third"], viewModel.Items);
    }

    [Fact]
    public async Task A_Failing_Stream_Ends_Failed()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();

        await viewModel.ConsumeAsync(Failing("a", new InvalidOperationException("boom")));

        Assert.Equal(Ex093_StreamState.Failed, viewModel.State);
        Assert.Equal("boom", viewModel.Error);
    }

    [Fact]
    public async Task Items_Received_Before_A_Failure_Are_Kept()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();

        await viewModel.ConsumeAsync(Failing("a", new InvalidOperationException("boom")));

        // A partial result is still a result: throwing the items away on failure is a
        // decision, and rarely the right one.
        Assert.Equal(["a"], viewModel.Items);
    }

    [Fact]
    public async Task The_State_Is_Running_While_Items_Arrive()
    {
        var stream = new ControlledStream();
        var viewModel = new Ex093_AsyncStreamViewModel();
        stream.Push("a");

        var consuming = viewModel.ConsumeAsync(stream.Enumerate());

        Assert.Equal(Ex093_StreamState.Running, viewModel.State);

        stream.Complete();
        await consuming;
    }

    [Fact]
    public async Task Stopping_Ends_The_Consumption()
    {
        var stream = new ControlledStream();
        var viewModel = new Ex093_AsyncStreamViewModel();
        stream.Push("a");
        var consuming = viewModel.ConsumeAsync(stream.Enumerate());

        viewModel.Stop();
        await consuming;

        // An await foreach without a token cannot be stopped at all: the loop ends when
        // the producer says so, and a page nobody is looking at keeps appending.
        Assert.Equal(Ex093_StreamState.Stopped, viewModel.State);
    }

    [Fact]
    public async Task Items_After_A_Stop_Do_Not_Arrive()
    {
        var stream = new ControlledStream();
        var viewModel = new Ex093_AsyncStreamViewModel();
        stream.Push("a");
        var consuming = viewModel.ConsumeAsync(stream.Enumerate());

        viewModel.Stop();
        stream.Push("too late");
        await consuming;

        Assert.Equal(["a"], viewModel.Items);
    }

    [Fact]
    public async Task A_Second_Consumption_Supersedes_The_First()
    {
        var first = new ControlledStream();
        var viewModel = new Ex093_AsyncStreamViewModel();
        first.Push("old");
        var consumingFirst = viewModel.ConsumeAsync(first.Enumerate());

        await viewModel.ConsumeAsync(Fixed("new"));
        await consumingFirst;

        // The items are cleared and the newer run owns the state - the same discipline as
        // ex064, one level up.
        Assert.Equal(["new"], viewModel.Items);
        Assert.Equal(Ex093_StreamState.Completed, viewModel.State);
    }

    [Fact]
    public async Task A_Later_Success_Clears_An_Earlier_Error()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();
        await viewModel.ConsumeAsync(Failing("a", new InvalidOperationException("boom")));

        await viewModel.ConsumeAsync(Fixed("b"));

        Assert.Null(viewModel.Error);
        Assert.Equal(Ex093_StreamState.Completed, viewModel.State);
    }

    [Fact]
    public void Stopping_When_Nothing_Runs_Is_Harmless()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();

        viewModel.Stop();

        Assert.Equal(Ex093_StreamState.Idle, viewModel.State);
    }

    [Fact]
    public async Task The_State_Changes_Are_Announced()
    {
        var viewModel = new Ex093_AsyncStreamViewModel();
        var names = new List<string?>();
        viewModel.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        await viewModel.ConsumeAsync(Fixed("a"));

        Assert.Contains(nameof(Ex093_AsyncStreamViewModel.State), names);
    }
}
