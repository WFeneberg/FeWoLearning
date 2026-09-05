using System.Text.Json;
using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex034_PersistentStateLeakTests
{
    private const string AuthToken = "tok_9f8a3b2c1d4e5f60";
    private const string Email = "ada.lovelace@example.com";
    private const string DisplayName = "Ada";
    private const string LastViewedPage = "/reports";
    private const string UserId = "user-77219";

    private static IRenderedComponent<Ex034_PersistentStateLeak> RenderWithFullSession(BlazorHarness harness) =>
        harness.Render<Ex034_PersistentStateLeak>(p => p
            .Add(c => c.SessionAuthToken, AuthToken)
            .Add(c => c.SessionEmail, Email)
            .Add(c => c.SessionDisplayName, DisplayName)
            .Add(c => c.SessionLastViewedPage, LastViewedPage)
            .Add(c => c.SessionUserId, UserId));

    [Fact]
    public void Attack_Persisted_Payload_Excludes_Secrets_And_The_Key_Excludes_The_User_Id()
    {
        using var harness = new BlazorHarness();
        var state = harness.AddBunitPersistentComponentState();

        RenderWithFullSession(harness);
        state.TriggerOnPersisting();

        Assert.DoesNotContain(UserId, Ex034_PersistentStateLeak.PersistenceKey, StringComparison.OrdinalIgnoreCase);

        Assert.True(state.TryTake<JsonElement>(Ex034_PersistentStateLeak.PersistenceKey, out var persisted));
        var raw = persisted.GetRawText();

        Assert.DoesNotContain(AuthToken, raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(Email, raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Persisted_Payload_Contains_The_Display_Name_And_Last_Viewed_Page()
    {
        using var harness = new BlazorHarness();
        var state = harness.AddBunitPersistentComponentState();

        RenderWithFullSession(harness);
        state.TriggerOnPersisting();

        Assert.True(state.TryTake<JsonElement>(Ex034_PersistentStateLeak.PersistenceKey, out var persisted));
        var raw = persisted.GetRawText();

        Assert.Contains(DisplayName, raw);
        Assert.Contains(LastViewedPage, raw);
    }

    [Fact]
    public void Use_TryTake_Restores_State_For_The_Interactive_Render()
    {
        using var harness = new BlazorHarness();
        var state = harness.AddBunitPersistentComponentState();

        RenderWithFullSession(harness);
        state.TriggerOnPersisting();

        // A fresh instance, with none of the original parameters - exactly
        // what the interactive circuit renders after prerendering handed off.
        var secondRender = harness.Render<Ex034_PersistentStateLeak>();

        Assert.Equal(DisplayName, secondRender.Find("#display-name").TextContent);
        Assert.Equal(LastViewedPage, secondRender.Find("#last-viewed").TextContent);
    }
}
