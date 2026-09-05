using System.Net;
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
}
