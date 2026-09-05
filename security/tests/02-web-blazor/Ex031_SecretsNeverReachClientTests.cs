using System.Text.Json;
using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex031_SecretsNeverReachClientTests
{
    private static readonly Ex031_ApiSettings Settings =
        new("https://api.example.com", "sk-live-topsecret-9f8e7d6c5b4a");

    [Fact]
    public void Attack_Rendered_Output_Contains_Neither_The_ApiKey_Value_Nor_Its_Name()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex031_ConfigPanel>(p => p.Add(c => c.Settings, Settings));
        var html = cut.Markup;

        Assert.DoesNotContain(Settings.ApiKey, html);
        Assert.DoesNotContain("ApiKey", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Attack_Serialised_Client_View_Contains_No_Secret_Anywhere()
    {
        var view = Ex031_SecretsNeverReachClient.ToClientView(Settings);
        var json = JsonSerializer.Serialize(view);

        Assert.DoesNotContain(Settings.ApiKey, json);
        Assert.DoesNotContain("ApiKey", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Rendered_Output_Contains_The_Public_Base_Url()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex031_ConfigPanel>(p => p.Add(c => c.Settings, Settings));

        Assert.Contains(Settings.PublicBaseUrl, cut.Markup);
    }

    [Fact]
    public void Use_Client_View_Exposes_PublicBaseUrl_Under_A_Stable_Member_Name()
    {
        var view = Ex031_SecretsNeverReachClient.ToClientView(Settings);

        var property = view.GetType().GetProperty("PublicBaseUrl");

        Assert.NotNull(property);
        Assert.Equal(Settings.PublicBaseUrl, property!.GetValue(view));
    }
}
