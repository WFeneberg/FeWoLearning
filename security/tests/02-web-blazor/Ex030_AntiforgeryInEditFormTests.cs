using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using Microsoft.AspNetCore.Components;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex030_AntiforgeryInEditFormTests
{
    [Fact]
    public void Attack_Rendered_Form_Always_Carries_A_Non_Empty_Antiforgery_Token()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex030_AntiforgeryInEditForm>(p => p
            .Add(c => c.Model, new Ex030_ProfileModel { Name = "Ada", Email = "ada@example.com" }));

        var token = cut.Find("input[name='__RequestVerificationToken']").GetAttribute("value");

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void Attack_Two_Separately_Rendered_Instances_Produce_Different_Tokens()
    {
        using var harness1 = new BlazorHarness();
        using var harness2 = new BlazorHarness();

        var cut1 = harness1.Render<Ex030_AntiforgeryInEditForm>(p => p.Add(c => c.Model, new Ex030_ProfileModel()));
        var cut2 = harness2.Render<Ex030_AntiforgeryInEditForm>(p => p.Add(c => c.Model, new Ex030_ProfileModel()));

        var token1 = cut1.Find("input[name='__RequestVerificationToken']").GetAttribute("value");
        var token2 = cut2.Find("input[name='__RequestVerificationToken']").GetAttribute("value");

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void Use_Fields_Render_With_The_Models_Current_Values()
    {
        using var harness = new BlazorHarness();
        var model = new Ex030_ProfileModel { Name = "Grace Hopper", Email = "grace@example.com" };

        var cut = harness.Render<Ex030_AntiforgeryInEditForm>(p => p.Add(c => c.Model, model));

        Assert.Equal("Grace Hopper", cut.Find("#name").GetAttribute("value"));
        Assert.Equal("grace@example.com", cut.Find("#email").GetAttribute("value"));
    }

    [Fact]
    public async Task Use_Submitting_Valid_Data_Invokes_OnValidSubmit_Exactly_Once()
    {
        using var harness = new BlazorHarness();
        var invocationCount = 0;

        var cut = harness.Render<Ex030_AntiforgeryInEditForm>(p => p
            .Add(c => c.Model, new Ex030_ProfileModel { Name = "Ada", Email = "ada@example.com" })
            .Add(c => c.OnValidSubmit, EventCallback.Factory.Create(this, () => invocationCount++)));

        await cut.Find("form").SubmitAsync();

        Assert.Equal(1, invocationCount);
    }
}
