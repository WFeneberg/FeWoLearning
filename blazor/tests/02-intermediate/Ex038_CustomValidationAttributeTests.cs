using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex038_CustomValidationAttributeTests : BunitContext
{
    [Fact]
    public void A_Well_Formed_Code_Produces_No_Errors()
    {
        var cut = Render<Ex038_CustomValidationAttribute>();

        cut.Find("#code").Change("ABC-123");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll("#errors li")));
    }

    [Fact]
    public void A_Lowercase_Code_Is_Rejected_With_The_Expected_Message()
    {
        var cut = Render<Ex038_CustomValidationAttribute>();

        cut.Find("#code").Change("abc-123");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var errors = cut.FindAll("#errors li");
            Assert.Single(errors);
            Assert.Equal("Code must look like ABC-123", errors[0].TextContent);
        });
    }

    [Fact]
    public void A_Four_Letter_Prefix_Is_Rejected()
    {
        var cut = Render<Ex038_CustomValidationAttribute>();

        cut.Find("#code").Change("ABCD-123");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var errors = cut.FindAll("#errors li");
            Assert.Single(errors);
            Assert.Equal("Code must look like ABC-123", errors[0].TextContent);
        });
    }

    [Fact]
    public void An_Empty_Code_Is_Rejected()
    {
        var cut = Render<Ex038_CustomValidationAttribute>();

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            var errors = cut.FindAll("#errors li");
            Assert.Single(errors);
            Assert.Equal("Code must look like ABC-123", errors[0].TextContent);
        });
    }
}
