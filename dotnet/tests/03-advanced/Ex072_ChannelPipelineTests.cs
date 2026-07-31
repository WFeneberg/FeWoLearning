using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex072_ChannelPipelineTests
{
    [Fact]
    public async Task ConsumesAllItemsInProductionOrder()
    {
        var items = Enumerable.Range(0, 20).ToList();

        // Capacity is deliberately much smaller than the item count so the producer
        // must block on backpressure and the pipeline must still preserve order.
        var result = await ChannelPipeline.RunAsync(items, capacity: 3);

        Assert.Equal(items, result);
        Assert.Equal(20, result.Count);
    }

    [Fact]
    public async Task PreservesOrderForStrings()
    {
        var items = new List<string> { "a", "b", "c", "d", "e", "f", "g" };

        var result = await ChannelPipeline.RunAsync(items, capacity: 1);

        Assert.Equal(items, result);
    }

    [Fact]
    public async Task HandlesEmptyInput()
    {
        var result = await ChannelPipeline.RunAsync(Enumerable.Empty<int>(), capacity: 4);

        Assert.Empty(result);
    }

    [Fact]
    public async Task HandlesCapacityLargerThanItemCount()
    {
        var items = new[] { 10, 20, 30 };

        var result = await ChannelPipeline.RunAsync(items, capacity: 100);

        Assert.Equal(items, result);
    }
}
