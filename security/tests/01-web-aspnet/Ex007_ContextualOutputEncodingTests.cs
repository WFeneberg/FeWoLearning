using System.Net;
using System.Text.Json;
using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex007_ContextualOutputEncodingTests
{
    [Fact]
    public void Attack_ForHtmlBody_Leaves_No_Raw_Angle_Bracket()
    {
        var result = Ex007_ContextualOutputEncoding.ForHtmlBody("<script>alert(1)</script>");

        Assert.DoesNotContain("<", result);
    }

    [Fact]
    public void Attack_ForHtmlAttribute_Leaves_No_Raw_Double_Quote()
    {
        var result = Ex007_ContextualOutputEncoding.ForHtmlAttribute("\" onmouseover=\"alert(1)");

        Assert.DoesNotContain("\"", result);
    }

    [Fact]
    public void Attack_ForJavaScriptString_Breaks_Up_A_Closing_Script_Tag()
    {
        var result = Ex007_ContextualOutputEncoding.ForJavaScriptString("</script>");

        Assert.DoesNotContain("</script>", result);
    }

    [Fact]
    public void Attack_ForUrlQuery_Leaves_No_Raw_Ampersand()
    {
        var result = Ex007_ContextualOutputEncoding.ForUrlQuery("a&b=c");

        Assert.DoesNotContain("&", result);
    }

    [Fact]
    public void Use_A_Plain_Alphanumeric_String_Is_Unchanged_In_Every_Sink()
    {
        const string input = "plainAlphanumeric123";

        Assert.Equal(input, Ex007_ContextualOutputEncoding.ForHtmlBody(input));
        Assert.Equal(input, Ex007_ContextualOutputEncoding.ForHtmlAttribute(input));
        Assert.Equal(input, Ex007_ContextualOutputEncoding.ForJavaScriptString(input));
        Assert.Equal(input, Ex007_ContextualOutputEncoding.ForUrlQuery(input));
    }

    [Fact]
    public void Use_ForHtmlBody_Round_Trips_Non_Ascii_Text()
    {
        var encoded = Ex007_ContextualOutputEncoding.ForHtmlBody("café");

        Assert.Equal("café", WebUtility.HtmlDecode(encoded));
    }

    // The three facts below exist because the four attack facts above only prove
    // that *something* was escaped - and any of the three encoders escapes all of
    // those payloads. Each of these pins one sink to its own encoder by decoding
    // the output the way that sink's consumer would: swap in a different encoder
    // in good faith and the round-trip stops round-tripping.
    [Fact]
    public void Use_ForUrlQuery_Round_Trips_Through_Uri_UnescapeDataString()
    {
        // Percent-encoding is what a URL query consumer undoes. A JavaScript or
        // HTML encoder escapes the same characters, but not into %XX, so
        // UnescapeDataString hands back the escape sequences verbatim instead.
        const string input = "a b&c=d/café";

        var encoded = Ex007_ContextualOutputEncoding.ForUrlQuery(input);

        Assert.Equal(input, Uri.UnescapeDataString(encoded));
    }

    [Fact]
    public void Use_ForHtmlAttribute_Keeps_Spaces_And_Round_Trips_Through_HtmlDecode()
    {
        // A UrlEncoder would turn every space into %20 - legal in a URL, but
        // garbage inside an attribute value - and a JavaScriptEncoder's unicode
        // escapes survive HtmlDecode untouched. Only an HTML encoder does both.
        const string input = "he said \"hi\" & left";

        var encoded = Ex007_ContextualOutputEncoding.ForHtmlAttribute(input);

        Assert.Contains(" ", encoded);
        Assert.Equal(input, WebUtility.HtmlDecode(encoded));
    }

    [Fact]
    public void Use_ForJavaScriptString_Round_Trips_As_A_JSON_String_Literal()
    {
        // The defining property of this sink: the output must be a valid body for
        // a double-quoted JavaScript/JSON string literal, and reading that literal
        // back must yield the original text. "&quot;" and "%22" both fail that.
        const string input = "he said \"hi\" \\ then </script> & 'left'";

        var encoded = Ex007_ContextualOutputEncoding.ForJavaScriptString(input);

        Assert.Equal(input, JsonSerializer.Deserialize<string>("\"" + encoded + "\""));
    }
}
