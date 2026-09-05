using System.Diagnostics;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex049_ProcessArgumentInjectionTests
{
    [Fact]
    public void Attack_A_Shell_Metacharacter_Payload_Stays_One_ArgumentList_Entry_And_Arguments_Is_Empty()
    {
        const string payload = "\" & del /q *";

        var startInfo = Ex049_ProcessArgumentInjection.BuildStartInfo("notepad.exe", [payload]);

        Assert.Single(startInfo.ArgumentList);
        Assert.Equal(payload, startInfo.ArgumentList[0]);
        Assert.Equal(string.Empty, startInfo.Arguments);
    }

    [Fact]
    public void Attack_UseShellExecute_Is_False()
    {
        var startInfo = Ex049_ProcessArgumentInjection.BuildStartInfo("notepad.exe", ["one"]);

        Assert.False(startInfo.UseShellExecute);
    }

    [Fact]
    public void Attack_An_Argument_Containing_A_Newline_Stays_One_Entry()
    {
        const string payload = "line-one\nline-two";

        var startInfo = Ex049_ProcessArgumentInjection.BuildStartInfo("notepad.exe", [payload]);

        Assert.Single(startInfo.ArgumentList);
        Assert.Equal(payload, startInfo.ArgumentList[0]);
    }

    [Fact]
    public void Use_Three_Ordinary_Arguments_Appear_In_Order_Verbatim()
    {
        var arguments = new[] { "--input", "C:\\data\\file.csv", "--verbose" };

        var startInfo = Ex049_ProcessArgumentInjection.BuildStartInfo("tool.exe", arguments);

        Assert.Equal(3, startInfo.ArgumentList.Count);
        Assert.Equal(arguments[0], startInfo.ArgumentList[0]);
        Assert.Equal(arguments[1], startInfo.ArgumentList[1]);
        Assert.Equal(arguments[2], startInfo.ArgumentList[2]);
    }

    [Fact]
    public void Use_FileName_Equals_The_Executable_Passed_In()
    {
        var startInfo = Ex049_ProcessArgumentInjection.BuildStartInfo("C:\\tools\\tool.exe", ["a"]);

        Assert.Equal("C:\\tools\\tool.exe", startInfo.FileName);
    }
}
