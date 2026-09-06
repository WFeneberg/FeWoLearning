using FeWoLearning.Telemetry.Exercises.Support;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Canaries. These must pass in BOTH modes (`dotnet test` and
/// `dotnet test -p:UseSolutions=true`) and are the first thing to fail when a
/// package bump breaks the harness. They are never TODOs and never get a
/// catalog.md row.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void The_test_project_references_exactly_one_content_library()
    {
        // Touch a type from the content library FIRST. A referenced assembly is
        // loaded lazily, so walking GetAssemblies() without this finds nothing and
        // the canary fails for a reason unrelated to the track. Measured 2026-09-06.
        Assert.Equal("telemetry", TrackMarker.TrackName);

        var loaded = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .Where(n => n is "FeWoLearning.Telemetry.Exercises" or "FeWoLearning.Telemetry.Solutions")
            .ToArray();

        // Two would mean the UseSolutions switch stopped being exclusive, and the
        // identical type names would collide. Zero is impossible after the line above.
        Assert.Single(loaded);
    }
}
