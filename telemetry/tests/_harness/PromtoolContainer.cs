using System.Text;
using DotNet.Testcontainers.Builders;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Runs Prometheus's own <c>promtool</c> against an exposition document.
///
/// This is the cheapest possible container fact and the shape to copy: no ports, no
/// networking back into the test host, no cleanup beyond the container itself. The
/// document goes in as a mapped file, promtool validates it, and the exit code is the
/// answer.
///
/// It is also a genuinely stricter grader than the assertions beside it - promtool
/// checks the whole grammar of the exposition format, including the parts a test would
/// never think to look at.
///
/// The container is kept ALIVE and the tool run through Exec, rather than being started
/// with the tool as its command. Measured 2026-09-06: Testcontainers' default wait
/// strategy waits for the container to be running, so a container whose command exits
/// immediately never satisfies it and StartAsync fails before anything can be read. Exec
/// sidesteps that entirely and hands back the exit code and both streams.
/// </summary>
public static class PromtoolContainer
{
    /// <summary>Pinned. A moving tag would make a green run today prove nothing tomorrow.</summary>
    private const string Image = "prom/prometheus:v3.1.0";

    private const string DocumentPath = "/tmp/metrics.txt";

    /// <summary>Validate <paramref name="expositionText"/>; exit code 0 means accepted.</summary>
    public static async Task<(long ExitCode, string Output)> CheckMetrics(string expositionText)
    {
        var container = new ContainerBuilder(Image)
            .WithResourceMapping(Encoding.UTF8.GetBytes(expositionText), DocumentPath)
            // The image's own entrypoint is the Prometheus server, which would try to
            // start and fail without a config. Idle instead, and run the tool by hand.
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("sleep 120")
            .Build();

        try
        {
            await container.StartAsync();

            var result = await container.ExecAsync(
                ["/bin/sh", "-c", $"promtool check metrics < {DocumentPath}"]);

            // ExitCode is nullable here; a null means the daemon never reported one,
            // which is a failure rather than a pass.
            return (result.ExitCode ?? -1, result.Stdout + result.Stderr);
        }
        finally
        {
            await container.DisposeAsync();
        }
    }
}
