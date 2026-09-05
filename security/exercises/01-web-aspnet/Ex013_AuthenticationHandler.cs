using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 013 — AuthenticationHandler (web-aspnet).
// Goal:   Write a custom AuthenticationHandler that authenticates a request from
//         an X-Api-Key header rather than a cookie or bearer token, building a
//         ClaimsPrincipal only once the key is checked to be an exact,
//         case-sensitive match.
// Drills: AuthenticationHandler, ClaimsPrincipal construction, scheme selection.
// Passes: attack facts   - a request with no X-Api-Key header fails
//                          authentication (401 from a challenged endpoint); a
//                          request with a wrong key fails; a request whose key
//                          differs only in case fails;
//         use facts      - a request with the valid key reaches the endpoint
//                          (200) and the endpoint observes a ClaimsPrincipal
//                          with Identity.IsAuthenticated true and a
//                          NameIdentifier claim.
public sealed record Ex013_ApiKeyOptions(string ValidApiKey);

public sealed class Ex013_ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly Ex013_ApiKeyOptions _apiKeyOptions;

    public Ex013_ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        Ex013_ApiKeyOptions apiKeyOptions)
        : base(options, logger, encoder)
    {
        _apiKeyOptions = apiKeyOptions;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        throw new NotImplementedException(
            "TODO: Ex013 - fail unless X-Api-Key is present and matches _apiKeyOptions.ValidApiKey exactly (case-sensitive), else build a ClaimsPrincipal carrying a NameIdentifier claim");
}

public static class Ex013_AuthenticationHandler
{
    public const string SchemeName = "ApiKey";

    public static void AddServices(IServiceCollection services, string validApiKey) =>
        throw new NotImplementedException(
            "TODO: Ex013 - register a singleton Ex013_ApiKeyOptions(validApiKey) and the SchemeName authentication scheme backed by Ex013_ApiKeyAuthenticationHandler");
}
