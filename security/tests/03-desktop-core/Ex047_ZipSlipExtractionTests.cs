using System.IO;
using System.IO.Compression;
using System.Text;
using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex047_ZipSlipExtractionTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "fewo-sec-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
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

    // Snapshots the parent directory's top-level files, runs the extraction, and
    // returns whatever files are new afterwards - catching any escape path the
    // implementation might resolve to, not only one hand-picked expected path.
    private List<string> ExtractAndFindNewFilesInParent(MemoryStream archive)
    {
        Directory.CreateDirectory(_directory);
        var parent = Path.GetDirectoryName(_directory)!;
        var before = new HashSet<string>(Directory.GetFiles(parent, "*", SearchOption.TopDirectoryOnly));

        Ex047_ZipSlipExtraction.ExtractTo(archive, _directory);

        var after = Directory.GetFiles(parent, "*", SearchOption.TopDirectoryOnly);
        var newFiles = after.Where(f => !before.Contains(f)).ToList();

        // Clean up defensively: if the implementation under test really did leak
        // a file into the shared parent (%TEMP% itself), do not leave it behind
        // for the next run to trip over.
        foreach (var leaked in newFiles) File.Delete(leaked);

        return newFiles;
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
        var absoluteTarget = Path.Combine(Path.GetTempPath(), "fewo-sec-absolute-" + Guid.NewGuid().ToString("N") + ".txt");
        using var archive = BuildArchive((absoluteTarget, "escaped-content"));

        try
        {
            Ex047_ZipSlipExtraction.ExtractTo(archive, _directory);

            Assert.False(File.Exists(absoluteTarget), "an absolute-path entry must not be written");
        }
        finally
        {
            if (File.Exists(absoluteTarget)) File.Delete(absoluteTarget);
        }
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
