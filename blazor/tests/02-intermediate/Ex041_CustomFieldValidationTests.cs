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

        // Negative assertion - stays bare per README §11: WaitForAssertion cannot help
        // it, and if this genuinely stayed non-empty, wrapping would only delay
        // catching that by the full timeout.
        Assert.Empty(cut.FindAll("#errors li"));
        cut.WaitForAssertion(() => Assert.Equal("1/0", cut.Find("#counts").TextContent));
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

    // Exercises the FieldIdentifier the validator actually used, not just that a
    // message exists somewhere: ValidationMessage's For="() => Model.Name" only
    // renders a message added under that exact FieldIdentifier. A validator that adds
    // its message under any other identifier (e.g. a nested "Address.Bogus" path)
    // still satisfies the unscoped #errors list above, but leaves #name-errors empty.
    [Fact]
    public void An_Admin_Name_Also_Shows_Its_Message_Next_To_The_Field()
    {
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, new ContactModel { Name = "admin" }));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.Equal("Name must not be \"admin\"", cut.Find("#name-errors").TextContent));
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

    // Non-vacuity (verified live by breaking the solution and restoring it): a
    // validator that never clears its ValidationMessageStore at the start of a
    // validation request leaves this first, stale message behind forever - #errors
    // would still show it, and the counts would never reach 1/1.
    [Fact]
    public void Fixing_The_Name_After_A_Rejected_Submit_Clears_The_Error()
    {
        var model = new ContactModel { Name = "admin" };
        var cut = Render<Ex041_CustomFieldValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll("#errors li")));

        model.Name = "Ada";
        cut.Find("form").Submit();

        // Negative assertion - stays bare per README §11 (see the first fact above).
        Assert.Empty(cut.FindAll("#errors li"));
        cut.WaitForAssertion(() => Assert.Equal("1/1", cut.Find("#counts").TextContent));
    }
}
