using System.IO;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A throwaway directory for the rows whose subject is a real file on disk - rolling
/// logs, support bundles, offline buffers. Created on construction, deleted on
/// disposal, uniquely named so a serial run never collides with a leftover.
///
/// Deletion is best-effort: a sink that has not been closed yet still holds its file
/// open, and failing the test on that would obscure whatever the row was actually
/// about. If a directory survives, it survives in the OS temp path.
/// </summary>
public sealed class ScratchDirectory : IDisposable
{
    public ScratchDirectory()
    {
        Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            "fewolearning-telemetry",
            Guid.NewGuid().ToString("n"));

        Directory.CreateDirectory(Path);
    }

    /// <summary>The directory itself. It exists for as long as this object does.</summary>
    public string Path { get; }

    /// <summary>A path inside the directory. The file is not created.</summary>
    public string File(string name) => System.IO.Path.Combine(Path, name);

    /// <summary>The files currently in the directory, in name order.</summary>
    public IReadOnlyList<string> Files() =>
        Directory.GetFiles(Path).OrderBy(f => f, StringComparer.Ordinal).ToArray();

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch (Exception)
        {
            // Best effort - see the class comment.
        }
    }
}
