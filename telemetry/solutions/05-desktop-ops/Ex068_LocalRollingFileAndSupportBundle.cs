using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 068 — LocalRollingFileAndSupportBundle (desktop-ops).
// Goal:   Make the thing a user can actually send you when the telemetry never left their
//         machine.
// Drills: collecting the retained log files, a manifest, entry names that give nothing
//         away.
// Passes: the bundle contains every log file in the folder, and nothing else from it;
//         it carries a manifest naming the version, the session and the installation;
//         the entry names are BARE FILE NAMES - no absolute path, no user profile;
//         a folder with no logs still produces a valid bundle with just the manifest;
//         and the result is one file the user can attach to an email.
//
// Everything before this row assumed telemetry gets out. On a desktop that assumption
// fails constantly and for reasons you cannot fix: no network, a corporate proxy, a
// firewall, or a user who declined in row 066's dialog and is now asking for help anyway.
// The support bundle is the manual fallback, and it is the only telemetry that works when
// the user has said no - because they are choosing to send it, once, about this.
//
// The third clause is row 066 arriving somewhere nobody thinks to look. A zip built from
// full paths carries <c>C:\Users\ada\AppData\...</c> in every entry NAME, so the user's
// name is in the archive's table of contents whether or not a single log line mentions
// it. Bare names cost nothing and remove the whole category.
//
// The manifest is what makes the bundle answerable. Without it you have log files and no
// idea which version produced them, which run they came from, or whether this is the same
// installation that reported last week - which is to say you have row 067's two ids and
// no way to read them.
//
// The fourth clause is the one that decides whether the feature is usable at three in the
// morning: a bundle that fails when there is nothing to bundle sends the user back with
// "it didn't work" instead of with a file.
public static class Ex068_LocalRollingFileAndSupportBundle
{
    /// <summary>What the manifest is called inside the bundle.</summary>
    public const string ManifestEntryName = "manifest.json";

    /// <summary>The pattern that identifies a log file in the folder.</summary>
    public const string LogFilePattern = "*.log";

    /// <summary>
    /// Build a support bundle at <paramref name="bundlePath"/> from every
    /// <see cref="LogFilePattern"/> file in <paramref name="logDirectory"/>.
    ///
    /// Each log goes in under its BARE FILE NAME. Alongside them goes
    /// <see cref="ManifestEntryName"/>: a JSON object with the members
    /// <c>appVersion</c>, <c>sessionId</c> and <c>installationId</c>.
    ///
    /// A folder with no logs still produces a valid bundle carrying the manifest.
    /// </summary>
    public static void CreateBundle(
        string logDirectory,
        string bundlePath,
        string appVersion,
        string sessionId,
        string installationId)
    {
        using var stream = File.Create(bundlePath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

        // The manifest goes in first and unconditionally, so the empty case needs no
        // special path: a folder with nothing in it still produces a bundle that says
        // which build and which installation had nothing to say.
        var manifest = archive.CreateEntry(ManifestEntryName);

        // Both scopes close before the next entry is created: a ZipArchive in Create mode
        // allows exactly one entry open at a time, and Utf8JsonWriter.Dispose flushes the
        // writer without closing the stream underneath it.
        using (var entry = manifest.Open())
        using (var writer = new Utf8JsonWriter(entry))
        {
            writer.WriteStartObject();
            writer.WriteString("appVersion", appVersion);
            writer.WriteString("sessionId", sessionId);
            writer.WriteString("installationId", installationId);
            writer.WriteEndObject();
        }

        foreach (var log in Directory.GetFiles(logDirectory, LogFilePattern))
        {
            // CreateEntryFromFile with the BARE name. Handing it the full path would put
            // the user's own profile directory in the archive's table of contents - row
            // 066's problem arriving somewhere nobody thinks to look, because it is not in
            // a log line at all.
            archive.CreateEntryFromFile(log, Path.GetFileName(log));
        }
    }
}
