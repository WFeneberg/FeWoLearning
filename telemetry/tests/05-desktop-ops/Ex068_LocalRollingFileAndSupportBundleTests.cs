using System.IO;
using System.IO.Compression;
using System.Text.Json;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex068_LocalRollingFileAndSupportBundleTests
{
    private const string AppVersion = "3.4.1";
    private const string SessionId = "5f6b1c0e-0f2d-4a0b-9c8e-1d2e3f4a5b6c";
    private const string InstallationId = "9a8b7c6d-5e4f-4312-a1b0-c9d8e7f6a5b4";

    private static ZipArchive Bundle(ScratchDirectory scratch, params string[] logNames)
    {
        foreach (var name in logNames)
            File.WriteAllText(scratch.File(name), $"a line from {name}\n");

        var bundlePath = Path.Combine(Path.GetTempPath(), $"bundle-{Guid.NewGuid():n}.zip");

        Ex068_LocalRollingFileAndSupportBundle.CreateBundle(
            scratch.Path, bundlePath, AppVersion, SessionId, InstallationId);

        // Read it into memory and delete the file, so the archive outlives the temp path
        // and no test leaves a zip behind.
        var bytes = File.ReadAllBytes(bundlePath);
        File.Delete(bundlePath);

        return new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
    }

    private static JsonElement Manifest(ZipArchive archive)
    {
        var entry = archive.GetEntry(Ex068_LocalRollingFileAndSupportBundle.ManifestEntryName);
        Assert.NotNull(entry);

        using var stream = entry.Open();
        return JsonDocument.Parse(stream).RootElement.Clone();
    }

    [Fact]
    public void The_bundle_is_one_file_carrying_every_log()
    {
        using var scratch = new ScratchDirectory();
        using var archive = Bundle(scratch, "app-20260904.log", "app-20260905.log");

        Assert.Equal(
            new[] { "app-20260904.log", "app-20260905.log", "manifest.json" },
            archive.Entries.Select(e => e.FullName).OrderBy(n => n, StringComparer.Ordinal));

        var first = archive.GetEntry("app-20260904.log");
        Assert.NotNull(first);
        using var reader = new StreamReader(first.Open());
        Assert.Contains("a line from app-20260904.log", reader.ReadToEnd());
    }

    [Fact]
    public void Adversarial_A_The_entry_names_give_nothing_away()
    {
        // The half nobody checks. Every log line can be immaculate and the archive's table
        // of contents still reads C:\Users\<the user>\AppData\... - row 066's problem
        // arriving through a door that is not the logger.
        //
        // The scratch directory is under the real temp path, which on Windows contains the
        // real user name, so a zip built from full paths fails this for real rather than
        // by construction.
        using var scratch = new ScratchDirectory();
        using var archive = Bundle(scratch, "app.log");

        Assert.All(archive.Entries, entry =>
        {
            Assert.DoesNotContain('/', entry.FullName);
            Assert.DoesNotContain('\\', entry.FullName);
            Assert.DoesNotContain(':', entry.FullName);
            Assert.DoesNotContain(
                Environment.UserName, entry.FullName, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void Adversarial_B_Only_the_logs_are_collected()
    {
        // The folder an application writes its logs into holds other things - a settings
        // file, a cached token, a crash dump with the document open in it. "Zip the folder"
        // is one line shorter and ships whatever happens to be next to the logs.
        using var scratch = new ScratchDirectory();
        File.WriteAllText(scratch.File("credentials.json"), """{"token":"hunter2"}""");
        File.WriteAllText(scratch.File("last-document.txt"), "the user's novel");

        using var archive = Bundle(scratch, "app.log");

        Assert.Equal(
            new[] { "app.log", "manifest.json" },
            archive.Entries.Select(e => e.FullName).OrderBy(n => n, StringComparer.Ordinal));
    }

    [Fact]
    public void Adversarial_C_A_folder_with_no_logs_still_produces_a_bundle()
    {
        // Three in the morning, the logging itself is what broke, and the user gets
        // "it didn't work" instead of a file. The manifest alone is still worth having:
        // it says which version and which installation had nothing to say.
        using var scratch = new ScratchDirectory();
        using var archive = Bundle(scratch);

        var entry = Assert.Single(archive.Entries);
        Assert.Equal(Ex068_LocalRollingFileAndSupportBundle.ManifestEntryName, entry.FullName);
    }

    [Fact]
    public void The_manifest_says_what_the_logs_are()
    {
        // Without it you have log files and no idea which build wrote them, which run they
        // came from, or whether this installation reported last week - row 067's two ids
        // with no way to read them.
        using var scratch = new ScratchDirectory();
        using var archive = Bundle(scratch, "app.log");

        var manifest = Manifest(archive);

        Assert.Equal(AppVersion, manifest.GetProperty("appVersion").GetString());
        Assert.Equal(SessionId, manifest.GetProperty("sessionId").GetString());
        Assert.Equal(InstallationId, manifest.GetProperty("installationId").GetString());
    }
}
