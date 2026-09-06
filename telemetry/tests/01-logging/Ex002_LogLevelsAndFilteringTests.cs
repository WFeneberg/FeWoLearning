using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex002_LogLevelsAndFilteringTests
{
    [Fact]
    public void Adversarial_A_The_emitter_writes_all_six_levels_unconditionally()
    {
        // Run WITHOUT the exercise's own Configure. If fewer than six arrive, the
        // emitter is deciding for itself - and every filtering fact below would then
        // be measuring the emitter rather than the filter rules.
        using var logs = new LogProbe();

        Ex002_LogLevelsAndFiltering.EmitOnePerLevel(logs.For(Ex002_LogLevelsAndFiltering.QuietCategory));

        Assert.Equal(6, logs.Records.Count);
        Assert.Equal(
            [LogLevel.Trace, LogLevel.Debug, LogLevel.Information,
             LogLevel.Warning, LogLevel.Error, LogLevel.Critical],
            logs.Records.Select(r => r.Level));
    }

    [Fact]
    public void The_default_minimum_level_keeps_only_Warning_and_above()
    {
        using var logs = new LogProbe(Ex002_LogLevelsAndFiltering.Configure);

        Ex002_LogLevelsAndFiltering.EmitOnePerLevel(logs.For(Ex002_LogLevelsAndFiltering.QuietCategory));

        Assert.Equal(
            [LogLevel.Warning, LogLevel.Error, LogLevel.Critical],
            logs.Records.Select(r => r.Level));
    }

    [Fact]
    public void The_lowered_category_keeps_Debug_and_above_but_not_Trace()
    {
        using var logs = new LogProbe(Ex002_LogLevelsAndFiltering.Configure);

        Ex002_LogLevelsAndFiltering.EmitOnePerLevel(logs.For(Ex002_LogLevelsAndFiltering.ChattyCategory));

        Assert.Equal(
            [LogLevel.Debug, LogLevel.Information, LogLevel.Warning,
             LogLevel.Error, LogLevel.Critical],
            logs.Records.Select(r => r.Level));
    }

    [Fact]
    public void Adversarial_B_IsEnabled_agrees_with_what_actually_arrives()
    {
        // A call site consults IsEnabled BEFORE building its arguments. If it
        // disagrees with the filter that ultimately drops the record, you either pay
        // to construct messages nobody keeps or you skip records the operator asked
        // for - and both are silent. A rule installed somewhere the filter honours
        // but IsEnabled does not would pass the two facts above and fail this one.
        using var logs = new LogProbe(Ex002_LogLevelsAndFiltering.Configure);
        var quiet = logs.For(Ex002_LogLevelsAndFiltering.QuietCategory);
        var chatty = logs.For(Ex002_LogLevelsAndFiltering.ChattyCategory);

        Assert.False(quiet.IsEnabled(LogLevel.Information));
        Assert.True(quiet.IsEnabled(LogLevel.Warning));
        Assert.False(chatty.IsEnabled(LogLevel.Trace));
        Assert.True(chatty.IsEnabled(LogLevel.Debug));
    }
}
