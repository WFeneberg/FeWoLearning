using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex057_EmbeddedBrowserNavigationPolicyTests
{
    private const string AppOrigin = "https://app.example.com";

    [WpfFact]
    public void Attack_A_Javascript_Uri_Is_Denied()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide("javascript:alert(document.cookie)", AppOrigin);

        Assert.False(decision.Allow);
    }

    [WpfFact]
    public void Attack_A_File_Uri_Is_Denied()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide("file:///C:/Users/secrets.txt", AppOrigin);

        Assert.False(decision.Allow);
    }

    [WpfFact]
    public void Attack_A_Data_Html_Uri_Is_Denied()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide(
            "data:text/html,<script>alert(document.domain)</script>", AppOrigin);

        Assert.False(decision.Allow);
    }

    [WpfFact]
    public void Attack_A_Plain_Http_Url_Is_Denied()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide("http://app.example.com/page", AppOrigin);

        Assert.False(decision.Allow);
    }

    [WpfFact]
    public void Use_A_Url_On_The_App_Origin_Is_Allowed_In_Frame()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide("https://app.example.com/help/faq", AppOrigin);

        Assert.True(decision.Allow);
        Assert.False(decision.OpenExternally);
    }

    [WpfFact]
    public void Use_An_Https_Url_On_Another_Host_Is_Allowed_But_Opened_Externally()
    {
        var decision = Ex057_EmbeddedBrowserNavigationPolicy.Decide("https://other.example.com/", AppOrigin);

        Assert.True(decision.Allow);
        Assert.True(decision.OpenExternally);
    }
}
