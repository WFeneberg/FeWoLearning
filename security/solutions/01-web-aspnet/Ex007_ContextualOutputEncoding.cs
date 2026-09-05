using System.Text.Encodings.Web;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 007 — ContextualOutputEncoding (reference solution).
public static class Ex007_ContextualOutputEncoding
{
    public static string ForHtmlBody(string untrusted) => HtmlEncoder.Default.Encode(untrusted);

    // The same HtmlEncoder that is safe for element content is also safe for a
    // quoted attribute value: it escapes both '"' and '\'', so it works whichever
    // quote character wraps the attribute.
    public static string ForHtmlAttribute(string untrusted) => HtmlEncoder.Default.Encode(untrusted);

    public static string ForJavaScriptString(string untrusted) => JavaScriptEncoder.Default.Encode(untrusted);

    public static string ForUrlQuery(string untrusted) => UrlEncoder.Default.Encode(untrusted);
}
