using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Components;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex035_ErrorBoundaryLeakageTests
{
    private const string SecretExceptionMessage = "connection string: Server=prod-db;Password=hunter2!";

    private static void ThrowFromDeepInTheCallStack() =>
        throw new InvalidOperationException(SecretExceptionMessage);

    [Fact]
    public void Attack_Error_Content_Never_Leaks_The_Exceptions_Message_Type_Or_A_Stack_Frame()
    {
        using var harness = new BlazorHarness();
        RenderFragment throwingChild = _ => ThrowFromDeepInTheCallStack();

        var cut = harness.Render<Ex035_ErrorBoundaryLeakage>(p => p.AddChildContent(throwingChild));
        var html = cut.Find("#error-content").InnerHtml;

        Assert.DoesNotContain(SecretExceptionMessage, html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(nameof(InvalidOperationException), html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(nameof(ThrowFromDeepInTheCallStack), html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Error_Content_Shows_A_Stable_Non_Empty_Message_And_A_Correlation_Id()
    {
        using var harness = new BlazorHarness();
        RenderFragment throwingChild = _ => throw new InvalidOperationException("boom");

        var cut = harness.Render<Ex035_ErrorBoundaryLeakage>(p => p.AddChildContent(throwingChild));

        var message = cut.Find("#error-message").TextContent;
        var correlationId = cut.Find("#error-correlation-id").TextContent;

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.False(string.IsNullOrWhiteSpace(correlationId));
    }

    [Fact]
    public void Use_A_Non_Throwing_Child_Renders_Normally_With_No_Error_Content()
    {
        using var harness = new BlazorHarness();
        RenderFragment normalChild = builder => builder.AddContent(0, "all good");

        var cut = harness.Render<Ex035_ErrorBoundaryLeakage>(p => p.AddChildContent(normalChild));

        // FindAll + Assert.Single rather than Find(): an implementation that
        // never renders #normal-content at all must fail this on the
        // assertion, not on Bunit.ElementNotFoundException from Find().
        var normalContent = cut.FindAll("#normal-content");
        Assert.Single(normalContent);
        Assert.Equal("all good", normalContent[0].TextContent);
        Assert.Empty(cut.FindAll("#error-content"));
    }
}
