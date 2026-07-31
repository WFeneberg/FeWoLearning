using System.Linq;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex097_RoslynAnalyzerTests
{
    // Line numbers below are 1-based positions *within this string literal*
    // (line 1 is "public class Sample", the first line of the verbatim string).
    private const string ViolatingSource =
@"public class Sample
{
    public int Bad(int x)
    {
        if (x > 0)
        {
            return x;
        }
    }

    public int AlsoBad(int x)
    {
        if (x > 0)
        {
            return x;
        }
        else
        {
            x = x + 1;
        }
    }

    public void Ok(int x)
    {
        System.Console.WriteLine(x);
    }
}";

    private const string CompliantSource =
@"public class Sample
{
    public int Good(int x)
    {
        if (x > 0)
        {
            return x;
        }
        else
        {
            return -x;
        }
    }

    public int AlsoGood(int x)
    {
        if (x > 0)
        {
            return x;
        }

        return 0;
    }

    public void Ok(int x)
    {
        System.Console.WriteLine(x);
    }
}";

    [Fact]
    public void FlagsMethodsWithMissingReturnPaths()
    {
        var diagnostics = RoslynAnalyzer.AnalyzeMissingReturns(ViolatingSource);
        var names = diagnostics.Select(d => d.MethodName).ToList();

        Assert.Equal(2, diagnostics.Count);
        Assert.Contains("Bad", names);
        Assert.Contains("AlsoBad", names);
        Assert.DoesNotContain("Ok", names);
        Assert.All(diagnostics, d => Assert.Contains("does not return a value on all code paths", d.Message));
    }

    [Fact]
    public void ReportsCorrectLineNumberForFlaggedMethod()
    {
        var diagnostics = RoslynAnalyzer.AnalyzeMissingReturns(ViolatingSource);

        var bad = diagnostics.Single(d => d.MethodName == "Bad");
        Assert.Equal(3, bad.Line);

        var alsoBad = diagnostics.Single(d => d.MethodName == "AlsoBad");
        Assert.Equal(11, alsoBad.Line);
    }

    [Fact]
    public void ReportsNoDiagnosticsForCompliantCode()
    {
        var diagnostics = RoslynAnalyzer.AnalyzeMissingReturns(CompliantSource);
        Assert.Empty(diagnostics);
    }
}
