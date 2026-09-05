using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests;

/// <summary>
/// Fails first when the two-library UseSolutions mechanism breaks. These facts
/// must pass in BOTH the red run and the green run - they grade the harness,
/// not an exercise.
/// </summary>
public class HarnessSmokeTests
{
    [Fact]
    public void Tier_marker_resolves_from_whichever_library_is_referenced()
        => Assert.Equal("01-beginner", TierMarker.Tier);

    [Fact]
    public void Exactly_one_content_library_is_loaded()
    {
        var names = typeof(TierMarker).Assembly.GetName().Name;
        Assert.True(
            names is "FeWoLearning.MicroServices.Exercises" or "FeWoLearning.MicroServices.Solutions",
            $"Unexpected content assembly: {names}");
    }
}
