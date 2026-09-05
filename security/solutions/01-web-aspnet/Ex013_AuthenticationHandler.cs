using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 013 — AuthenticationHandler (reference solution).
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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var provided) || provided.Count == 0)
        {
            return Task.FromResult(AuthenticateResult.Fail("X-Api-Key header is missing"));
        }

        // Ordinal, case-sensitive comparison: a key differing only in case must
        // never be treated as a match.
        if (!string.Equals(provided.ToString(), _apiKeyOptions.ValidApiKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("X-Api-Key header does not match"));
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "api-client") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class Ex013_AuthenticationHandler
{
    public const string SchemeName = "ApiKey";

    public static void AddServices(IServiceCollection services, string validApiKey)
    {
        services.AddSingleton(new Ex013_ApiKeyOptions(validApiKey));
        services
            .AddAuthentication(SchemeName)
            .AddScheme<AuthenticationSchemeOptions, Ex013_ApiKeyAuthenticationHandler>(SchemeName, _ => { });
    }
}
