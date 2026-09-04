// Exercise 089 - Subscription Tokens (advanced).
// Goal:   Make unsubscribing something a caller cannot forget.
// Drills: Subscribe returning IDisposable, idempotent disposal, and a source that really
//         drops its reference to the handler.
// Passes: dotnet test --filter FullyQualifiedName~Ex089_
//
// ex066 solved this by holding subscribers weakly, which is the right answer for a global
// bus. This is the other answer, for everything with a scope: hand back a token, and the
// caller's `using` or Dispose does the rest. It is deterministic where the weak version is
// merely eventual - and it composes, because a page can keep one composite token for
// everything it subscribed to.
//
// The bar is not "the caller can unsubscribe". It is: after disposal the source holds
// nothing, which is what a WeakReference test can actually check.

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A source of string notifications whose subscriptions are handed back as tokens.
/// </summary>
public sealed class Ex089_SubscriptionTokens
{
    private readonly List<Action<string>> _handlers = [];

    /// <summary>How many handlers the source currently holds.</summary>
    public int HandlerCount => _handlers.Count;

    /// <summary>
    /// Subscribes <paramref name="handler"/> and returns the token that ends the
    /// subscription. Disposing it twice is harmless, and disposing it after the source has
    /// dropped the handler is harmless too.
    /// </summary>
    public IDisposable Subscribe(Action<string> handler)
    {
        _handlers.Add(handler);

        // The token closes over the list and this exact delegate instance, and removes by
        // reference. Removing "the first handler equal to this one" would take somebody
        // else's identical lambda with it.
        return new Token(() => _handlers.Remove(handler));
    }

    private sealed class Token(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose()
        {
            // Nulled first, so a second Dispose is a no-op rather than a second removal -
            // which would unsubscribe whoever registered the same delegate later.
            var action = _unsubscribe;
            _unsubscribe = null;
            action?.Invoke();
        }
    }

    /// <summary>Sends <paramref name="message"/> to every current handler.</summary>
    public int Publish(string message)
    {
        // A snapshot: a handler is allowed to unsubscribe itself while being called, and
        // iterating the live list would throw halfway through the publish.
        var snapshot = _handlers.ToArray();

        foreach (var handler in snapshot)
        {
            handler(message);
        }

        return snapshot.Length;
    }

    /// <summary>
    /// A token that owns several others and disposes them together - what a page keeps one
    /// of.
    /// </summary>
    public static IDisposable Combine(params IDisposable[] tokens) =>
        new Token(() =>
        {
            foreach (var token in tokens)
            {
                token.Dispose();
            }
        });
}
