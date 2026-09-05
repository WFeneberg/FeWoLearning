using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FeWoLearning.Security.Tests.WebAspNet;

// Test-side helper for Ex017 (and reused wherever a later exercise needs a real
// JWT rather than a hand-edited string): mints tokens with the actual
// Microsoft.IdentityModel.JsonWebTokens handler, so every attack token differs
// from a valid one in exactly the one property under test - never in the
// base64/JSON well-formedness the handler itself already guarantees.
public static class Ex017_TokenFactory
{
    public static byte[] NewKey() => RandomNumberGenerator.GetBytes(32);

    public static string CreateValid(
        byte[] signingKey,
        string issuer,
        string audience,
        string subject,
        DateTime? notBefore = null,
        DateTime? expires = null)
    {
        var now = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            NotBefore = notBefore ?? now.AddMinutes(-1),
            IssuedAt = now,
            Expires = expires ?? now.AddMinutes(5),
            Claims = new Dictionary<string, object> { ["sub"] = subject },
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256),
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    // An unsigned JWS - the real "alg: none" attack, minted by the handler's own
    // unsigned-token overload rather than by splicing a fake header onto a payload.
    public static string CreateUnsigned(string issuer, string audience, string subject)
    {
        var now = DateTimeOffset.UtcNow;
        var payload = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["iss"] = issuer,
            ["aud"] = audience,
            ["sub"] = subject,
            ["nbf"] = now.AddMinutes(-1).ToUnixTimeSeconds(),
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = now.AddMinutes(5).ToUnixTimeSeconds(),
        });

        return new JsonWebTokenHandler().CreateToken(payload);
    }

    // Mints a genuinely valid token, then edits the payload segment only,
    // leaving the original header and signature untouched - so the signature
    // no longer matches. This is the "payload edited after signing" attack;
    // the base token still comes from the real handler, only the tamper step
    // is a string edit.
    public static string CreateWithTamperedPayload(byte[] signingKey, string issuer, string audience, string subject)
    {
        var valid = CreateValid(signingKey, issuer, audience, subject);
        var parts = valid.Split('.');

        var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
            Encoding.UTF8.GetString(Base64UrlDecode(parts[1])))!;

        var mutated = new Dictionary<string, object?>();
        foreach (var (name, element) in claims)
        {
            mutated[name] = name == "sub" ? subject + "-tampered" : element;
        }

        parts[1] = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(mutated));
        return string.Join('.', parts);
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded += (padded.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => string.Empty,
        };
        return Convert.FromBase64String(padded);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
}
