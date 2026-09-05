using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 014 — AuthorizationPolicies (web-aspnet).
// Goal:   Register a policy-based authorization requirement over a dateOfBirth
//         claim, so "is this principal an adult" is a reusable, declarative
//         policy rather than an if-check scattered across every endpoint that
//         needs it.
// Drills: policy-based authorization, requirements, handler registration.
// Passes: attack facts   - a principal with no dateOfBirth claim fails the
//                          policy; a 17-year-old fails; a principal whose
//                          dateOfBirth claim value is malformed fails the
//                          policy rather than throwing;
//         use facts      - an 18-year-old passes the policy, and a 40-year-old
//                          passes the policy.
public static class Ex014_AuthorizationPolicies
{
    public const string PolicyName = "AdultsOnly";

    public static void AddServices(IServiceCollection services) =>
        throw new NotImplementedException(
            "TODO: Ex014 - register the AdultsOnly policy with a requirement/handler pair that reads the dateOfBirth claim and fails (never throws) when it is missing or malformed");
}
