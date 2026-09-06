using System.Security.Cryptography;
using System.Text;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Domain.Ex089;

/// <summary>What the receiver gets: a body and the headers that let it trust the body.</summary>
public sealed record SignedDelivery(string Body, string Signature, long TimestampSeconds, string EventId);

public sealed class WebhookRejectedException(string reason) : Exception(reason);

// Exercise 089 — WebhookDelivery (domain).
// Goal:   Push an event to somebody else's server so that they can tell it really came
//         from you, and so that a replay of it is worthless.
// Drills: HMAC signing, timestamp in the signed payload, replay windows, receiver dedup.
// Passes: signing   - the signature is an HMAC over the TIMESTAMP AND the body, so neither
//                     can be changed without invalidating it.
//         verifying - a correct signature verifies; a tampered body, a swapped timestamp
//                     and a wrong secret all fail.
//         THE FIRST  - a delivery older than the tolerance is REFUSED even with a perfect
//                     signature. A signature proves who sent it, never when - and without
//                     a window, anybody who ever captured one request can send it again
//                     for ever.
//         THE SECOND - the same EventId delivered twice is refused by the receiver. The
//                     sender retries on any failure it cannot distinguish from a timeout,
//                     so at-least-once is the delivery guarantee whether anybody chose it
//                     or not.
//         comparison- the signature check is fixed-time.
//
// Everything here is the receiver's problem, and that is why the sender has to make it
// solvable. The signature has to cover the timestamp, or the replay window is unenforceable
// - an attacker just sends an old body with a fresh header. The event id has to be stable
// across retries, or dedup is impossible. Neither can be added later by the receiver.
public static class Ex089_WebhookDelivery
{
    /// <summary>Sign a body for sending. The signature covers "{timestamp}.{body}".</summary>
    public static SignedDelivery Sign(IClock clock, string secret, string eventId, string body) =>
        throw new NotImplementedException(
            "TODO: Ex089 - HMAC-SHA256 over \"{unix seconds}.{body}\" with the secret, hex lower-case");

    /// <summary>
    /// Verify one delivery: the signature, then the age. Throws
    /// <see cref="WebhookRejectedException"/> saying which check failed.
    /// </summary>
    public static void Verify(IClock clock, string secret, SignedDelivery delivery, TimeSpan tolerance) =>
        throw new NotImplementedException(
            "TODO: Ex089 - recompute the signature and compare in fixed time, then refuse anything older than the tolerance");

    /// <summary>Shared helper: lower-case hex HMAC-SHA256.</summary>
    public static string ComputeHmac(string secret, string payload) =>
        Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(payload)));
}

/// <summary>The receiving end: verifies, then refuses anything it has already handled.</summary>
public sealed class WebhookReceiver(IClock clock, string secret, TimeSpan tolerance)
{
    private readonly HashSet<string> _handled = new(StringComparer.Ordinal);

    public int Handled { get; private set; }

    /// <summary>Returns whether this delivery did any work.</summary>
    public bool Receive(SignedDelivery delivery) =>
        throw new NotImplementedException(
            "TODO: Ex089 - verify first, then handle the event only if this EventId has not been seen before");
}
