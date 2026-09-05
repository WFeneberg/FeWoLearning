using System.IO;
using System.Windows;
using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex056_DragDropUntrustedPayloadTests : IDisposable
{
    private static readonly IReadOnlyCollection<string> AllowedExtensions = new[] { ".txt", ".png" };

    private readonly string _root =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public Ex056_DragDropUntrustedPayloadTests()
    {
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    [WpfFact]
    public void Attack_A_Dot_Exe_File_Yields_An_Empty_List()
    {
        var exe = MakeFile("payload.exe");
        var data = new DataObject(DataFormats.FileDrop, new[] { exe });

        var result = Ex056_DragDropUntrustedPayload.AcceptableFiles(data, _root, AllowedExtensions);

        Assert.Empty(result);
    }

    [WpfFact]
    public void Attack_A_Path_Outside_The_Allowed_Root_Yields_An_Empty_List()
    {
        var outsideRoot = Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outsideRoot);
        try
        {
            var outsideFile = Path.Combine(outsideRoot, "sneaky.txt");
            File.WriteAllText(outsideFile, "content");

            var data = new DataObject(DataFormats.FileDrop, new[] { outsideFile });

            var result = Ex056_DragDropUntrustedPayload.AcceptableFiles(data, _root, AllowedExtensions);

            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(outsideRoot, recursive: true);
        }
    }

    [WpfFact]
    public void Attack_No_FileDrop_Format_Yields_An_Empty_List_Rather_Than_Throwing()
    {
        var data = new DataObject(DataFormats.Text, "just some pasted text, no files at all");

        var result = Ex056_DragDropUntrustedPayload.AcceptableFiles(data, _root, AllowedExtensions);

        Assert.Empty(result);
    }

    [WpfFact]
    public void Use_Two_Allowed_Files_Are_Returned_In_Order()
    {
        var first = MakeFile("a.txt");
        var second = MakeFile("b.png");
        var data = new DataObject(DataFormats.FileDrop, new[] { first, second });

        var result = Ex056_DragDropUntrustedPayload.AcceptableFiles(data, _root, AllowedExtensions);

        Assert.Equal(new[] { Path.GetFullPath(first), Path.GetFullPath(second) }, result);
    }

    [WpfFact]
    public void Use_A_Mixed_Drop_Returns_Only_The_Allowed_Files()
    {
        var disallowed = MakeFile("bad.exe");
        var allowed = MakeFile("ok.txt");
        var data = new DataObject(DataFormats.FileDrop, new[] { disallowed, allowed });

        var result = Ex056_DragDropUntrustedPayload.AcceptableFiles(data, _root, AllowedExtensions);

        Assert.Equal(new[] { Path.GetFullPath(allowed) }, result);
    }

    private string MakeFile(string relativePath)
    {
        var full = Path.Combine(_root, relativePath);
        File.WriteAllText(full, "content");
        return full;
    }
}
