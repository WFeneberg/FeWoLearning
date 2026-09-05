using System.IO;
using System.IO.Compression;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex047_ZipSlipExtractionTests : IDisposable
{
    // Two levels, deliberately. The escape checks below work by watching the
    // extraction root's PARENT for files that appear during extraction, so that
    // parent must be a directory this test owns outright - never %TEMP% itself,
    // whose contents belong to every other process on the machine.
    private readonly string _sandbox =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    private readonly string _directory;

    public Ex047_ZipSlipExtractionTests()
    {
        _directory = Path.Combine(_sandbox, "root");
    }

    public void Dispose()
    {
        if (Directory.Exists(_sandbox)) Directory.Delete(_sandbox, recursive: true);
    }

    private static MemoryStream BuildArchive(params (string Name, string Content)[] entries)
    {
        var stream = new MemoryStream();
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                var entry = zip.CreateEntry(name);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                writer.Write(content);
            }
        }

        stream.Position = 0;
        return stream;
    }

    // Snapshots the extraction root's parent - this test's own private sandbox
    // directory, never %TEMP% - runs the extraction, and returns whatever files
    // are new afterwards. Watching the whole parent, rather than one hand-picked
    // expected path, catches any escape path the implementation resolves to; and
    // because the parent is private, "a file appeared here" can only mean this
    // extraction put it there. Nothing is deleted individually: Dispose removes
    // the sandbox whole, so the test can never touch a file it did not create.
    private List<string> ExtractAndFindNewFilesInParent(MemoryStream archive)
    {
        Directory.CreateDirectory(_directory);
        var parent = Path.GetDirectoryName(_directory)!;
        var before = new HashSet<string>(Directory.GetFiles(parent, "*", SearchOption.TopDirectoryOnly));

        Ex047_ZipSlipExtraction.ExtractTo(archive, _directory);

        var after = Directory.GetFiles(parent, "*", SearchOption.TopDirectoryOnly);
        return after.Where(f => !before.Contains(f)).ToList();
    }

    [Fact]
    public void Attack_A_Parent_Relative_Entry_Is_Not_Written_Anywhere_Outside_The_Destination()
    {
        using var archive = BuildArchive(("../escaped.txt", "escaped-content"));

        var newFiles = ExtractAndFindNewFilesInParent(archive);

        Assert.Empty(newFiles);
    }

    [Fact]
    public void Attack_An_Absolute_Path_Entry_Is_Not_Written()
    {
        Directory.CreateDirectory(_directory);

        // An absolute path that is outside the destination but still inside this
        // test's own sandbox: it exercises the rooted-entry escape exactly the
        // same way an absolute path into %TEMP% would, and if the implementation
        // under test really does write it, Dispose is what removes it.
        var absoluteTarget = Path.Combine(_sandbox, "absolute-escape.txt");
        using var archive = BuildArchive((absoluteTarget, "escaped-content"));

        Ex047_ZipSlipExtraction.ExtractTo(archive, _directory);

        Assert.False(File.Exists(absoluteTarget), "an absolute-path entry must not be written");
    }

    [Fact]
    public void Attack_A_Sub_Then_Double_Dot_Entry_Is_Not_Written()
    {
        using var archive = BuildArchive(("sub/../../escaped.txt", "escaped-content"));

        var newFiles = ExtractAndFindNewFilesInParent(archive);

        Assert.Empty(newFiles);
    }

    [Fact]
    public void Use_Ordinary_Entries_Are_Written_With_Correct_Content_And_Named_Exactly_In_The_Result()
    {
        Directory.CreateDirectory(_directory);
        using var archive = BuildArchive(("a.txt", "content-a"), ("sub/b.txt", "content-b"));

        var written = Ex047_ZipSlipExtraction.ExtractTo(archive, _directory);

        var expectedA = Path.Combine(_directory, "a.txt");
        var expectedB = Path.Combine(_directory, "sub", "b.txt");

        Assert.True(File.Exists(expectedA));
        Assert.True(File.Exists(expectedB));
        Assert.Equal("content-a", File.ReadAllText(expectedA));
        Assert.Equal("content-b", File.ReadAllText(expectedB));

        Assert.Equal(2, written.Count);
        Assert.Contains(written, p => Path.GetFullPath(p) == Path.GetFullPath(expectedA));
        Assert.Contains(written, p => Path.GetFullPath(p) == Path.GetFullPath(expectedB));
    }
}
