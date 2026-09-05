using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex041_FixedTimeComparisonTests
{
    private const string Expected = "Sup3rSecretToken1234567890ABCDEF"; // 32 characters

    [Fact]
    public void Attack_A_Presented_Value_That_Is_A_Prefix_Of_Expected_Does_Not_Match()
    {
        var presented = Expected[..(Expected.Length - 1)];

        Assert.False(Ex041_FixedTimeComparison.TokensMatch(presented, Expected));
    }

    [Fact]
    public void Attack_Sharing_The_First_31_Of_32_Characters_Does_Not_Match()
    {
        var presented = Expected[..31] + "!"; // same length, only the last character differs

        Assert.False(Ex041_FixedTimeComparison.TokensMatch(presented, Expected));
    }

    [Fact]
    public void Use_Identical_Tokens_Match()
    {
        Assert.True(Ex041_FixedTimeComparison.TokensMatch(Expected, Expected));
    }

    [Fact]
    public void Use_Comparison_Is_Ordinal_So_Case_Differs_Do_Not_Match()
    {
        Assert.False(Ex041_FixedTimeComparison.TokensMatch(Expected.ToLowerInvariant(), Expected));
    }

    [Fact]
    public void Use_Empty_Presented_Against_Empty_Expected_Matches()
    {
        Assert.True(Ex041_FixedTimeComparison.TokensMatch(string.Empty, string.Empty));
    }
}
