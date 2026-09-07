using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

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
    /// therefore shares across facts and across classes.
    ///
    /// <para><b>THE CONTRACT, and it is sharper than "closures are safe".</b> The model
    /// must be a pure function of the delegate. A <i>closure</i> over mutable state
    /// happens to be harmless - each closure instance is a different <c>Target</c>, so it
    /// misses the cache - but a <b>static</b> <c>Configure</c> that reads <b>static
    /// mutable state</b> is NOT: its key is a stable <c>(MethodInfo, null)</c>, so the
    /// second call HITS and would be handed the first call's manifest. That pattern is
    /// live in this assembly, not hypothetical - <c>tests/_support/TestParallelism.cs</c>
    /// names ex023's two static scenario flags and ex025's static hook log, and there are
    /// sixty rows still to be written against the same freedom.
    /// <b>A static <c>Configure</c> that reads static mutable state must call
    /// <see cref="PublishAsync"/>, which shares nothing, not
    /// <see cref="GenerateAsync"/>.</b></para>
    ///
    /// <para>That rule is <b>enforced</b>, not merely documented. Every cache HIT rebuilds
    /// the model - in publish mode, so that a <c>Configure</c> branching on
    /// <c>IsPublishMode</c> is compared like for like - and fingerprints it against what
    /// was published. A model that changed throws an <see cref="InvalidOperationException"/>
    /// naming the delegate and pointing at <see cref="PublishAsync"/>. The rebuild costs a
    /// model build (~10 ms warm against a ~100 ms publish), and it invokes
    /// <paramref name="configure"/> exactly ONCE per call - which is what it did before
    /// this cache existed, so nothing that was safe before became unsafe.</para>
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
                VerifyUnchanged(configure, existing, containerRuntime);
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

    /// <summary>
    /// The guard behind <see cref="SharedPublishAsync"/>'s contract: rebuild the model
    /// and refuse to hand back a manifest that no longer describes it.
    ///
    /// <para><b>What it catches, exactly.</b> Everything an annotation stores
    /// declaratively: the set of resources and their runtime types, their
    /// connection-string expressions, which annotations each carries, and every
    /// annotation property whose type is a string, a primitive, an enum or an
    /// <see cref="IResource"/>. In practice that is `WithImageTag` / `WithImageRegistry` /
    /// `WithImageSHA256`, endpoint ports and schemes and `IsExternal` / `IsProxied`, mount
    /// source / target / type / read-only, `WithReplicas`, `WithLifetime`,
    /// `WaitAnnotation`'s type and exit code, health-check keys.</para>
    ///
    /// <para><b>What it does NOT catch, and this is stated because the first version of
    /// this comment overclaimed.</b> A value computed inside a <b>callback</b>.
    /// `WithEnvironment("MODE", flag ? "a" : "b")` writes an internal
    /// <c>EnvironmentAnnotation</c> whose only public member is a <c>Func&lt;&gt;</c> -
    /// measured - and `WithArgs` is the same shape. Two models differing only there have
    /// identical fingerprints and the stale manifest comes back. Closing that would mean
    /// the guard <b>invoking learner-authored callbacks</b>, which is a side effect a
    /// safety net has no business causing: an ex007-shaped row that counted callback
    /// invocations would be corrupted by the very thing protecting it.</para>
    ///
    /// <para>So: <b>the RULE in <see cref="SharedPublishAsync"/> is the protection; this
    /// is a net under the structural half of it.</b> A static <c>Configure</c> reading
    /// static mutable state must call <see cref="PublishAsync"/> whether or not the guard
    /// would happen to notice.</para>
    /// </summary>
    private static void VerifyUnchanged(
        Action<IDistributedApplicationBuilder> configure,
        PublishOutput published,
        string? containerRuntime)
    {
        // The rebuild is handed the same ASPIRE_CONTAINER_RUNTIME the publish got. It runs
        // no pipeline so it never probes either way; this exists so that a Configure which
        // ever branched on the key would be compared like for like, the same reason the
        // rebuild is in publish mode.
        var current = Fingerprint(ModelHarness.BuildForPublish(configure, containerRuntime).Resources);
        if (current == published.ModelFingerprint)
        {
            return;
        }

        throw new InvalidOperationException(
            $"'{configure.Method.DeclaringType?.Name}.{configure.Method.Name}' built a DIFFERENT "
            + "model this time, so the shared publish held for it is stale and would have been "
            + "handed back silently. This is what happens when a STATIC Configure reads STATIC "
            + "MUTABLE state: its cache key is a stable (MethodInfo, null), so it hits. Call "
            + "ManifestHarness.PublishAsync, which shares nothing, instead of GenerateAsync or "
            + "SharedPublishAsync. See MicroServices/README.md sections 6 and 9.");
    }

    /// <summary>
    /// A model's observable shape, flattened - see <see cref="VerifyUnchanged"/> for what
    /// that does and does not include.
    ///
    /// Deterministic within a process for a given model: the generated volume names and
    /// conditional-resource hashes Aspire derives from the AppHost are stable per name
    /// (README section 6), secrets appear as unresolved <c>{x.value}</c> placeholders
    /// rather than as values, and the property whitelist below deliberately excludes the
    /// endpoint annotation's allocation-time members (<c>AllocatedEndpoint</c> and the two
    /// snapshot collections), whose <c>ToString()</c> is a type name at model time.
    ///
    /// Measured cost: ~0.18 ms for a model of two servers, a database and a container -
    /// against ~26 ms for the rebuild that produces it, so the value half of this is free.
    /// </summary>
    internal static string Fingerprint(IEnumerable<IResource> resources)
        => string.Join(
            "\n",
            resources.Select(resource =>
            {
                var annotations = string.Join(
                    ",", resource.Annotations.Select(Describe).Order());
                var connectionString = resource is IResourceWithConnectionString withConnectionString
                    ? withConnectionString.ConnectionStringExpression.ValueExpression
                    : string.Empty;
                return $"{resource.Name}|{resource.GetType().FullName}|{annotations}|{connectionString}";
            }).Order());

    /// <summary>
    /// One annotation's type plus every value it stores declaratively. Generic on purpose:
    /// a whitelist of annotation TYPES would have to be extended by every future row and
    /// would silently go blind when it was not, whereas a whitelist of value KINDS covers
    /// annotations nobody has written yet.
    /// </summary>
    private static string Describe(object annotation)
    {
        var type = annotation.GetType();
        if (!ValueProperties.TryGetValue(type, out var properties))
        {
            properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead
                                   && property.GetIndexParameters().Length == 0
                                   && IsValueKind(property.PropertyType))
                .OrderBy(property => property.Name, StringComparer.Ordinal)
                .ToArray();
            ValueProperties[type] = properties;
        }

        var values = properties.Select(property => $"{property.Name}={Render(property, annotation)}");
        return $"{type.FullName}({string.Join(";", values)})";
    }

    private static readonly Dictionary<Type, PropertyInfo[]> ValueProperties = [];

    /// <summary>
    /// Whether a property's type is something whose value is stable, cheap and meaningful
    /// to render at model time. Everything else - delegates above all, but also
    /// allocation-time objects and collections - is excluded, because rendering it would
    /// be either nondeterministic or a type name, and both are worse than absent.
    /// </summary>
    private static bool IsValueKind(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying == typeof(string)
               || underlying.IsPrimitive
               || underlying.IsEnum
               || underlying == typeof(decimal)
               || typeof(IResource).IsAssignableFrom(underlying);
    }

    private static string Render(PropertyInfo property, object annotation)
    {
        try
        {
            return property.GetValue(annotation) switch
            {
                null => "<null>",
                IResource resource => resource.Name,
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                var other => other.ToString() ?? "<null>"
            };
        }
        catch (Exception failure)
        {
            // A getter that throws must not be able to fail a test that was only asking
            // for a manifest. Recording the failure keeps it deterministic and visible.
            return $"<unreadable:{failure.GetType().Name}>";
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

            // Captured from THIS builder rather than from a second one, so that a publish
            // invokes configure exactly once - the count it had before the shared cache
            // existed. Taken after Build(), because that is where ModelHarness.BuildForPublish
            // takes its snapshot too, and the two have to be comparable.
            var fingerprint = Fingerprint(builder.Resources);

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

            return new PublishOutput(dir, fingerprint);
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

    internal PublishOutput(string directory, string modelFingerprint)
    {
        Directory = directory;
        ModelFingerprint = modelFingerprint;
    }

    /// <summary>
    /// The shape of the model this output was published from, as
    /// <see cref="ManifestHarness.Fingerprint"/> renders it. Read only by
    /// ManifestHarness's stale-share guard.
    /// </summary>
    internal string ModelFingerprint { get; }

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
