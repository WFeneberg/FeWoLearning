using FeWoLearning.MicroServices.Tests;

// The one place in the assembly that gets to run code AFTER the last test.
//
// Both fast paths added on 2026-09-07 own process-lifetime state that a per-test
// `finally` cannot release:
//
//   * ContainerHarness keeps ONE database server per flavour for the whole assembly, so
//     that twenty-odd container rows share a start-up instead of paying for one each.
//   * ManifestHarness keeps ONE publish output directory per distinct model, so that
//     several facts asserting on the same manifest publish it once.
//
// Measured on xunit.v3 3.2.2 with xunit.runner.visualstudio 3.1.5, which is the runner
// this track pins (README section 7): the fixture is constructed and InitializeAsync
// awaited BEFORE the first test - even under `--filter` - and DisposeAsync is awaited
// after the last one. That is the whole reason it exists, so InitializeAsync
// deliberately does NOTHING: an assembly fixture that started a container would start it
// in the default `dotnet test` too, which is exactly what ContainerGate exists to
// prevent. Everything here is lazy; this type only closes.
[assembly: AssemblyFixture(typeof(HarnessLifetime))]

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Assembly-scoped teardown for the two harnesses that keep state across tests.
/// See the comment on the assembly attribute above for why it starts nothing.
/// </summary>
public sealed class HarnessLifetime : IAsyncLifetime, IAsyncDisposable
{
    /// <summary>
    /// Deliberately empty. Nothing in this assembly may start a container, or publish,
    /// before a test asks for it.
    /// </summary>
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        // Manifest outputs first: deleting directories cannot fail in a way that should
        // stop the container teardown, and the container teardown is the one that can
        // leave something behind on Docker.
        ManifestHarness.DisposeSharedPublishes();
        await ContainerHarness.ShutdownSharedServersAsync();
    }
}
