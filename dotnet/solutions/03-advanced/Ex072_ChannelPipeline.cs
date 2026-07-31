using System.Threading.Channels;

namespace FeWoLearning.Exercises.Advanced;

// Exercise 072 — Channel<T> producer/consumer pipeline (reference solution).
// A bounded channel with capacity < item count forces the producer to await WriteAsync
// whenever the channel is full, and the consumer to await ReadAsync when it is empty.
// Channels preserve FIFO order for a single writer, so draining with ReadAllAsync
// reproduces the exact production order.
public static class ChannelPipeline
{
    public static async Task<List<T>> RunAsync<T>(IEnumerable<T> items, int capacity)
    {
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait,
        });

        var producer = Task.Run(async () =>
        {
            Exception? failure = null;
            try
            {
                foreach (var item in items)
                {
                    await channel.Writer.WriteAsync(item).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                channel.Writer.Complete(failure);
            }
        });

        var results = new List<T>();
        var consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                results.Add(item);
            }
        });

        await Task.WhenAll(producer, consumer).ConfigureAwait(false);
        return results;
    }
}
