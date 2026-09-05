using System.Security.Cryptography;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex037_DpapiProtectedDataTests
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Ex037-entropy");
    private static readonly byte[] OtherEntropy = Encoding.UTF8.GetBytes("Ex037-other-entropy");

    [Fact]
    public void Attack_Protected_Bytes_Never_Contain_The_Plaintext()
    {
        var plaintext = Encoding.UTF8.GetBytes("a-distinctive-plaintext-marker-9f3c1a7e");

        var protectedBytes = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);

        Assert.False(ContainsSubsequence(protectedBytes, plaintext));
    }

    [Fact]
    public void Attack_Unprotect_With_Wrong_Entropy_Throws()
    {
        var plaintext = Encoding.UTF8.GetBytes("secret payload");
        var protectedBytes = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);

        Assert.Throws<CryptographicException>(() => Ex037_DpapiProtectedData.Unprotect(protectedBytes, OtherEntropy));
    }

    [Fact]
    public void Attack_Protecting_The_Same_Plaintext_Twice_Yields_Different_Bytes()
    {
        var plaintext = Encoding.UTF8.GetBytes("same plaintext, same entropy, every time");

        var first = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);
        var second = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);

        Assert.False(first.SequenceEqual(second));
    }

    [Theory]
    [InlineData("")]
    [InlineData("x")]
    [InlineData("round trip me")]
    [InlineData("unicode: héllo wörld 漢字")]
    public void Use_Round_Trip_Reproduces_The_Plaintext(string text)
    {
        var plaintext = Encoding.UTF8.GetBytes(text);

        var protectedBytes = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);
        var recovered = Ex037_DpapiProtectedData.Unprotect(protectedBytes, Entropy);

        Assert.Equal(plaintext, recovered);
    }

    [Fact]
    public void Use_A_1MB_Payload_Round_Trips()
    {
        var plaintext = new byte[1024 * 1024];
        RandomNumberGenerator.Fill(plaintext);

        var protectedBytes = Ex037_DpapiProtectedData.Protect(plaintext, Entropy);
        var recovered = Ex037_DpapiProtectedData.Unprotect(protectedBytes, Entropy);

        Assert.Equal(plaintext, recovered);
    }

    private static bool ContainsSubsequence(byte[] haystack, byte[] needle)
    {
        if (needle.Length == 0) return true;

        for (var i = 0; i <= haystack.Length - needle.Length; i++)
        {
            var match = true;
            for (var j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j])
                {
                    match = false;
                    break;
                }
            }

            if (match) return true;
        }

        return false;
    }
}
