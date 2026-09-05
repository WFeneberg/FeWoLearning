using System.Net;
using System.Net.Sockets;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 021 — SsrfOutboundGuard (reference solution).
public static class Ex021_SsrfOutboundGuard
{
    public static bool IsAllowedTarget(string url)
    {
        // An unparsable or relative "URL" was never a fetchable target to begin
        // with, and a scheme other than http/https - file, gopher, ftp, and so on
        // - reaches resources an outbound-fetch feature has no business touching.
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return false;

        var host = uri.Host;

        // "localhost" never appears as an IP literal, so it needs its own check
        // before the literal-address path below even runs.
        if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
            return false;

        // A hostname (api.example.com) is not an IP literal at all - it is exactly
        // the public-target case the use facts require, and it reaches here with
        // no further check. Only a literal address - the shape every attack fact
        // in this exercise takes - is scrutinised further; DNS rebinding behind a
        // hostname is out of scope for a synchronous, network-free predicate.
        if (!IPAddress.TryParse(host, out var ip))
            return true;

        if (IPAddress.IsLoopback(ip))
            return false;

        return !IsPrivateOrLinkLocal(ip);
    }

    private static bool IsPrivateOrLinkLocal(IPAddress ip)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var b = ip.GetAddressBytes();
            return b[0] == 10
                || (b[0] == 172 && b[1] is >= 16 and <= 31)
                || (b[0] == 192 && b[1] == 168)
                || (b[0] == 169 && b[1] == 254); // link-local, including the cloud metadata address
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal;

        return false;
    }
}
