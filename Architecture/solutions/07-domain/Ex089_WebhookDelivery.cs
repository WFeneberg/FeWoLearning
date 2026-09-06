using System.Security.Cryptography;
using System.Text;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Domain.Ex089;

/// <summary>What the receiver gets: a body and the headers that let it trust the body.</summary>
public sealed record SignedDelivery(string Body, string Signature, long TimestampSeconds, string EventId);

public sealed class WebhookRejectedException(string reason) : Exception(reason);

// Exercise 089 — WebhookDelivery (reference solution).
public static class Ex089_WebhookDelivery
{
    public static SignedDelivery Sign(IClock clock, string secret, string eventId, string body)
    {
        var timestamp = clock.UtcNow.ToUnixTimeSeconds();

        // The timestamp is INSIDE the signed payload. Signing the body alone leaves the
        // replay window unenforceable: an attacker sends the old body with a fresh
        // timestamp header, and the signature still checks out.
        return new SignedDelivery(body, ComputeHmac(secret, $"{timestamp}.{body}"), timestamp, eventId);
    }

    public static void Verify(IClock clock, string secret, SignedDelivery delivery, TimeSpan tolerance)
    {
        var expected = ComputeHmac(secret, $"{delivery.TimestampSeconds}.{delivery.Body}");

        // Fixed-time comparison. A string equality check leaks how many leading characters
        // matched, which is enough to reconstruct a signature one byte at a time.
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(delivery.Signature)))
            throw new WebhookRejectedException("Signature does not match.");

        // A signature proves WHO sent it and never WHEN. Without this window, anybody who
        // ever captured one request can send it again for ever, and it stays valid.
        var age = clock.UtcNow - DateTimeOffset.FromUnixTimeSeconds(delivery.TimestampSeconds);

        if (age > tolerance || age < -tolerance)
            throw new WebhookRejectedException($"Delivery is {age} old, outside the {tolerance} tolerance.");
    }

    public static string ComputeHmac(string secret, string payload) =>
        Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(payload)));
}

public sealed class WebhookReceiver(IClock clock, string secret, TimeSpan tolerance)
{
    private readonly HashSet<string> _handled = new(StringComparer.Ordinal);

    public int Handled { get; private set; }

    public bool Receive(SignedDelivery delivery)
    {
        // Verified BEFORE anything is recorded. Adding the id to the dedup set first would
        // let an unsigned request permanently suppress the genuine delivery that follows.
        Ex089_WebhookDelivery.Verify(clock, secret, delivery, tolerance);

        // The sender retries on anything it cannot distinguish from a timeout, so
        // at-least-once is the guarantee whether anybody chose it or not.
        if (!_handled.Add(delivery.EventId))
            return false;

        Handled++;
        return true;
    }
}
