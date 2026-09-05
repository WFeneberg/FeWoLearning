namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 057 — EmbeddedBrowserNavigationPolicy (reference solution).
public sealed record Ex057_NavigationDecision(bool Allow, bool OpenExternally, string? Reason);

public static class Ex057_EmbeddedBrowserNavigationPolicy
{
    public static Ex057_NavigationDecision Decide(string targetUri, string appOrigin)
    {
        ArgumentNullException.ThrowIfNull(targetUri);
        ArgumentNullException.ThrowIfNull(appOrigin);

        if (!Uri.TryCreate(targetUri, UriKind.Absolute, out var target))
            return new Ex057_NavigationDecision(false, false, "not an absolute URI");

        var scheme = target.Scheme.ToLowerInvariant();

        if (scheme == "javascript")
            return new Ex057_NavigationDecision(false, false, "javascript: would execute script in the embedded browser");

        if (scheme == Uri.UriSchemeFile)
            return new Ex057_NavigationDecision(false, false, "file: would let embedded content read the local filesystem");

        if (scheme == "data")
            return new Ex057_NavigationDecision(false, false, "data: can smuggle an inline document past the allowlist");

        if (scheme == Uri.UriSchemeHttp)
            return new Ex057_NavigationDecision(false, false, "plain http is never allowed, only https and the app's own origin");

        if (scheme != Uri.UriSchemeHttps)
            return new Ex057_NavigationDecision(false, false, $"scheme '{scheme}' is not allowed");

        if (!Uri.TryCreate(appOrigin, UriKind.Absolute, out var origin))
            throw new ArgumentException("appOrigin must be an absolute URI", nameof(appOrigin));

        var sameOrigin =
            string.Equals(target.Scheme, origin.Scheme, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(target.Host, origin.Host, StringComparison.OrdinalIgnoreCase) &&
            target.Port == origin.Port;

        return sameOrigin
            ? new Ex057_NavigationDecision(true, false, null)
            : new Ex057_NavigationDecision(true, true, "different origin than the host app; hand off to the system browser");
    }
}
