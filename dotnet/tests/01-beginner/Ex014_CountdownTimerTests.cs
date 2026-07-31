using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex014_CountdownTimerTests
{
    [Theory]
    [InlineData(2026, 7, 31, 10, 0, 0, 2026, 7, 31, 12, 30, 45, "02:30:45")]
    [InlineData(2026, 7, 31, 0, 0, 0, 2026, 7, 31, 0, 0, 0, "00:00:00")]
    [InlineData(2026, 7, 31, 23, 59, 59, 2026, 8, 1, 1, 0, 0, "01:00:01")]
    [InlineData(2026, 1, 1, 6, 15, 30, 2026, 1, 1, 6, 15, 45, "00:00:15")]
    [InlineData(2026, 3, 1, 0, 0, 0, 2026, 3, 2, 0, 0, 0, "24:00:00")]
    public void FormatRemaining_ReturnsExpected(
        int sy, int smo, int sd, int sh, int smi, int ss,
        int ty, int tmo, int td, int th, int tmi, int ts,
        string expected)
    {
        var start = new DateTime(sy, smo, sd, sh, smi, ss);
        var target = new DateTime(ty, tmo, td, th, tmi, ts);

        Assert.Equal(expected, CountdownTimer.FormatRemaining(start, target));
    }
}
