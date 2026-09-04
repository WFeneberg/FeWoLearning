using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex037_DataAnnotationsValidationTests : BunitContext
{
    // ContactModel implements IValidatableObject to recurse into Address (see its own
    // comment) - and .NET's Validator only runs that nested check once the root's own
    // [Required]/[Range] attributes already pass with zero errors. So a submission
    // that is meant to be *fully* valid must also give Address a City, or the nested
    // "City is required" keeps the model invalid even with Name and Age both fine.
    // Facts that submit an already-invalid Name or Age never reach that nested check
    // at all (validated empirically: property-level errors short-circuit it), which
    // is why those facts below leave Address untouched.
    private static ContactModel ValidModel(string name, int age) => new()
    {
        Name = name,
        Age = age,
        Address = new AddressModel { City = "Springfield" }
    };

    [Fact]
    public void A_Fully_Valid_Submit_Counts_As_One_Valid()
    {
        var cut = Render<Ex037_DataAnnotationsValidation>(p => p.Add(x => x.Model, ValidModel("Ada", 30)));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("1/0", cut.Find("#counts").TextContent));
    }

    [Fact]
    public void A_Missing_Name_Counts_As_One_Invalid()
    {
        var model = new ContactModel { Name = null };
        var cut = Render<Ex037_DataAnnotationsValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Equal("0/1", cut.Find("#counts").TextContent));
    }

    [Fact]
    public void A_Missing_Name_Shows_Its_Message_In_The_Summary()
    {
        var model = new ContactModel { Name = null };
        var cut = Render<Ex037_DataAnnotationsValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("#errors li").Select(li => li.TextContent);
            Assert.Contains("Name is required", items);
        });
    }

    [Fact]
    public void An_Out_Of_Range_Age_Shows_Its_Message_And_Counts_As_Invalid()
    {
        var model = new ContactModel { Name = "Ada", Age = 999 };
        var cut = Render<Ex037_DataAnnotationsValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var items = cut.FindAll("#errors li").Select(li => li.TextContent);
            Assert.Contains("Age must be between 1 and 120", items);
            Assert.Equal("0/1", cut.Find("#counts").TextContent);
        });
    }

    [Fact]
    public void Fixing_The_Name_After_A_Failed_Submit_And_Resubmitting_Gives_One_Of_Each()
    {
        var model = new ContactModel { Name = null };
        var cut = Render<Ex037_DataAnnotationsValidation>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();
        cut.WaitForAssertion(() => Assert.Equal("0/1", cut.Find("#counts").TextContent));

        model.Name = "Ada";
        model.Address.City = "Springfield";
        cut.Find("form").Submit();

        // Proves the validator re-runs on every submit rather than caching its first
        // verdict.
        cut.WaitForAssertion(() => Assert.Equal("1/1", cut.Find("#counts").TextContent));
    }
}
