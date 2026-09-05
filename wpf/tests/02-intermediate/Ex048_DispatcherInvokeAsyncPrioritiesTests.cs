using System.Windows.Threading;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex048_DispatcherInvokeAsyncPrioritiesTests : WpfTestContext
{
    [WpfFact]
    public async Task Queuing_Happens_Before_Anything_Runs()
    {
        var order = new List<string>();
        var items = new (DispatcherPriority Priority, Action Callback)[]
        {
            (DispatcherPriority.Background, () => order.Add("a")),
            (DispatcherPriority.Send, () => order.Add("b")),
        };

        var all = Ex048_DispatcherPriorityQueue.RunAllAsync(Dispatcher.CurrentDispatcher, items);

        // Nothing has run yet - a bypass that executes callbacks inline instead of actually
        // queuing them through the dispatcher would already show something here.
        Assert.Empty(order);

        await WithTimeout(all);

        Assert.Equal(2, order.Count);
    }

    [WpfFact]
    public async Task Runs_In_Priority_Order_Regardless_Of_The_Order_Queued()
    {
        var order = new List<string>();
        var items = new (DispatcherPriority Priority, Action Callback)[]
        {
            (DispatcherPriority.Normal, () => order.Add("normal")),
            (DispatcherPriority.Background, () => order.Add("background")),
            (DispatcherPriority.Send, () => order.Add("send")),
            (DispatcherPriority.Input, () => order.Add("input")),
        };

        await WithTimeout(Ex048_DispatcherPriorityQueue.RunAllAsync(Dispatcher.CurrentDispatcher, items));

        // Highest priority first: Send(10) > Normal(9) > Input(5) > Background(4) - not the
        // call order above, which deliberately does not match.
        Assert.Equal(new[] { "send", "normal", "input", "background" }, order);
    }

    [WpfFact]
    public async Task An_Externally_Queued_Marker_Proves_Real_Per_Item_Priority_Not_A_Presorted_Single_Priority()
    {
        var order = new List<string>();
        var items = new (DispatcherPriority Priority, Action Callback)[]
        {
            (DispatcherPriority.Background, () => order.Add("background")),
            (DispatcherPriority.Send, () => order.Add("send")),
        };

        var all = Ex048_DispatcherPriorityQueue.RunAllAsync(Dispatcher.CurrentDispatcher, items);

        // Queued directly against the SAME dispatcher, bypassing RunAllAsync entirely, at a
        // real priority strictly between Background and Send. This is the discriminating
        // check: an implementation that pre-sorts its own items and queues all of them at one
        // uniform priority (instead of each item's own real priority) can still produce the
        // right-looking two-item order above, but cannot place a THIRD, externally-queued
        // operation correctly in the middle - only genuine per-item WPF priority scheduling can.
        var marker = Dispatcher.CurrentDispatcher.InvokeAsync(() => order.Add("marker"), DispatcherPriority.Input);

        await WithTimeout(all);
        await WithTimeout(marker.Task);

        Assert.Equal(new[] { "send", "marker", "background" }, order);
    }

    [WpfFact]
    public async Task Every_Queued_Item_Runs_Exactly_Once_Even_With_Repeated_Priorities()
    {
        var counts = new Dictionary<string, int>();
        void Record(string label) => counts[label] = counts.GetValueOrDefault(label) + 1;

        var items = new (DispatcherPriority Priority, Action Callback)[]
        {
            (DispatcherPriority.Background, () => Record("a")),
            (DispatcherPriority.Background, () => Record("b")),
            (DispatcherPriority.Send, () => Record("c")),
        };

        await WithTimeout(Ex048_DispatcherPriorityQueue.RunAllAsync(Dispatcher.CurrentDispatcher, items));

        Assert.Equal(1, counts["a"]);
        Assert.Equal(1, counts["b"]);
        Assert.Equal(1, counts["c"]);
    }
}
