// Exercise 092 - MVUX List State And Commands (expert).
// Goal:   Hold a collection as MVUX state, and drive it from a command.
// Drills: ListState<T> over an immutable list, AddAsync/RemoveAllAsync/UpdateAllAsync, and
//         Command.Async as the thing a button binds to.
// Passes: dotnet test --filter FullyQualifiedName~Ex092_
//
// A ListState is a State whose value is an IImmutableList, plus mutation helpers that
// produce the next list rather than editing the current one. That immutability is what
// makes the change notifications trustworthy: a subscriber holding a previous value still
// holds exactly what it was given.
//
// Command.Async is the other half. It needs a dispatcher, and in this track the
// Uno.Extensions.Reactive.WinUI package supplies it - without that reference Command.Async
// throws with a message naming precisely that, which is a good error and still surprising
// the first time.

using System.Collections.Immutable;
using Uno.Extensions.Reactive;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>
/// A to-do list held as MVUX state, with a command that adds to it.
/// </summary>
public sealed class Ex092_TodoList
{
    private readonly List<string> _added = [];

    /// <summary>
    /// The items, seeded with <paramref name="seed"/> the first time the state is asked for.
    /// </summary>
    public Ex092_TodoList(params string[] seed) => Seed = [.. seed];

    /// <summary>What the list starts as.</summary>
    public IImmutableList<string> Seed { get; }

    /// <summary>Every item the command has been asked to add, in order.</summary>
    public IReadOnlyList<string> Added => _added;

    /// <summary>The list state. The same instance every time.</summary>
    public IListState<string> Items =>
        // TODO: ListState<string>.Value takes the owner and a factory for the seed.
        throw new NotImplementedException("TODO: Ex092 - expose the list state");

    /// <summary>Appends <paramref name="item"/>.</summary>
    public async Task AddAsync(string item, CancellationToken ct) =>
        throw new NotImplementedException("TODO: Ex092 - add an item to the list state");

    /// <summary>Removes every item equal to <paramref name="item"/>.</summary>
    public async Task RemoveAsync(string item, CancellationToken ct) =>
        throw new NotImplementedException("TODO: Ex092 - remove matching items");

    /// <summary>Upper-cases every item.</summary>
    public async Task ShoutAsync(CancellationToken ct) =>
        // TODO: UpdateAsync's whole-list overload takes the current list and returns the
        // next one, which is the shape that maps every item in one change. Mutating the
        // list in place would not be an update at all - the value is immutable on purpose.
        throw new NotImplementedException("TODO: Ex092 - map every item");

    /// <summary>
    /// A command that adds a fixed item and records that it ran. Created once, so a bound
    /// button keeps the same command instance.
    /// </summary>
    public IAsyncCommand AddFixedItemCommand =>
        // TODO: Command.Async takes an async action. Record the item in _added and add it
        // to the state.
        throw new NotImplementedException("TODO: Ex092 - build the command");
}
