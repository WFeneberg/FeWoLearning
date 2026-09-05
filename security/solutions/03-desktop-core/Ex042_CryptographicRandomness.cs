using System.Security.Cryptography;

namespace FeWoLearning.Security.Exercises.DesktopCore;

// Exercise 042 — CryptographicRandomness (reference solution).
public static class Ex042_CryptographicRandomness
{
    public static string NewToken(int byteCount)
    {
        if (byteCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(byteCount), byteCount, "byteCount must be positive.");

        var bytes = RandomNumberGenerator.GetBytes(byteCount);

        // Unpadded, URL-safe base64: '+' -> '-', '/' -> '_', trailing '=' dropped.
        // The padding is recoverable from the string's length alone, so nothing
        // is lost by omitting it.
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
