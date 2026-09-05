using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex021_SsrfOutboundGuardTests
{
    [Theory]
    [InlineData("http://127.0.0.1/admin")]
    [InlineData("http://localhost/")]
    [InlineData("http://169.254.169.254/latest/meta-data/")]
    [InlineData("http://10.0.0.5/")]
    [InlineData("http://192.168.1.1/")]
    [InlineData("file:///C:/Windows/win.ini")]
    [InlineData("gopher://example.com/")]
    [InlineData("http://[::1]/")]
    public void Attack_A_Local_Private_Or_NonHttp_Target_Is_Rejected(string url)
    {
        Assert.False(Ex021_SsrfOutboundGuard.IsAllowedTarget(url));
    }

    [Theory]
    [InlineData("https://api.example.com/v1/items")]
    [InlineData("https://example.com:8443/path?q=1")]
    public void Use_A_Public_Https_Target_Is_Allowed(string url)
    {
        Assert.True(Ex021_SsrfOutboundGuard.IsAllowedTarget(url));
    }
}
