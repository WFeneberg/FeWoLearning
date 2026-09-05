using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 015 — ResourceBasedAuthorization (reference solution).
public sealed record Ex015_Document(int Id, string OwnerId, string Body);

public static class Ex015_ResourceBasedAuthorization
{
    public const string PolicyName = "DocumentOwner";
    public const string DeletePolicyName = "DocumentOwnerDelete";

    public static void AddServices(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Read: the owner, or anyone holding the admin role.
            options.AddPolicy(PolicyName, policy => policy.RequireAssertion(ctx =>
                ctx.Resource is Ex015_Document document &&
                (IsOwner(ctx.User, document) || ctx.User.IsInRole("admin"))));

            // Delete: the owner only - admin is deliberately NOT a bypass here,
            // so a blanket "admins can do anything" handler would fail this half.
            options.AddPolicy(DeletePolicyName, policy => policy.RequireAssertion(ctx =>
                ctx.Resource is Ex015_Document document && IsOwner(ctx.User, document)));
        });
    }

    private static bool IsOwner(ClaimsPrincipal user, Ex015_Document document) =>
        user.Identity?.IsAuthenticated == true &&
        string.Equals(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, document.OwnerId, StringComparison.Ordinal);
}
