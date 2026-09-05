using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex022_OpenRedirectGuardTests
{
    private const string Fallback = "/home";

    [Theory]
    [InlineData("https://evil.example/")]
    [InlineData("//evil.example/")]
    [InlineData("/\\evil.example")]
    [InlineData("http:/\\/\\evil.example")]
    [InlineData("javascript:alert(1)")]
    [InlineData(null)]
    public void Attack_A_Foreign_Or_Scheme_Carrying_Candidate_Falls_Back(string? candidate)
    {
        Assert.Equal(Fallback, Ex022_OpenRedirectGuard.SafeReturnUrl(candidate, Fallback));
    }

    [Theory]
    [InlineData("/dashboard")]
    [InlineData("/reports?year=2026")]
    public void Use_A_Local_Path_Is_Returned_Unchanged(string candidate)
    {
        Assert.Equal(candidate, Ex022_OpenRedirectGuard.SafeReturnUrl(candidate, Fallback));
    }
}
