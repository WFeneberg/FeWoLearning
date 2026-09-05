namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 021 — SsrfOutboundGuard (web-aspnet).
// Goal:   Decide whether an outbound URL an application is about to fetch on the
//         caller's behalf is safe to request at all - rejecting any non-http(s)
//         scheme and any address that names the local host, a loopback, a
//         link-local or a private range, so a server-side request forgery cannot
//         reach the cloud metadata service, a loopback admin panel, or the local
//         filesystem through a URL the caller merely supplied as data.
// Drills: outbound URL validation, scheme allowlists, private address ranges.
// Passes: attack facts   - "http://127.0.0.1/admin", "http://localhost/",
//                          "http://169.254.169.254/latest/meta-data/" (the cloud
//                          metadata endpoint), "http://10.0.0.5/",
//                          "http://192.168.1.1/", "file:///C:/Windows/win.ini",
//                          "gopher://example.com/" and a URL whose host is
//                          "[::1]" are all rejected;
//         use facts      - "https://api.example.com/v1/items" is allowed, and so
//                          is "https://example.com:8443/path?q=1" - a
//                          non-standard port on a public host must still pass,
//                          or this is a port filter rather than an address one.
public static class Ex021_SsrfOutboundGuard
{
    public static bool IsAllowedTarget(string url) =>
        throw new NotImplementedException(
            "TODO: Ex021 - allow only http/https URLs whose host is not localhost, loopback, link-local or a private range");
}
