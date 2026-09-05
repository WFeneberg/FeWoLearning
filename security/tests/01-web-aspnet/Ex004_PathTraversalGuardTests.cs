using System.IO;
using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex004_PathTraversalGuardTests : IDisposable
{
    private readonly string _root;

    public Ex004_PathTraversalGuardTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "fewolearning-ex004-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(Path.Combine(_root, "subdir"));
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Theory]
    [InlineData("../secrets.txt")]
    [InlineData("..\\secrets.txt")]
    [InlineData("C:\\Windows\\win.ini")]
    [InlineData("subdir/../../outside.txt")]
    [InlineData("a/b/../../../secrets.txt")] // escapes only once "." / ".." are collapsed
    public void Attack_A_Path_That_Escapes_The_Root_Is_Rejected(string requestedPath)
    {
        var resolved = Ex004_PathTraversalGuard.TryResolve(_root, requestedPath, out var fullPath);

        Assert.False(resolved);
        Assert.Equal("", fullPath);
    }

    [Theory]
    [InlineData("report.txt")]
    [InlineData("subdir/report.txt")]
    public void Use_A_Path_Inside_The_Root_Resolves_To_A_Path_Under_It(string requestedPath)
    {
        var resolved = Ex004_PathTraversalGuard.TryResolve(_root, requestedPath, out var fullPath);

        Assert.True(resolved);
        Assert.Equal(Path.GetFullPath(Path.Combine(_root, requestedPath)), fullPath);
        Assert.StartsWith(Path.GetFullPath(_root), fullPath);
    }
}
