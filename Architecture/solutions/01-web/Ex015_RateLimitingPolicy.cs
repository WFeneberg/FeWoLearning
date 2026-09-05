using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Exercises.Web.Ex015;

// Exercise 015 — RateLimitingPolicy (reference solution).
public sealed class TokenBucketLimiter(IClock clock, int capacity, TimeSpan refillInterval)
{
    private sealed class Bucket(double tokens, DateTimeOffset lastRefill)
    {
        public double Tokens { get; set; } = tokens;
        public DateTimeOffset LastRefill { get; set; } = lastRefill;
    }

    private readonly Dictionary<string, Bucket> _buckets = [];

    public bool TryAcquire(string clientId)
    {
        var now = clock.UtcNow;

        if (!_buckets.TryGetValue(clientId, out var bucket))
        {
            // One bucket per client. A single shared counter is the bug where one noisy
            // caller silently rate-limits everybody else.
            bucket = new Bucket(capacity, now);
            _buckets[clientId] = bucket;
        }
        else
        {
            var elapsed = now - bucket.LastRefill;
            var earned = elapsed.TotalMilliseconds / refillInterval.TotalMilliseconds;

            // Math.Min caps the bucket. Without it an idle client accrues tokens for as
            // long as it stays quiet and can then fire all of them at once - exactly the
            // burst the limiter was installed to prevent.
            bucket.Tokens = Math.Min(capacity, bucket.Tokens + earned);
            bucket.LastRefill = now;
        }

        if (bucket.Tokens < 1)
            return false;

        bucket.Tokens -= 1;
        return true;
    }
}
