using System.Text.Json;
using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Runs Aspire's publish operation in-process (~3.7 s) and returns the generated
/// aspire-manifest.json.
///
/// Do NOT shell out to `aspire publish`: it writes its artifacts and then does not
/// exit in a non-interactive shell, dropping into "press CTRL+C to stop the AppHost".
/// Measured still running at 600 s.
///
/// The manifest carries per resource: type (container.v0 / value.v0 / parameter.v0),
/// the pinned image, the full env map including ConnectionStrings__*, bindings with
/// targetPort, and the generated-secret policy. Docker Compose YAML is NOT produced
/// in-process - see the spec for how compose rows are graded instead.
/// </summary>
public static class ManifestHarness
{
    public static async Task<JsonDocument> GenerateAsync(
        Action<IDistributedApplicationBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        var dir = Path.Combine(Path.GetTempPath(), "fewo-ms-" + Guid.NewGuid().ToString("N")[..8]);
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
            timeout.CancelAfter(TimeSpan.FromSeconds(90));
            await app.RunAsync(timeout.Token);

            var path = Path.Combine(dir, "aspire-manifest.json");
            if (!File.Exists(path))
            {
                throw new InvalidOperationException(
                    $"Publish produced no manifest. Files present: " +
                    (Directory.Exists(dir)
                        ? string.Join(", ", Directory.GetFiles(dir).Select(Path.GetFileName))
                        : "<no directory>"));
            }

            return JsonDocument.Parse(await File.ReadAllTextAsync(path, cancellationToken));
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
        }
    }
}
