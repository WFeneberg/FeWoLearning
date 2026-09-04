using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml.Media.Animation;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex083_EasingFunctionsTests : UnoTestContext
{
    private static double At(EasingMode mode, double progress) =>
        Ex083_EasingFunctions.Apply(Ex083_EasingFunctions.CreateCubic(mode), progress);

    [Fact]
    public void The_Easing_Is_A_Cubic_In_The_Requested_Mode()
    {
        var easing = Ex083_EasingFunctions.CreateCubic(EasingMode.EaseOut);

        Assert.IsType<CubicEase>(easing);
        Assert.Equal(EasingMode.EaseOut, easing.EasingMode);
    }

    [Fact]
    public void Every_Mode_Honours_The_Endpoints()
    {
        foreach (var mode in new[] { EasingMode.EaseIn, EasingMode.EaseOut, EasingMode.EaseInOut })
        {
            // An easing that does not start at 0 and end at 1 makes an animation jump at
            // one end, which is what "it feels wrong at the end" usually is.
            Assert.Equal(0, At(mode, 0), 6);
            Assert.Equal(1, At(mode, 1), 6);
        }
    }

    [Fact]
    public void Ease_In_Is_The_Raw_Cubic()
    {
        Assert.Equal(0.125, At(EasingMode.EaseIn, 0.5), 6);
        Assert.Equal(0.008, At(EasingMode.EaseIn, 0.2), 6);
    }

    [Fact]
    public void Ease_Out_Is_The_Curve_Mirrored()
    {
        // 1 - (1 - t)^3: the same curve through both axes, which is why it starts fast.
        Assert.Equal(0.875, At(EasingMode.EaseOut, 0.5), 6);
        Assert.Equal(1 - At(EasingMode.EaseIn, 0.8), At(EasingMode.EaseOut, 0.2), 6);
    }

    [Fact]
    public void Ease_In_Out_Meets_In_The_Middle()
    {
        Assert.Equal(0.5, At(EasingMode.EaseInOut, 0.5), 6);
    }

    [Fact]
    public void Ease_In_Starts_Slower_Than_Ease_Out()
    {
        Assert.True(At(EasingMode.EaseIn, 0.25) < At(EasingMode.EaseOut, 0.25));
    }

    [Fact]
    public void Sampling_Includes_Both_Endpoints()
    {
        var samples = Ex083_EasingFunctions.Sample(Ex083_EasingFunctions.CreateCubic(EasingMode.EaseIn), steps: 4);

        // Five samples for four steps. A sampler that stops before 1 hides exactly the
        // end-of-animation behaviour it was written to inspect.
        Assert.Equal(5, samples.Count);
        Assert.Equal(0, samples[0], 6);
        Assert.Equal(1, samples[^1], 6);
    }

    [Fact]
    public void Sampling_Walks_Evenly()
    {
        var samples = Ex083_EasingFunctions.Sample(Ex083_EasingFunctions.CreateCubic(EasingMode.EaseIn), steps: 2);

        Assert.Equal([0, 0.125, 1], samples.Select(value => Math.Round(value, 6)));
    }

    [Fact]
    public void A_Progress_Curve_Never_Goes_Backwards()
    {
        foreach (var mode in new[] { EasingMode.EaseIn, EasingMode.EaseOut, EasingMode.EaseInOut })
        {
            var samples = Ex083_EasingFunctions.Sample(Ex083_EasingFunctions.CreateCubic(mode), steps: 20);

            Assert.True(Ex083_EasingFunctions.IsMonotonic(samples), $"{mode} went backwards");
        }
    }

    [Fact]
    public void A_Sequence_That_Goes_Backwards_Is_Rejected()
    {
        // An easing may legitimately overshoot - BackEase, ElasticEase - and then this is
        // false. The check is about progress curves, not about all easings.
        Assert.False(Ex083_EasingFunctions.IsMonotonic([0, 0.5, 0.4, 1]));
    }

    [Fact]
    public void A_Single_Value_Is_Monotonic()
    {
        Assert.True(Ex083_EasingFunctions.IsMonotonic([0.5]));
    }
}
