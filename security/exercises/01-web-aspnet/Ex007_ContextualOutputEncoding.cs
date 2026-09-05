namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 007 — ContextualOutputEncoding (web-aspnet).
// Goal:   Encode untrusted text for four different output sinks - an HTML body,
//         an HTML attribute value, a JavaScript string literal, and a URL query
//         component - using the encoder built for each sink, because escaping
//         chosen for one sink does not make text safe in another.
// Drills: HtmlEncoder vs JavaScriptEncoder vs UrlEncoder, sink context.
// Passes: attack facts   - "<script>alert(1)</script>" through ForHtmlBody
//                          contains no raw "<"; "\" onmouseover=\"alert(1)"
//                          through ForHtmlAttribute contains no raw double quote;
//                          "</script>" through ForJavaScriptString does not
//                          contain the literal "</script>"; "a&b=c" through
//                          ForUrlQuery contains no raw "&";
//         use facts      - each method leaves a plain alphanumeric string
//                          unchanged, and each sink round-trips through the
//                          decoder its own consumer would use: ForHtmlBody and
//                          ForHtmlAttribute through WebUtility.HtmlDecode (the
//                          attribute one also keeping its spaces literal),
//                          ForUrlQuery through Uri.UnescapeDataString, and
//                          ForJavaScriptString by being read back as the body of
//                          a double-quoted JSON/JavaScript string literal. Those
//                          round-trips are what pin each sink to its own encoder:
//                          the attack facts alone are satisfied by any of the
//                          three, since all three escape all four payloads.
public static class Ex007_ContextualOutputEncoding
{
    public static string ForHtmlBody(string untrusted) =>
        throw new NotImplementedException("TODO: Ex007 - encode untrusted for an HTML body using HtmlEncoder");

    public static string ForHtmlAttribute(string untrusted) =>
        throw new NotImplementedException(
            "TODO: Ex007 - encode untrusted for an HTML attribute value using HtmlEncoder");

    public static string ForJavaScriptString(string untrusted) =>
        throw new NotImplementedException(
            "TODO: Ex007 - encode untrusted for a JavaScript string literal using JavaScriptEncoder");

    public static string ForUrlQuery(string untrusted) =>
        throw new NotImplementedException("TODO: Ex007 - encode untrusted for a URL query component using UrlEncoder");
}
