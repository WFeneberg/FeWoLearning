using System.Security.Claims;
using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex017_JwtValidationTests
{
    private static readonly byte[] Key = Ex017_TokenFactory.NewKey();
    private const string Issuer = "https://issuer.example";
    private const string Audience = "https://audience.example";
    private const string Subject = "alice";

    [Fact]
    public void Use_A_Valid_In_Date_Token_Validates_And_Carries_The_Subject_Claim()
    {
        var token = Ex017_TokenFactory.CreateValid(Key, Issuer, Audience, Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.True(ok);
        Assert.NotNull(principal);
        Assert.Equal(Subject, principal!.FindFirst("sub")?.Value);
    }

    [Fact]
    public void Attack_A_Token_Signed_With_A_Different_Key_Is_Rejected()
    {
        var token = Ex017_TokenFactory.CreateValid(Ex017_TokenFactory.NewKey(), Issuer, Audience, Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }

    [Fact]
    public void Attack_An_Alg_None_Token_Is_Rejected()
    {
        var token = Ex017_TokenFactory.CreateUnsigned(Issuer, Audience, Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }

    [Fact]
    public void Attack_A_Token_From_A_Different_Issuer_Is_Rejected()
    {
        var token = Ex017_TokenFactory.CreateValid(Key, "https://attacker.example", Audience, Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }

    [Fact]
    public void Attack_A_Token_For_A_Different_Audience_Is_Rejected()
    {
        var token = Ex017_TokenFactory.CreateValid(Key, Issuer, "https://someone-else.example", Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }

    [Fact]
    public void Attack_An_Expired_Token_Is_Rejected()
    {
        var now = DateTime.UtcNow;
        var token = Ex017_TokenFactory.CreateValid(
            Key, Issuer, Audience, Subject, notBefore: now.AddMinutes(-20), expires: now.AddMinutes(-10));

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }

    [Fact]
    public void Attack_A_Token_Whose_Payload_Was_Edited_After_Signing_Is_Rejected()
    {
        var token = Ex017_TokenFactory.CreateWithTamperedPayload(Key, Issuer, Audience, Subject);

        var ok = Ex017_JwtValidation.TryValidate(token, Key, Issuer, Audience, out var principal);

        Assert.False(ok);
        Assert.Null(principal);
    }
}
