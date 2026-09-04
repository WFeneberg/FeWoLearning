// Exercise 064 - MVUX Feed Basics (intermediate).
// Goal:   Read a feed the way it is meant to be read: as a stream of messages with three
//         independent axes.
// Drills: Feed.Async, Message<T>.Current, MessageEntry<T>.Data/Error/IsTransient, and
//         Option<T>'s three states - Some, None and Undefined.
// Passes: dotnet test --filter FullyQualifiedName~Ex064_
//
// MVUX is Uno's own answer to "load something and show loading, then data or an error".
// The reason it is worth learning rather than hand-rolling (which is what a view model with
// a State enum does) is that the three concerns are separate axes of one message: a feed
// can carry data *and* be refreshing, or carry stale data *and* an error.
//
// Option<T> is the part that catches people. Some means there is a value. None means there
// deliberably is not one - a filter excluded it, a query found nothing. Undefined means no
// answer yet, or an error instead of one. Collapsing None and Undefined into "null" loses
// the difference between "no results" and "not loaded".

using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>The three axes of one message, flattened for a test to assert on.</summary>
/// <param name="Data">The value, when the message carried one.</param>
/// <param name="HasData">Whether <paramref name="Data"/> means anything.</param>
/// <param name="IsEmpty">Whether the message deliberately carried no value.</param>
/// <param name="IsLoading">Whether the message was transient - work in flight.</param>
/// <param name="Error">The failure message, or null.</param>
public sealed record Ex064_Snapshot(int Data, bool HasData, bool IsEmpty, bool IsLoading, string? Error);

public static class Ex064_MvuxFeedBasics
{
    /// <summary>A feed that awaits <paramref name="load"/> for its value.</summary>
    // Feed.Async takes an AsyncFunc<T> - a ValueTask - so a Task-returning loader is
    // adapted rather than passed straight through.
    public static IFeed<int> Create(Func<CancellationToken, Task<int>> load) =>
        Feed.Async(async ct => await load(ct));

    /// <summary>
    /// Flattens <paramref name="message"/> into a snapshot: the data if there is any,
    /// whether the absence was deliberate, whether work is in flight, and the error.
    /// </summary>
    public static Ex064_Snapshot Describe(Message<int> message)
    {
        // One entry, read once: Current is the message's current state, Previous is what it
        // replaced, and Changes says which axes moved.
        var entry = message.Current;

        return new Ex064_Snapshot(
            Data: entry.Data.IsSome(out var value) ? value : 0,
            HasData: entry.Data.IsSome(out _),

            // None is a deliberate absence; Undefined is "no answer yet, or an error
            // instead of one". Treating both as null loses the difference between "no
            // results" and "not loaded".
            IsEmpty: entry.Data.Type == OptionType.None,
            IsLoading: entry.IsTransient,
            Error: entry.Error?.Message);
    }

    /// <summary>
    /// The feed's current value, or null when it has none.
    /// </summary>
    /// <remarks>
    /// Read outside any subscription context, so the answer is the current one - ex065 is
    /// about what happens when it is not.
    /// </remarks>
    public static async ValueTask<int?> CurrentValue(IFeed<int> feed, CancellationToken ct)
    {
        // Option rather than Value: it answers "there is no value" without inventing a
        // default, and an error still surfaces as an exception to the caller.
        var option = await feed.Option(ct);

        return option.IsSome(out var value) ? value : null;
    }
}
