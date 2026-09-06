using System.IO;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex067_SessionCorrelationTests
{
    private static string? Attribute(OpenTelemetry.Resources.Resource resource, string key) =>
        resource.Attributes.FirstOrDefault(a => a.Key == key).Value?.ToString();

    [Fact]
    public void The_session_id_is_one_random_id_for_the_whole_run()
    {
        var first = Ex067_SessionCorrelation.SessionId;
        var again = Ex067_SessionCorrelation.SessionId;

        Assert.Equal(first, again);
        Assert.True(Guid.TryParse(first, out _), $"expected a random id, got '{first}'");
    }

    [Fact]
    public void Adversarial_A_The_installation_id_survives_a_restart()
    {
        // The question this id answers is "has this happened HERE before", which is what
        // separates one unlucky user from a pattern. An id that changes every run cannot
        // answer it, and looks exactly like a correct implementation for one run.
        using var scratch = new ScratchDirectory();
        var file = scratch.File("installation.id");

        var first = Ex067_SessionCorrelation.GetOrCreateInstallationId(file);
        var afterRestart = Ex067_SessionCorrelation.GetOrCreateInstallationId(file);

        Assert.Equal(first, afterRestart);
        Assert.True(File.Exists(file), "the id has to be persisted, not just remembered");
    }

    [Fact]
    public void Adversarial_B_A_different_installation_gets_a_different_id()
    {
        // Per installation, not per machine and not per user. An id derived from the
        // machine name or the user would be identical here - and would follow the person
        // across applications and survive a reinstallation, which is the whole thing this
        // avoids.
        using var scratch = new ScratchDirectory();

        var one = Ex067_SessionCorrelation.GetOrCreateInstallationId(scratch.File("one.id"));
        var two = Ex067_SessionCorrelation.GetOrCreateInstallationId(scratch.File("two.id"));

        Assert.NotEqual(one, two);
        Assert.True(Guid.TryParse(one, out _));
        Assert.True(Guid.TryParse(two, out _));
    }

    [Fact]
    public void Adversarial_C_Neither_id_is_derived_from_anything_identifying()
    {
        // A random id is exactly as joinable as a derived one and carries none of the
        // baggage. This catches the implementation that reaches for Environment.MachineName
        // or the user name "so it is stable".
        using var scratch = new ScratchDirectory();

        var installation = Ex067_SessionCorrelation.GetOrCreateInstallationId(scratch.File("i.id"));

        foreach (var identifying in new[] { Environment.MachineName, Environment.UserName })
        {
            Assert.DoesNotContain(identifying, installation, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(identifying, Ex067_SessionCorrelation.SessionId, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Both_ids_reach_the_resource()
    {
        // Row 028's point: they are constant for the process, so they belong where the
        // exporter attaches them once - and then every span, metric and log carries them
        // without a single call site mentioning either.
        using var scratch = new ScratchDirectory();
        var file = scratch.File("installation.id");

        var resource = Ex067_SessionCorrelation.BuildResource(file);

        Assert.Equal(
            Ex067_SessionCorrelation.SessionId,
            Attribute(resource, Ex067_SessionCorrelation.SessionAttribute));
        Assert.Equal(
            Ex067_SessionCorrelation.GetOrCreateInstallationId(file),
            Attribute(resource, Ex067_SessionCorrelation.InstallationAttribute));
    }

    [Fact]
    public void The_session_and_the_installation_are_different_ids()
    {
        // Two different questions - "what else happened in this run" and "has this happened
        // here before" - and neither substitutes for the other.
        using var scratch = new ScratchDirectory();

        Assert.NotEqual(
            Ex067_SessionCorrelation.SessionId,
            Ex067_SessionCorrelation.GetOrCreateInstallationId(scratch.File("i.id")));
    }
}
