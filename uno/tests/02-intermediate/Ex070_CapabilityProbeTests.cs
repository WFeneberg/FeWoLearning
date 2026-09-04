using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex070_CapabilityProbeTests : UnoTestContext
{
    [Fact]
    public void A_Registered_Capability_Is_Answered()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => true);

        Assert.True(probe.Supports("camera"));
    }

    [Fact]
    public void A_Probe_That_Says_No_Is_Believed()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => false);

        Assert.False(probe.Supports("camera"));
    }

    [Fact]
    public void An_Unknown_Capability_Is_Not_Offered()
    {
        var probe = new Ex070_CapabilityProbe();

        // Unknown means "do not offer it". Defaulting to true would show a button that
        // fails when pressed, on exactly the platforms nobody tested.
        Assert.False(probe.Supports("teleporter"));
    }

    [Fact]
    public void A_Probe_Runs_At_Most_Once()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => true);

        probe.Supports("camera");
        probe.Supports("camera");
        probe.Supports("camera");

        // A probe can be a permission check or a hardware query, and the UI asks on every
        // layout pass.
        Assert.Equal(1, probe.ProbeCalls["camera"]);
    }

    [Fact]
    public void A_False_Answer_Is_Cached_Too()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => false);

        probe.Supports("camera");
        probe.Supports("camera");

        // Caching only the true answers is the easy bug: a false one then re-runs the
        // expensive probe on every single query.
        Assert.Equal(1, probe.ProbeCalls["camera"]);
    }

    [Fact]
    public void An_Unknown_Capability_Runs_No_Probe()
    {
        var probe = new Ex070_CapabilityProbe();

        probe.Supports("teleporter");

        Assert.Empty(probe.ProbeCalls);
    }

    [Fact]
    public void Capabilities_Are_Independent()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => true);
        probe.Register("nfc", () => false);

        Assert.True(probe.Supports("camera"));
        Assert.False(probe.Supports("nfc"));
    }

    [Fact]
    public void Invalidating_Runs_The_Probes_Again()
    {
        var probe = new Ex070_CapabilityProbe();
        var allowed = false;
        probe.Register("camera", () => allowed);
        Assert.False(probe.Supports("camera"));

        allowed = true;
        probe.Invalidate();

        // After a permission prompt the answer legitimately changes, and a cache with no
        // way to forget is then worse than no cache.
        Assert.True(probe.Supports("camera"));
        Assert.Equal(2, probe.ProbeCalls["camera"]);
    }

    [Fact]
    public void Re_Registering_Replaces_The_Probe_And_Its_Answer()
    {
        var probe = new Ex070_CapabilityProbe();
        probe.Register("camera", () => false);
        Assert.False(probe.Supports("camera"));

        probe.Register("camera", () => true);

        Assert.True(probe.Supports("camera"));
    }
}
