using System.Reflection;
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
    /// The value this harness puts in <c>ASPIRE_CONTAINER_RUNTIME</c>, and the single
    /// biggest thing about it.
    ///
    /// It names no real runtime, so Aspire's publish pipeline skips container-runtime
    /// detection entirely instead of shelling out to <c>podman</c>, <c>docker version</c>
    /// and <c>docker container ls</c>. Measured on 13.5.3: one publish drops from ~5.1 s
    /// to ~0.1 s and the manifest is identical, because the manifest is written at ~36 ms
    /// and everything after it was the probe. Nothing at L2 needs a container runtime -
    /// L2 asserts on generated ARTIFACTS - so this is the correct default rather than a
    /// shortcut, and it is also what makes the claim "the default `dotnet test` needs no
    /// daemon" true rather than nearly true. A future row that genuinely needs image
    /// building can pass <c>containerRuntime: "docker"</c> and pay the ~1 s probe.
    /// </summary>
    public const string NoContainerRuntime = "none";

    /// <summary>
    /// Publishes in-process and returns the parsed aspire-manifest.json. The caller owns
    /// the returned <see cref="JsonDocument"/> and should <c>using</c> it; the publish
    /// output behind it is SHARED (see <see cref="SharedPublishAsync"/>) and is deleted
    /// once, after the last test in the assembly.
    /// </summary>
    public static async Task<JsonDocument> GenerateAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default,
        string? containerRuntime = NoContainerRuntime)
    {
        var output = await SharedPublishAsync(configure, cancellationToken, containerRuntime);

        // A FRESH document per call, parsed from the shared directory's text. That is
        // what keeps the existing `using var manifest = await GenerateAsync(...)` at
        // every call site correct: the caller disposes its own document, and the shared
        // publish behind it is untouched.
        return JsonDocument.Parse(output.ReadText(PublishOutput.ManifestFileName));
    }

    /// <summary>
    /// ONE publish per distinct model, shared by every fact that asks for it.
    ///
    /// A publish costs a fixed amount regardless of how big the model is - measured: an
    /// empty model and a five-database one cost the same - so the only quantity worth
    /// managing at L2 is the NUMBER of publishes. This makes that number the number of
    /// distinct models rather than the number of facts, which matters most for the rows
    /// that assert on several generated files from one graph.
    ///
    /// The key is the delegate's identity - its method and its target - plus the
    /// container-runtime choice. A method group such as <c>Ex013_X.Configure</c>
    /// therefore shares across facts and across classes; two separately-created closures
    /// never do, even when they would build the same model, because their targets differ.
    /// That is the conservative direction. The contract is that a model must be a pure
    /// function of the delegate, and a closure over mutable state simply misses the cache
    /// instead of silently handing back somebody else's manifest.
    ///
    /// The returned object is owned by the HARNESS, not by the caller. Do NOT dispose it;
    /// <see cref="HarnessLifetime"/> deletes every shared output after the last test. Use
    /// <see cref="PublishAsync"/> when a test needs an output of its very own -
    /// <see cref="PublishOutput.Dispose"/> is precisely what a shared output must never
    /// have called on it.
    /// </summary>
    public static async Task<PublishOutput> SharedPublishAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default,
        string? containerRuntime = NoContainerRuntime)
    {
        var key = (configure.Method, configure.Target, containerRuntime);

        // The assembly runs serially (TestParallelism.cs), so this lock is never
        // contended. It is here so the "at most one publish per model" invariant belongs
        // to this method rather than to the runner's configuration.
        await SharedLock.WaitAsync(cancellationToken);
        try
        {
            if (Shared.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var output = await PublishAsync(configure, cancellationToken, containerRuntime);
            Shared[key] = output;
            return output;
        }
        finally
        {
            SharedLock.Release();
        }
    }

    private static readonly SemaphoreSlim SharedLock = new(1, 1);

    private static readonly Dictionary<(MethodInfo, object?, string?), PublishOutput> Shared = [];

    /// <summary>
    /// How many publishes this assembly has actually paid for. The smoke tests use it to
    /// prove that a second fact on the same model costs none.
    /// </summary>
    internal static int PublishCount { get; private set; }

    /// <summary>
    /// Deletes every shared publish output. Called once, after the last test in the
    /// assembly, by <see cref="HarnessLifetime"/>. The hourly sweep in the static
    /// constructor remains the backstop for a process that never gets here.
    /// </summary>
    public static void DisposeSharedPublishes()
    {
        foreach (var output in Shared.Values)
        {
            output.Dispose();
        }

        Shared.Clear();
    }

    /// <summary>
    /// Publishes in-process, UNSHARED, and returns the generated artifacts. ALWAYS
    /// dispose the result (`using var output = await ...`) - Dispose deletes the output
    /// directory. Prefer <see cref="SharedPublishAsync"/> unless a test needs an output
    /// nobody else can see.
    /// </summary>
    public static async Task<PublishOutput> PublishAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default,
        string? containerRuntime = NoContainerRuntime)
    {
        Directory.CreateDirectory(Root);
        var dir = Path.Combine(Root, Guid.NewGuid().ToString("N")[..12]);
        PublishCount++;
        try
        {
            var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
            {
                Args = ["--operation", "publish", "--output-path", dir],
                DisableDashboard = true
            });

            // Set on THIS builder's configuration, never on the process environment, so
            // it cannot reach ContainerHarness - which runs in RUN mode and needs a real
            // Docker. See NoContainerRuntime for what it buys.
            if (containerRuntime is not null)
            {
                builder.Configuration["ASPIRE_CONTAINER_RUNTIME"] = containerRuntime;
            }

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
