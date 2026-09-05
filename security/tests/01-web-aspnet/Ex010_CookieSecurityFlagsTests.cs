using FeWoLearning.Security.Exercises.WebAspNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex010_CookieSecurityFlagsTests
{
    // Parses the real Set-Cookie header through the framework's own typed
    // header reader, rather than asserting on the raw string - so this test
    // does not break on any legitimate reordering of the cookie's attributes.
    private static SetCookieHeaderValue AppendAndParse(string name, string value)
    {
        var context = new DefaultHttpContext();

        Ex010_CookieSecurityFlags.AppendSessionCookie(context.Response, name, value);

        return Assert.Single(context.Response.GetTypedHeaders().SetCookie);
    }

    [Fact]
    public void Attack_The_Cookie_Is_Marked_HttpOnly()
    {
        var cookie = AppendAndParse("session", "abc123");

        Assert.True(cookie.HttpOnly);
    }

    [Fact]
    public void Attack_The_Cookie_Is_Marked_Secure()
    {
        var cookie = AppendAndParse("session", "abc123");

        Assert.True(cookie.Secure);
    }

    [Fact]
    public void Attack_The_Cookie_Restricts_SameSite_To_Strict()
    {
        var cookie = AppendAndParse("session", "abc123");

        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, cookie.SameSite);
    }

    [Fact]
    public void Use_The_Name_And_Value_Round_Trip_Including_A_Value_Needing_Url_Encoding()
    {
        var cookie = AppendAndParse("session", "needs encoding: & spaces!");

        Assert.Equal("session", cookie.Name.ToString());
        Assert.Equal("needs encoding: & spaces!", Uri.UnescapeDataString(cookie.Value.ToString()));
    }

    [Fact]
    public void Use_The_Cookie_Is_Scoped_To_The_Whole_Site()
    {
        var cookie = AppendAndParse("session", "abc123");

        Assert.Equal("/", cookie.Path.ToString());
    }
}
