using System.Globalization;
using System.Security.Claims;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex014_AuthorizationPoliciesTests
{
    private static Task<WebHarness> StartAsync() =>
        WebHarness.StartAsync(
            services: Ex014_AuthorizationPolicies.AddServices,
            configure: app => app.Run(_ => Task.CompletedTask),
            ct: TestContext.Current.CancellationToken);

    private static ClaimsPrincipal PrincipalWithDateOfBirth(string? dateOfBirth)
    {
        var claims = dateOfBirth is null
            ? Array.Empty<Claim>()
            : new[] { new Claim("dateOfBirth", dateOfBirth) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static string AgedYears(int years) =>
        DateTime.UtcNow.AddYears(-years).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    [Fact]
    public async Task Attack_No_DateOfBirth_Claim_Fails_The_Policy()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();

        var result = await authz.AuthorizeAsync(PrincipalWithDateOfBirth(null), Ex014_AuthorizationPolicies.PolicyName);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Attack_A_Seventeen_Year_Old_Fails_The_Policy()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();

        var result = await authz.AuthorizeAsync(
            PrincipalWithDateOfBirth(AgedYears(17)), Ex014_AuthorizationPolicies.PolicyName);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Attack_A_Malformed_DateOfBirth_Fails_The_Policy_Instead_Of_Throwing()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();

        var result = await authz.AuthorizeAsync(
            PrincipalWithDateOfBirth("not-a-date"), Ex014_AuthorizationPolicies.PolicyName);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Use_An_Eighteen_Year_Old_Passes_The_Policy()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();
        var dateOfBirth = DateTime.UtcNow.AddYears(-18).AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var result = await authz.AuthorizeAsync(
            PrincipalWithDateOfBirth(dateOfBirth), Ex014_AuthorizationPolicies.PolicyName);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Use_A_Forty_Year_Old_Passes_The_Policy()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();

        var result = await authz.AuthorizeAsync(
            PrincipalWithDateOfBirth(AgedYears(40)), Ex014_AuthorizationPolicies.PolicyName);

        Assert.True(result.Succeeded);
    }
}
