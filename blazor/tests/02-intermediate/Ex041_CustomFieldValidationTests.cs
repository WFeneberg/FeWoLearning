using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex041_CustomFieldValidationTests : BunitContext
{
    [Fact]
    public void A_Valid_Name_Submits_Clean()
    {
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, new ContactModel { Name = "Ada" }));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("#errors li"));
            Assert.Equal("1/0", cut.Find("#counts").TextContent);
        });
    }

    [Fact]
    public void An_Admin_Name_Is_Rejected_With_The_Expected_Message()
    {
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, new ContactModel { Name = "admin" }));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var errors = cut.FindAll("#errors li");
            Assert.Single(errors);
            Assert.Equal("Name must not be \"admin\"", errors[0].TextContent);
            Assert.Equal("0/1", cut.Find("#counts").TextContent);
        });
    }

    [Fact]
    public void The_Admin_Check_Is_Case_Insensitive()
    {
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, new ContactModel { Name = "ADMIN" }));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var errors = cut.FindAll("#errors li");
            Assert.Single(errors);
            Assert.Equal("Name must not be \"admin\"", errors[0].TextContent);
        });
    }

    // Non-vacuity: a validator that never clears its ValidationMessageStore at the
    // start of a validation request would leave this first, stale message behind
    // forever - #errors would still show it, and the counts would never reach 1/1.
    [Fact]
    public void Fixing_The_Name_After_A_Rejected_Submit_Clears_The_Error()
    {
        var model = new ContactModel { Name = "admin" };
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("#errors li")));

        model.Name = "Ada";
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("#errors li"));
            Assert.Equal("1/1", cut.Find("#counts").TextContent);
        });
    }
}
