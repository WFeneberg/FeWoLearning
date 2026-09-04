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
using System.Linq;
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
    public IListState<string> Items => ListState<string>.Value(this, () => Seed);

    /// <summary>Appends <paramref name="item"/>.</summary>
    public async Task AddAsync(string item, CancellationToken ct) => await Items.AddAsync(item, ct);

    /// <summary>Removes every item equal to <paramref name="item"/>.</summary>
    public async Task RemoveAsync(string item, CancellationToken ct) =>
        await Items.RemoveAllAsync(candidate => candidate == item, ct);

    /// <summary>Upper-cases every item.</summary>
    public async Task ShoutAsync(CancellationToken ct) =>
        // One update producing the next list. The per-item UpdateAllAsync overload matches
        // and replaces item by item, which is the wrong tool here: the current list is
        // immutable, so a subscriber still holding it is unaffected either way, but only
        // this shape maps everything in a single change.
        await Items.UpdateAsync(
            items => items.Select(item => item.ToUpperInvariant()).ToImmutableList(),
            ct);

    /// <summary>
    /// A command that adds a fixed item and records that it ran. Created once, so a bound
    /// button keeps the same command instance.
    /// </summary>
    // Command.Async keys the command by owner too, so a bound button keeps watching the
    // same instance rather than a fresh one per read.
    public IAsyncCommand AddFixedItemCommand => Command.Async(async ct =>
    {
        _added.Add("from the command");
        await AddAsync("from the command", ct);
    });
}
