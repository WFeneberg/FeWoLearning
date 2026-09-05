using FeWoLearning.Security.Exercises.WebBlazor;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex032_JsInteropInjectionTests
{
    private const string MaliciousInput = "'); alert(1); ('";
    private const string DocumentedFunctionName = "fewoLearning.showToast";

    [Fact]
    public void Attack_Identifier_Never_Contains_Any_Part_Of_The_Malicious_User_Input()
    {
        var (identifier, _) = Ex032_JsInteropInjection.BuildCall(MaliciousInput);

        Assert.DoesNotContain(MaliciousInput, identifier);
        Assert.DoesNotContain("alert", identifier, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("ok")]
    [InlineData(MaliciousInput)]
    public void Attack_Identifier_Is_Never_Eval(string userInput)
    {
        var (identifier, _) = Ex032_JsInteropInjection.BuildCall(userInput);

        Assert.NotEqual("eval", identifier, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Args_Contains_The_User_Input_Verbatim_And_Unmodified()
    {
        var (_, args) = Ex032_JsInteropInjection.BuildCall(MaliciousInput);

        Assert.Contains(MaliciousInput, args);
    }

    [Fact]
    public void Use_Identifier_Is_The_Documented_Function_Name_For_A_Benign_Input_Too()
    {
        var (identifier, _) = Ex032_JsInteropInjection.BuildCall("ok");

        Assert.Equal(DocumentedFunctionName, identifier);
    }
}
