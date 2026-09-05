using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex018_RefreshTokenRotationTests
{
    [Fact]
    public void Attack_Redeeming_The_Same_Token_Twice_Fails_The_Second_Time()
    {
        var store = new Ex018_RefreshTokenStore();
        var token = store.Issue("alice");

        var first = store.TryRedeem(token, out _);
        var second = store.TryRedeem(token, out var secondReplacement);

        Assert.True(first);
        Assert.False(second);
        Assert.Null(secondReplacement);
    }

    [Fact]
    public void Attack_After_A_Reuse_Attempt_The_Replacement_Is_Also_Refused()
    {
        var store = new Ex018_RefreshTokenStore();
        var token = store.Issue("alice");
        store.TryRedeem(token, out var replacement); // legitimate first use

        store.TryRedeem(token, out _); // attacker replays the original token

        var replacementRedeemed = store.TryRedeem(replacement!, out var next);

        Assert.False(replacementRedeemed);
        Assert.Null(next);
    }

    [Fact]
    public void Attack_A_Token_Never_Issued_Is_Refused()
    {
        var store = new Ex018_RefreshTokenStore();

        var ok = store.TryRedeem("never-issued-token", out var replacement);

        Assert.False(ok);
        Assert.Null(replacement);
    }

    [Fact]
    public void Use_A_Freshly_Issued_Token_Redeems_Once_And_Yields_A_Different_Replacement()
    {
        var store = new Ex018_RefreshTokenStore();
        var token = store.Issue("alice");

        var ok = store.TryRedeem(token, out var replacement);

        Assert.True(ok);
        Assert.NotNull(replacement);
        Assert.NotEqual(token, replacement);
    }

    [Fact]
    public void Use_The_Replacement_Itself_Redeems_Once()
    {
        var store = new Ex018_RefreshTokenStore();
        var token = store.Issue("alice");
        store.TryRedeem(token, out var replacement);

        var ok = store.TryRedeem(replacement!, out var next);

        Assert.True(ok);
        Assert.NotNull(next);
        Assert.NotEqual(replacement, next);
    }

    [Fact]
    public void Use_A_Second_Users_Tokens_Are_Unaffected_By_The_First_Users_Revocation()
    {
        var store = new Ex018_RefreshTokenStore();
        var aliceToken = store.Issue("alice");
        var bobToken = store.Issue("bob");

        store.TryRedeem(aliceToken, out _);
        store.TryRedeem(aliceToken, out _); // reuse - revokes alice's family only

        var bobResult = store.TryRedeem(bobToken, out var bobReplacement);

        Assert.True(bobResult);
        Assert.NotNull(bobReplacement);
    }
}
