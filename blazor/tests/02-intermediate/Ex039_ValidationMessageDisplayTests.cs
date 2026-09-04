using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex039_ValidationMessageDisplayTests : BunitContext
{
    [Fact]
    public void Both_Fields_Invalid_Shows_Each_Ones_Own_Message_In_Its_Own_Div()
    {
        var model = new ContactModel { Age = 999 };
        var cut = Render<Ex039_ValidationMessageDisplay>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("Name is required", cut.Find("#name-field .validation-message").TextContent);
            Assert.Equal("Age must be between 1 and 120", cut.Find("#age-field .validation-message").TextContent);
        });
    }

    [Fact]
    public void Only_The_Invalid_Fields_Own_Div_Shows_A_Message()
    {
        // Age keeps its default of 1, which is inside the valid [1, 120] range, so
        // this model is invalid on Name alone.
        var model = new ContactModel();
        var cut = Render<Ex039_ValidationMessageDisplay>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotEmpty(cut.FindAll("#name-field .validation-message"));
            Assert.Empty(cut.FindAll("#age-field .validation-message"));
        });
    }

    [Fact]
    public void Both_Fields_Valid_Shows_No_Messages_In_Either_Div()
    {
        var model = new ContactModel { Name = "Ada", Age = 30 };
        var cut = Render<Ex039_ValidationMessageDisplay>(p => p.Add(x => x.Model, model));

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Empty(cut.FindAll("#name-field .validation-message"));
            Assert.Empty(cut.FindAll("#age-field .validation-message"));
        });
    }
}
