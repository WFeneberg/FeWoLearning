namespace FeWoLearning.Security.Exercises.DesktopWpf;

// Exercise 057 — EmbeddedBrowserNavigationPolicy (desktop-wpf).
// Goal:   An embedded browser control (WebView2 or otherwise) hosted inside a desktop
//         app is a live navigation surface: whatever URI the embedded page (or a link
//         a user clicks inside it) tries to navigate to, the host app must decide
//         whether to allow it in-frame, allow it but hand it off to the system
//         browser, or refuse it outright — before any navigation happens. This
//         exercise is the decision function alone, as a plain class with no WebView2
//         reference: the real control cannot be meaningfully exercised headless, but
//         the policy that would gate its NavigationStarting event can be.
// Drills: navigation allowlists, scheme restrictions, host object exposure.
// Passes: attack facts   - a `javascript:` target is denied (it would run script in
//                          the embedded browser's own context); a `file:///` target
//                          is denied (it would let embedded content read the local
//                          filesystem); a `data:text/html` target is denied (a data
//                          URI can smuggle an entire inline document past a host
//                          allowlist); a plain `http` (not `https`) target is denied
//                          regardless of host.
//         use facts      - a target on `appOrigin` is allowed in-frame (Allow true,
//                          OpenExternally false); an `https` target on a *different*
//                          host is allowed, but OpenExternally is true — the fact
//                          that stops a policy which simply denies everything.
public sealed record Ex057_NavigationDecision(bool Allow, bool OpenExternally, string? Reason);

public static class Ex057_EmbeddedBrowserNavigationPolicy
{
    public static Ex057_NavigationDecision Decide(string targetUri, string appOrigin) =>
        throw new NotImplementedException(
            "TODO: Ex057 - deny javascript:/file:/data: schemes and plain http outright; allow https " +
            "targets, staying in-frame when the host matches appOrigin and flagging OpenExternally otherwise");
}
