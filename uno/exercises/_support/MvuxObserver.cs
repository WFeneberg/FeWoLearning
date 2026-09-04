using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Support;

/// <summary>
/// Test fixture: collects the first messages a feed produces. Not an exercise.
/// </summary>
/// <remarks>
/// A feed is a stream, so this is the honest way to look at one: subscribe, take the first
/// n messages, and assert on them. Reading a value instead is a convenience whose answer
/// depends on whether a live subscription already exists - see ex065.
/// </remarks>
public static class MvuxObserver
{
    /// <summary>
    /// Subscribes to <paramref name="feed"/> and returns its first <paramref name="count"/>
    /// messages. When <paramref name="whileRunning"/> is given it runs after the
    /// subscription is live, so the changes it causes are part of what is collected.
    /// </summary>
    public static async Task<IReadOnlyList<Message<T>>> Collect<T>(
        IFeed<T> feed,
        int count,
        Func<Task>? whileRunning = null)
        where T : notnull
    {
        var messages = new List<Message<T>>();
        var subscribed = new TaskCompletionSource();

        var reading = Task.Run(async () =>
        {
            await foreach (var message in feed.Messages())
            {
                messages.Add(message);
                subscribed.TrySetResult();

                if (messages.Count >= count)
                {
                    return;
                }
            }
        });

        if (whileRunning is not null)
        {
            // The first message arrives on subscribe, so waiting for it is what makes the
            // ordering deterministic rather than a race.
            await subscribed.Task;
            await whileRunning();
        }

        await Task.WhenAny(reading, Task.Delay(TimeSpan.FromSeconds(5)));
        return messages;
    }

    /// <summary>The data of each message, or null where a message carried none.</summary>
    public static IReadOnlyList<T?> DataOf<T>(IEnumerable<Message<T>> messages)
        where T : class =>
        messages.Select(message => message.Current.Data.IsSome(out var value) ? value : null).ToList();

    /// <summary>The data of each message as a nullable value type.</summary>
    public static IReadOnlyList<T?> ValuesOf<T>(IEnumerable<Message<T>> messages)
        where T : struct =>
        messages.Select(message => message.Current.Data.IsSome(out var value) ? value : (T?)null).ToList();
}
