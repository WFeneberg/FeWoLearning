using System.IO;
using FeWoLearning.Security.Exercises.DesktopWpf;

namespace FeWoLearning.Security.Tests.DesktopWpf;

public class Ex060_FilePickerResultStillUntrustedTests : IDisposable
{
    private readonly string _root =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    private readonly string _outside =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public Ex060_FilePickerResultStillUntrustedTests()
    {
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(_outside);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
        if (Directory.Exists(_outside)) Directory.Delete(_outside, recursive: true);
    }

    [WpfFact]
    public void Attack_A_Path_Outside_The_Allowed_Root_Is_Rejected_Even_Though_A_Dialog_Produced_It()
    {
        var outsideFile = Path.Combine(_outside, "sneaky.txt");
        File.WriteAllBytes(outsideFile, new byte[] { 1, 2, 3 });

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            outsideFile, _root, maxBytes: 1024, out var rejection);

        Assert.False(accepted);
        Assert.False(string.IsNullOrEmpty(rejection));
    }

    [WpfFact]
    public void Attack_A_Symbolic_Link_Resolving_Outside_The_Root_Is_Rejected()
    {
        var target = Path.Combine(_outside, "real-target.txt");
        File.WriteAllBytes(target, new byte[] { 1, 2, 3 });
        var link = Path.Combine(_root, "innocent-looking.txt");

        try
        {
            File.CreateSymbolicLink(link, target);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Assert.Skip(
                "Creating a symbolic link needs elevation or Developer Mode on this machine "
                + $"({ex.GetType().Name}: {ex.Message}). See security/README.md.");
            return;
        }

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            link, _root, maxBytes: 1024, out var rejection);

        Assert.False(accepted);
        Assert.False(string.IsNullOrEmpty(rejection));
    }

    [WpfFact]
    public void Attack_A_File_Exceeding_The_Byte_Limit_Is_Rejected()
    {
        var file = Path.Combine(_root, "too-big.bin");
        File.WriteAllBytes(file, new byte[16]);

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            file, _root, maxBytes: 15, out var rejection);

        Assert.False(accepted);
        Assert.False(string.IsNullOrEmpty(rejection));
    }

    [WpfFact]
    public void Attack_A_Path_Naming_A_Directory_Is_Rejected()
    {
        var dir = Path.Combine(_root, "a-directory");
        Directory.CreateDirectory(dir);

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            dir, _root, maxBytes: 1024, out var rejection);

        Assert.False(accepted);
        Assert.False(string.IsNullOrEmpty(rejection));
    }

    [WpfFact]
    public void Use_An_Ordinary_In_Root_File_Under_The_Limit_Is_Accepted()
    {
        var file = Path.Combine(_root, "ok.txt");
        File.WriteAllBytes(file, new byte[8]);

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            file, _root, maxBytes: 1024, out var rejection);

        Assert.True(accepted);
        Assert.Null(rejection);
    }

    [WpfFact]
    public void Use_A_File_Exactly_At_The_Byte_Limit_Is_Accepted()
    {
        var file = Path.Combine(_root, "exact.bin");
        File.WriteAllBytes(file, new byte[16]);

        var accepted = Ex060_FilePickerResultStillUntrusted.TryAcceptPickedPath(
            file, _root, maxBytes: 16, out var rejection);

        Assert.True(accepted);
        Assert.Null(rejection);
    }
}
