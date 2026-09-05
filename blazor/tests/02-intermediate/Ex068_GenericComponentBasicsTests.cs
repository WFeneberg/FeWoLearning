using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

// Rendered through Ex068_GenericComponentBasics, never through the badge directly:
// inference happens at a Razor call site, and a bUnit Render<Badge<string>> would
// state the type argument itself and prove nothing about it.
public class Ex068_GenericComponentBasicsTests : BunitContext
{
    [Fact]
    public void Infers_Int32_From_A_Literal_Value()
    {
        var cut = Render<Ex068_GenericComponentBasics>();

        var badge = cut.Find("#number .badge");
        Assert.Equal("Int32", badge.GetAttribute("data-type"));
        Assert.Equal("42", badge.TextContent.Trim());
    }

    [Fact]
    public void Infers_String_From_A_Parameter_Value()
    {
        var cut = Render<Ex068_GenericComponentBasics>(p => p.Add(c => c.Name, "grace"));

        var badge = cut.Find("#text .badge");
        Assert.Equal("String", badge.GetAttribute("data-type"));
        Assert.Equal("grace", badge.TextContent.Trim());
    }

    // The template's context is typed, not object - the call site calls a string
    // method on it. This also proves the template is used in preference to the
    // value's own text.
    [Fact]
    public void Applies_The_Typed_Template_When_One_Is_Given()
    {
        var cut = Render<Ex068_GenericComponentBasics>(p => p.Add(c => c.Name, "grace"));

        var badge = cut.Find("#templated .badge");
        Assert.Equal("String", badge.GetAttribute("data-type"));
        Assert.Equal("GRACE", cut.Find("#templated .badge b").TextContent);
    }

    // Nothing to infer from, so the call site states T - and a null Value renders as
    // no content rather than as "null" or a crash.
    [Fact]
    public void Uses_The_Explicit_Type_Argument_When_There_Is_No_Value()
    {
        var cut = Render<Ex068_GenericComponentBasics>();

        var badge = cut.Find("#explicit .badge");
        Assert.Equal("String", badge.GetAttribute("data-type"));
        Assert.Equal("", badge.TextContent.Trim());
    }

    // The same call with a value-type T, which has no null: "T?" on an unconstrained
    // T is a nullability annotation, so Value is default(Guid) and prints. Asserted
    // rather than avoided, because it is the surprise this exercise is likely to
    // spring on a reader.
    [Fact]
    public void A_Value_Type_Argument_Falls_Back_To_Default_Not_To_Nothing()
    {
        var cut = Render<Ex068_GenericComponentBasics>();

        var badge = cut.Find("#struct .badge");
        Assert.Equal("Guid", badge.GetAttribute("data-type"));
        Assert.Equal(Guid.Empty.ToString(), badge.TextContent.Trim());
    }
}
