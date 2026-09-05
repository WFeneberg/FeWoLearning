using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex069_GenericConstraintComponentTests : BunitContext
{
    [Fact]
    public void Finds_The_Extremes_Of_A_Number_List()
    {
        var cut = Render<Ex069_GenericConstraintComponent<int>>(p => p.Add(
            c => c.Items, new[] { 3, 9, 1, 7 }));

        Assert.Equal("1", cut.Find("#min").TextContent);
        Assert.Equal("9", cut.Find("#max").TextContent);
    }

    // A different T, ordered by IComparable rather than by anything numeric.
    [Fact]
    public void Finds_The_Extremes_Of_A_String_List()
    {
        var cut = Render<Ex069_GenericConstraintComponent<string>>(p => p.Add(
            c => c.Items, new[] { "pear", "apple", "quince" }));

        Assert.Equal("apple", cut.Find("#min").TextContent);
        Assert.Equal("quince", cut.Find("#max").TextContent);
    }

    [Fact]
    public void An_Empty_List_Has_No_Extremes()
    {
        var cut = Render<Ex069_GenericConstraintComponent<string>>(p => p.Add(
            c => c.Items, Array.Empty<string>()));

        Assert.Equal("", cut.Find("#min").TextContent);
        Assert.Equal("", cut.Find("#max").TextContent);
    }

    // The constraint is the row's actual subject, and behaviour cannot prove it:
    // LINQ's Min()/Max() need no constraint and would satisfy every fact above. So
    // this one reads the type parameter's metadata instead - which is also why it is
    // the single fact in this track that goes red on an assertion rather than on the
    // exercise's NotImplementedException (see README §11).
    [Fact]
    public void The_Type_Parameter_Is_Constrained_To_IComparable_Of_Itself()
    {
        var parameter = typeof(Ex069_GenericConstraintComponent<>).GetGenericArguments()[0];

        Assert.Contains(
            parameter.GetGenericParameterConstraints(),
            constraint => constraint.IsGenericType
                && constraint.GetGenericTypeDefinition() == typeof(IComparable<>)
                && constraint.GetGenericArguments()[0] == parameter);
    }
}
