using System.Text.Json;
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Runs Aspire's publish operation in-process and hands back what it wrote.
///
/// Do NOT shell out to `aspire publish`: it writes its artifacts and then does not
/// exit in a non-interactive shell, dropping into "press CTRL+C to stop the AppHost".
/// Measured still running at 600 s.
///
/// Two entry points:
/// <list type="bullet">
/// <item><see cref="GenerateAsync"/> - the manifest alone (~3.7 s). Per resource it
/// carries type (container.v0 / value.v0 / parameter.v0 / azure.bicep.v0), the pinned
/// image, the full env map including ConnectionStrings__*, bindings with targetPort,
/// and the generated-secret policy.</item>
/// <item><see cref="PublishAsync"/> - the whole output directory, kept alive for the
/// lifetime of the returned <see cref="PublishOutput"/>. The manifest is NOT the only
/// in-process artifact: a model carrying Azure resources also writes real Bicep
/// (`aca.module.bicep`, `storage.module.bicep`, per-resource `.bicep`) in ~7.5 s, which
/// is what the Azure rows (093, 094, 099, 100) assert against.</item>
/// </list>
///
/// Docker Compose YAML is the single exception - it is emitted by a pipeline the CLI
/// drives and cannot be obtained in-process, so compose rows grade against a committed
/// golden file instead. See MicroServices/README.md sections 4 and 6.
/// </summary>
public static class ManifestHarness
{
    /// <summary>
    /// The single root every publish-shaped output lives under, so the stale-output
    /// sweep in the static constructor can reach all of it. <c>internal</c> rather than
    /// private because <see cref="ModelHarness.BuildForPublish"/> also constructs a
    /// publish-mode builder and must not point its output path somewhere unswept.
    /// Reading it runs this type's static constructor, and hence the sweep - guaranteed,
    /// because an explicit static constructor makes the type not <c>beforefieldinit</c>.
    /// </summary>
    internal static readonly string Root = Path.Combine(Path.GetTempPath(), "fewo-ms-publish");

    static ManifestHarness() => SweepStaleOutputs();

    /// <summary>
    /// Publishes in-process and returns only the parsed aspire-manifest.json. The output
    /// directory is deleted before this returns; the JsonDocument outlives it.
    /// </summary>
    public static async Task<JsonDocument> GenerateAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        using var output = await PublishAsync(configure, cancellationToken);
        return JsonDocument.Parse(output.ReadText(PublishOutput.ManifestFileName));
    }

    /// <summary>
    /// Publishes in-process and returns the generated artifacts. ALWAYS dispose the
    /// result (`using var output = await ...`) - Dispose deletes the output directory.
    /// </summary>
    public static async Task<PublishOutput> PublishAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Root);
        var dir = Path.Combine(Root, Guid.NewGuid().ToString("N")[..12]);
        try
        {
            var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
            {
                Args = ["--operation", "publish", "--output-path", dir],
                DisableDashboard = true
            });
            configure(builder);
            using var app = builder.Build();

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(120));
            await app.RunAsync(timeout.Token);

            if (!File.Exists(Path.Combine(dir, PublishOutput.ManifestFileName)))
            {
                throw new InvalidOperationException(
                    "Publish produced no manifest. Files present: " +
                    (Directory.Exists(dir)
                        ? string.Join(", ", EnumerateRelative(dir))
                        : "<no directory>"));
            }

            return new PublishOutput(dir);
        }
        catch
        {
            TryDelete(dir);
            throw;
        }
    }

    internal static IEnumerable<string> EnumerateRelative(string dir)
        => Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                    .Select(f => Path.GetRelativePath(dir, f).Replace(Path.DirectorySeparatorChar, '/'))
                    .Order();

    internal static void TryDelete(string dir)
    {
        try
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
        }
        catch (IOException) { /* a leftover handle must not fail a test */ }
        catch (UnauthorizedAccessException) { }
    }

    /// <summary>
    /// Safety net for the one way this harness could leak: a test that forgets to
    /// dispose a <see cref="PublishOutput"/>, or a process killed mid-run. Every output
    /// lives under one root, and anything in it older than an hour is swept on first use.
    /// </summary>
    private static void SweepStaleOutputs()
    {
        try
        {
            if (!Directory.Exists(Root)) return;
            var cutoff = DateTime.UtcNow - TimeSpan.FromHours(1);
            foreach (var d in Directory.EnumerateDirectories(Root))
            {
                if (Directory.GetLastWriteTimeUtc(d) < cutoff) TryDelete(d);
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

/// <summary>
/// The artifacts one in-process publish wrote. Dispose deletes them.
/// </summary>
public sealed class PublishOutput : IDisposable
{
    internal const string ManifestFileName = "aspire-manifest.json";

    private JsonDocument? _manifest;
    private bool _disposed;

    internal PublishOutput(string directory) => Directory = directory;

    /// <summary>The publish output directory. Valid until this object is disposed.</summary>
    public string Directory { get; }

    /// <summary>Every generated file, as forward-slashed paths relative to <see cref="Directory"/>.</summary>
    public IReadOnlyList<string> Files => ManifestHarness.EnumerateRelative(Directory).ToList();

    /// <summary>The parsed aspire-manifest.json. Owned by this object.</summary>
    public JsonDocument Manifest => _manifest ??= JsonDocument.Parse(ReadText(ManifestFileName));

    public bool Has(string relativePath) => File.Exists(Resolve(relativePath));

    /// <summary>
    /// The text of one generated file, with a failure message that lists what WAS written -
    /// the useful message when an Azure row asserts on a .bicep name that moved.
    /// </summary>
    public string ReadText(string relativePath)
    {
        var path = Resolve(relativePath);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Publish wrote no '{relativePath}'. Files present: " +
                string.Join(", ", Files), path);
        }
        return File.ReadAllText(path);
    }

    /// <summary>Every generated Bicep file, relative-path → contents.</summary>
    public IReadOnlyDictionary<string, string> BicepFiles =>
        Files.Where(f => f.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase))
             .ToDictionary(f => f, ReadText);

    private string Resolve(string relativePath)
        => Path.GetFullPath(Path.Combine(Directory, relativePath));

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _manifest?.Dispose();
        ManifestHarness.TryDelete(Directory);
    }
}
