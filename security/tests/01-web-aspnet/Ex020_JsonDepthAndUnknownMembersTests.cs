using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex020_JsonDepthAndUnknownMembersTests
{
    private sealed record Payload(string Name, int Count);

    private sealed record Wrapper(int Value, Wrapper? Inner);

    [Fact]
    public void Attack_A_Deeply_Nested_Array_Fails_Without_A_Value()
    {
        var json = string.Concat(Enumerable.Repeat("[", 200)) + string.Concat(Enumerable.Repeat("]", 200));

        var ok = Ex020_JsonDepthAndUnknownMembers.TryParse<Payload>(json, out var value, out var error);

        Assert.False(ok);
        Assert.Null(value);
        Assert.NotNull(error);
    }

    [Fact]
    public void Attack_A_Member_The_Target_Type_Does_Not_Declare_Fails()
    {
        var json = """{"Name":"widget","Count":3,"IsAdmin":true}""";

        var ok = Ex020_JsonDepthAndUnknownMembers.TryParse<Payload>(json, out var value, out var error);

        Assert.False(ok);
        Assert.Null(value);
        Assert.NotNull(error);
    }

    [Fact]
    public void Attack_The_Failure_Error_Never_Names_The_Target_Type_Or_Stack_Detail()
    {
        var json = """{"Name":"widget","Count":3,"IsAdmin":true}""";

        Ex020_JsonDepthAndUnknownMembers.TryParse<Payload>(json, out _, out var error);

        Assert.NotNull(error);
        Assert.DoesNotContain(typeof(Payload).FullName!, error);
        Assert.DoesNotContain(nameof(Payload), error, StringComparison.Ordinal);
        Assert.DoesNotContain("StackTrace", error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" at ", error, StringComparison.Ordinal);
    }

    [Fact]
    public void Use_A_Well_Formed_Payload_At_Nesting_Depth_Five_Parses_To_A_Correct_Value()
    {
        var json = """
            {"Value":1,"Inner":{"Value":2,"Inner":{"Value":3,"Inner":{"Value":4,"Inner":{"Value":5,"Inner":null}}}}}
            """;

        var ok = Ex020_JsonDepthAndUnknownMembers.TryParse<Wrapper>(json, out var value, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.NotNull(value);
        Assert.Equal(1, value!.Value);
        Assert.Equal(5, value.Inner!.Inner!.Inner!.Inner!.Value);
        Assert.Null(value.Inner.Inner.Inner.Inner.Inner);
    }

    [Fact]
    public void Use_Different_Casing_For_Known_Members_Still_Parses()
    {
        var json = """{"name":"widget","count":7}""";

        var ok = Ex020_JsonDepthAndUnknownMembers.TryParse<Payload>(json, out var value, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("widget", value!.Name);
        Assert.Equal(7, value.Count);
    }
}
