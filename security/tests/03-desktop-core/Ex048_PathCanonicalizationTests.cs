using System.IO;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex048_PathCanonicalizationTests : IDisposable
{
    private readonly string _root =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public Ex048_PathCanonicalizationTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void Attack_A_Sibling_Whose_Name_Merely_Starts_With_The_Roots_Name_Is_Not_Inside()
    {
        var sibling = _root + "-evil";
        var candidate = Path.Combine(sibling, "file.txt");

        Assert.False(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Attack_A_Dot_Dot_Escaping_Path_Is_Not_Inside()
    {
        var candidate = Path.Combine(_root, "..", "outside", "file.txt");

        Assert.False(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Attack_A_Unc_Path_Is_Not_Inside()
    {
        const string candidate = @"\\localhost\C$\somewhere\file.txt";

        Assert.False(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Attack_A_Device_Path_Escaping_The_Root_Is_Not_Inside()
    {
        var candidate = $@"\\?\{Path.GetPathRoot(_root)}other\file.txt";

        Assert.False(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Attack_A_Path_With_A_Trailing_Alternate_Data_Stream_Is_Not_Inside()
    {
        var candidate = Path.Combine(_root, "file.txt") + ":hidden";

        Assert.False(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Use_The_Root_Itself_Is_Inside()
    {
        Assert.True(Ex048_PathCanonicalization.IsInside(_root, _root));
    }

    [Fact]
    public void Use_A_Nested_File_Is_Inside()
    {
        var candidate = Path.Combine(_root, "nested", "file.txt");

        Assert.True(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }

    [Fact]
    public void Use_A_Nested_Path_Written_With_Forward_Slashes_Is_Inside()
    {
        var candidate = _root.Replace('\\', '/') + "/nested/file.txt";

        Assert.True(Ex048_PathCanonicalization.IsInside(_root, candidate));
    }
}
