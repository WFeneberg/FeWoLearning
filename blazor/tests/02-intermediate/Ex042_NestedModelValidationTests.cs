using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex042_NestedModelValidationTests : BunitContext
{
    // Types into both #name and #city rather than pre-setting the model, so a stub
    // that forgets to bind Model.Address.City to the #city input (or forgets to
    // render #city at all) cannot pass by accident - it either throws finding the
    // missing element or leaves Address.City null and reports 0/1 instead of 1/0.
    [Fact]
    public void Filling_In_Name_And_City_Via_The_Inputs_Submits_Clean()
    {
        var model = new ContactModel { Name = null };
        var cut = Render<Ex042_NestedModelValidation>(p => p.Add(x => x.Model, model));

        cut.Find("#name").Change("Ada");
        cut.Find("#city").Change("Springfield");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("1/0", cut.Find("#counts").TextContent));
    }

    [Fact]
    public void An_Empty_City_Is_Invalid_With_Its_Own_Message()
    {
        var model = new ContactModel { Name = "Ada" };
        var cut = Render<Ex042_NestedModelValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("#errors li").Select(li => li.TextContent);
            Assert.Contains("City is required", items);
            Assert.Equal("0/1", cut.Find("#counts").TextContent);
        });
    }

    // Non-vacuity for the nested check specifically (verified live: with City left
    // set, this fact could never distinguish a real short-circuit from one that
    // still reached the nested check, since neither path would ever produce "City is
    // required" - the fix is leaving City null too). ContactModel only runs its
    // hand-written Address recursion once its OWN properties (Name, Age) already
    // pass, so a missing Name must short-circuit to just "Name is required" and never
    // reach the nested City check at all, even though the city really is unset and
    // would fail it if reached.
    [Fact]
    public void An_Empty_Name_Short_Circuits_Before_The_Nested_City_Check_Even_Though_City_Is_Also_Unset()
    {
        var model = new ContactModel { Name = null };
        var cut = Render<Ex042_NestedModelValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        // Negative assertion - stays bare per README §11 (see Ex041's tests for the
        // same reasoning): WaitForAssertion cannot help it, and if the nested check
        // genuinely ran anyway, wrapping would only delay catching that by the full
        // timeout.
        Assert.DoesNotContain("City is required", cut.FindAll("#errors li").Select(li => li.TextContent));

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("#errors li").Select(li => li.TextContent).ToList();
            Assert.Contains("Name is required", items);
            Assert.Equal("0/1", cut.Find("#counts").TextContent);
        });
    }
}
