using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 018 — RefreshTokenRotation (reference solution).
public sealed class Ex018_RefreshTokenStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, TokenRecord> _tokens = new(StringComparer.Ordinal);
    private readonly HashSet<Guid> _revokedFamilies = new();

    public string Issue(string userId)
    {
        lock (_lock)
        {
            var familyId = Guid.NewGuid();
            var token = NewTokenValue();
            _tokens[token] = new TokenRecord(familyId, userId, Redeemed: false);
            return token;
        }
    }

    public bool TryRedeem(string refreshToken, out string? replacement)
    {
        lock (_lock)
        {
            replacement = null;

            if (!_tokens.TryGetValue(refreshToken, out var record))
            {
                // Never issued - nothing to redeem and nothing to revoke.
                return false;
            }

            if (_revokedFamilies.Contains(record.FamilyId))
            {
                // The family this token belongs to was already revoked, either
                // by an earlier reuse attempt against this same token or one
                // of its ancestors/descendants in the rotation chain.
                return false;
            }

            if (record.Redeemed)
            {
                // Someone is presenting a token that already served its one
                // redemption - the legitimate holder must already be sitting
                // on its replacement, so this token has been stolen. Revoke
                // the entire family: the replacement chain stops here too.
                _revokedFamilies.Add(record.FamilyId);
                return false;
            }

            _tokens[refreshToken] = record with { Redeemed = true };

            var next = NewTokenValue();
            _tokens[next] = new TokenRecord(record.FamilyId, record.UserId, Redeemed: false);
            replacement = next;
            return true;
        }
    }

    private static string NewTokenValue() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    private sealed record TokenRecord(Guid FamilyId, string UserId, bool Redeemed);
}
