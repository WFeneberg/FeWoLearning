using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 014 — AuthorizationPolicies (reference solution).
internal sealed class Ex014_MinimumAgeRequirement : IAuthorizationRequirement
{
    public Ex014_MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;

    public int MinimumAge { get; }
}

internal sealed class Ex014_MinimumAgeHandler : AuthorizationHandler<Ex014_MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, Ex014_MinimumAgeRequirement requirement)
    {
        var claim = context.User.FindFirst("dateOfBirth");

        // A missing or malformed claim simply never succeeds the requirement -
        // it must never throw, or a single bad claim would take down every
        // authorization check that shares this handler.
        if (claim is not null &&
            DateTime.TryParse(claim.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth) &&
            CalculateAge(dateOfBirth, DateTime.UtcNow) >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static int CalculateAge(DateTime dateOfBirth, DateTime today)
    {
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}

public static class Ex014_AuthorizationPolicies
{
    public const string PolicyName = "AdultsOnly";

    public static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, Ex014_MinimumAgeHandler>();
        services.AddAuthorization(options =>
            options.AddPolicy(PolicyName, policy => policy.Requirements.Add(new Ex014_MinimumAgeRequirement(18))));
    }
}
