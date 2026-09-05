using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex042_CryptographicRandomnessTests
{
    private const int ByteCount = 16;

    [Fact]
    public void Attack_1000_Tokens_Contain_No_Duplicate()
    {
        var seen = new HashSet<string>();

        for (var i = 0; i < 1000; i++)
        {
            var token = Ex042_CryptographicRandomness.NewToken(ByteCount);
            Assert.True(seen.Add(token), $"token #{i} duplicates an earlier one");
        }
    }

    [Fact]
    public void Attack_No_Token_Is_Reproducible_By_Any_Seeded_System_Random()
    {
        // What every new Random(seed)-driven token generator would have
        // produced, for seed 0 through 999. If NewToken were built on
        // System.Random instead of a CSPRNG, a token seeded from anywhere in
        // this common range would show up here byte-for-byte.
        var seededOutputs = new List<byte[]>(1000);
        for (var seed = 0; seed < 1000; seed++)
        {
            var buffer = new byte[ByteCount];
            new Random(seed).NextBytes(buffer);
            seededOutputs.Add(buffer);
        }

        for (var i = 0; i < 1000; i++)
        {
            var decoded = DecodeToken(Ex042_CryptographicRandomness.NewToken(ByteCount));
            Assert.DoesNotContain(seededOutputs, candidate => candidate.SequenceEqual(decoded));
        }
    }

    [Fact]
    public void Attack_NewToken_Zero_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Ex042_CryptographicRandomness.NewToken(0));
    }

    [Fact]
    public void Use_NewToken_32_Decodes_To_Exactly_32_Bytes()
    {
        var token = Ex042_CryptographicRandomness.NewToken(32);

        Assert.Equal(32, DecodeToken(token).Length);
    }

    [Fact]
    public void Use_Encoding_Is_Url_Safe()
    {
        for (var i = 0; i < 100; i++)
        {
            var token = Ex042_CryptographicRandomness.NewToken(ByteCount);

            Assert.DoesNotContain('+', token);
            Assert.DoesNotContain('/', token);
            Assert.DoesNotContain('=', token);
        }
    }

    // Reverses the unpadded, URL-safe base64 the use facts above pin down as
    // the required output shape. Independent of the production encoder: it
    // only assumes the contract the use facts already enforce.
    private static byte[] DecodeToken(string token)
    {
        var standard = token.Replace('-', '+').Replace('_', '/');
        var padded = (standard.Length % 4) switch
        {
            2 => standard + "==",
            3 => standard + "=",
            _ => standard,
        };
        return Convert.FromBase64String(padded);
    }
}
