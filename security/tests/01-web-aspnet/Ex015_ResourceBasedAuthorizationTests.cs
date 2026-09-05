using System.Security.Claims;
using FeWoLearning.Security.Exercises.WebAspNet;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex015_ResourceBasedAuthorizationTests
{
    private static readonly Ex015_Document Document = new(1, "alice", "confidential body");
    private static readonly ClaimsPrincipal Anonymous = new();

    private static Task<WebHarness> StartAsync() =>
        WebHarness.StartAsync(
            services: Ex015_ResourceBasedAuthorization.AddServices,
            configure: app => app.Run(_ => Task.CompletedTask),
            ct: TestContext.Current.CancellationToken);

    private static ClaimsPrincipal User(string userId, bool admin = false)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        if (admin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    [Fact]
    public async Task Attack_A_Non_Owner_Is_Denied_Read_And_Delete()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();
        var mallory = User("mallory");

        var readResult = await authz.AuthorizeAsync(mallory, Document, Ex015_ResourceBasedAuthorization.PolicyName);
        var deleteResult = await authz.AuthorizeAsync(mallory, Document, Ex015_ResourceBasedAuthorization.DeletePolicyName);

        Assert.False(readResult.Succeeded);
        Assert.False(deleteResult.Succeeded);
    }

    [Fact]
    public async Task Attack_An_Anonymous_Principal_Is_Denied()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();

        var result = await authz.AuthorizeAsync(Anonymous, Document, Ex015_ResourceBasedAuthorization.PolicyName);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Use_The_Owner_Is_Allowed_To_Read_And_To_Delete()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();
        var owner = User("alice");

        var readResult = await authz.AuthorizeAsync(owner, Document, Ex015_ResourceBasedAuthorization.PolicyName);
        var deleteResult = await authz.AuthorizeAsync(owner, Document, Ex015_ResourceBasedAuthorization.DeletePolicyName);

        Assert.True(readResult.Succeeded);
        Assert.True(deleteResult.Succeeded);
    }

    [Fact]
    public async Task Use_An_Admin_Is_Allowed_To_Read_But_Still_Denied_Delete()
    {
        await using var harness = await StartAsync();
        var authz = harness.Services.GetRequiredService<IAuthorizationService>();
        var admin = User("carol", admin: true);

        var readResult = await authz.AuthorizeAsync(admin, Document, Ex015_ResourceBasedAuthorization.PolicyName);
        var deleteResult = await authz.AuthorizeAsync(admin, Document, Ex015_ResourceBasedAuthorization.DeletePolicyName);

        Assert.True(readResult.Succeeded);
        Assert.False(deleteResult.Succeeded);
    }
}
