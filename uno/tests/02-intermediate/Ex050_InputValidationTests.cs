using System.Collections;
using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex050_InputValidationTests : UnoTestContext
{
    private static List<string> Errors(Ex050_InputValidation form, string? property) =>
        form.GetErrors(property).Cast<object>().Select(e => e.ToString()!).ToList();

    private static Ex050_InputValidation Valid() => new() { Name = "Ada", Age = 36 };

    [Fact]
    public void A_Valid_Form_Has_No_Errors()
    {
        var form = Valid();

        Assert.False(form.HasErrors);
        Assert.Empty(Errors(form, nameof(Ex050_InputValidation.Name)));
    }

    [Fact]
    public void A_Blank_Name_Is_An_Error()
    {
        var form = Valid();

        form.Name = "";

        Assert.True(form.HasErrors);
        Assert.Equal(["Name is required"], Errors(form, nameof(Ex050_InputValidation.Name)));
    }

    [Fact]
    public void An_Age_Out_Of_Range_Is_An_Error()
    {
        var form = Valid();

        form.Age = 200;

        Assert.Equal(["Age is out of range"], Errors(form, nameof(Ex050_InputValidation.Age)));
    }

    [Fact]
    public void A_Negative_Age_Is_An_Error()
    {
        var form = Valid();

        form.Age = -1;

        Assert.Equal(["Age is out of range"], Errors(form, nameof(Ex050_InputValidation.Age)));
    }

    [Fact]
    public void Errors_Are_Reported_Per_Property()
    {
        var form = Valid();

        form.Name = "";

        Assert.NotEmpty(Errors(form, nameof(Ex050_InputValidation.Name)));
        Assert.Empty(Errors(form, nameof(Ex050_InputValidation.Age)));
    }

    [Fact]
    public void A_Null_Property_Name_Asks_For_Everything()
    {
        var form = Valid();

        form.Name = "";
        form.Age = 200;

        // The framework calls it this way for a form-level summary, and an implementation
        // that only switches on known names returns nothing here.
        Assert.Equal(2, Errors(form, null).Count);
    }

    [Fact]
    public void An_Unknown_Property_Has_No_Errors()
    {
        var form = Valid();

        form.Name = "";

        Assert.Empty(Errors(form, "Nickname"));
    }

    [Fact]
    public void Announces_A_Property_That_Became_Invalid()
    {
        var form = Valid();
        var announced = new List<string?>();
        form.ErrorsChanged += (_, e) => announced.Add(e.PropertyName);

        form.Name = "";

        Assert.Equal([nameof(Ex050_InputValidation.Name)], announced);
    }

    [Fact]
    public void Announces_A_Property_That_Became_Valid_Again()
    {
        var form = Valid();
        form.Name = "";
        var announced = new List<string?>();
        form.ErrorsChanged += (_, e) => announced.Add(e.PropertyName);

        form.Name = "Ada";

        // The disappearing case. Miss it and the field keeps its red border, and the
        // submit button bound to HasErrors never comes back.
        Assert.Equal([nameof(Ex050_InputValidation.Name)], announced);
    }

    [Fact]
    public void Does_Not_Announce_A_Property_Whose_Error_State_Did_Not_Move()
    {
        var form = Valid();
        var announced = new List<string?>();
        form.ErrorsChanged += (_, e) => announced.Add(e.PropertyName);

        form.Age = 37;

        // Name was valid before and is valid now. Raising for it anyway makes every
        // keystroke re-validate the whole form.
        Assert.DoesNotContain(nameof(Ex050_InputValidation.Name), announced);
    }

    [Fact]
    public void HasErrors_Follows_The_Last_Error_Away()
    {
        var form = Valid();
        form.Name = "";
        form.Age = 200;

        form.Name = "Ada";
        Assert.True(form.HasErrors);

        form.Age = 36;
        Assert.False(form.HasErrors);
    }
}
