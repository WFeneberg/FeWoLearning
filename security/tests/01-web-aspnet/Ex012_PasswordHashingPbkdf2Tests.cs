using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex012_PasswordHashingPbkdf2Tests
{
    private const string Password = "correct horse battery staple";

    [Fact]
    public void Attack_Hashing_The_Same_Password_Twice_Yields_Different_Stored_Values()
    {
        var first = Ex012_PasswordHashingPbkdf2.Hash(Password);
        var second = Ex012_PasswordHashingPbkdf2.Hash(Password);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Attack_Verify_Rejects_A_Wrong_Password()
    {
        var stored = Ex012_PasswordHashingPbkdf2.Hash(Password);

        Assert.False(Ex012_PasswordHashingPbkdf2.Verify("wrong password", stored));
    }

    [Fact]
    public void Attack_Verify_Rejects_A_Stored_Value_Whose_Salt_Was_Altered_By_One_Byte()
    {
        var stored = Ex012_PasswordHashingPbkdf2.Hash(Password);
        var parts = stored.Split(':');
        var salt = Convert.FromBase64String(parts[1]);
        salt[0] ^= 0xFF;
        var tampered = string.Join(':', parts[0], Convert.ToBase64String(salt), parts[2]);

        Assert.False(Ex012_PasswordHashingPbkdf2.Verify(Password, tampered));
    }

    [Fact]
    public void Attack_The_Stored_Value_Never_Contains_The_Password_As_A_Substring()
    {
        var stored = Ex012_PasswordHashingPbkdf2.Hash(Password);

        Assert.DoesNotContain(Password, stored, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("correct horse battery staple")]
    [InlineData("pässwörd❤")]
    [InlineData("")]
    public void Use_Verify_Round_Trips_Its_Own_Hash(string password)
    {
        var stored = Ex012_PasswordHashingPbkdf2.Hash(password);

        Assert.True(Ex012_PasswordHashingPbkdf2.Verify(password, stored));
    }

    [Fact]
    public void Use_The_Stored_Iteration_Count_Read_Back_Is_At_Least_100000()
    {
        var stored = Ex012_PasswordHashingPbkdf2.Hash(Password);

        var iterations = int.Parse(stored.Split(':')[0]);

        Assert.True(iterations >= 100_000);
    }
}
