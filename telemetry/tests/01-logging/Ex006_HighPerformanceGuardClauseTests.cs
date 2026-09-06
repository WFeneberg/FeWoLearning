using System.Reflection;
using FeWoLearning.Telemetry.Exercises.Logging;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.Logging;

namespace FeWoLearning.Telemetry.Tests.Logging;

public class Ex006_HighPerformanceGuardClauseTests
{
    /// <summary>A phase description that remembers how often anybody asked for it.</summary>
    private sealed class CountingDescription
    {
        public int Calls { get; private set; }

        public string Describe()
        {
            Calls++;
            return "copy";
        }
    }

    [Fact]
    public void Adversarial_A_The_expensive_argument_is_not_evaluated_when_the_level_is_off()
    {
        // The whole exercise. logger.LogDebug("...", describePhase()) evaluates its
        // argument before the logger is ever consulted, so the work happens on the hot
        // path in production, where Debug is off, for a record nobody keeps.
        var description = new CountingDescription();
        using var logs = new LogProbe(builder => builder.SetMinimumLevel(LogLevel.Information));

        Ex006_HighPerformanceGuardClause.ReportProgress(logs.For("copy"), description.Describe, 40);

        Assert.Empty(logs.Records);
        Assert.Equal(0, description.Calls);
    }

    [Fact]
    public void Adversarial_B_The_expensive_argument_is_evaluated_exactly_once_when_on()
    {
        // The other half. A guard that calls describePhase() to decide something and
        // then again to log pays twice for the same string.
        var description = new CountingDescription();
        using var logs = new LogProbe();

        Ex006_HighPerformanceGuardClause.ReportProgress(logs.For("copy"), description.Describe, 40);

        Assert.Single(logs.Records);
        Assert.Equal(1, description.Calls);
    }

    [Fact]
    public void The_record_carries_the_phase_and_percent_as_named_fields()
    {
        var description = new CountingDescription();
        using var logs = new LogProbe();

        Ex006_HighPerformanceGuardClause.ReportProgress(logs.For("copy"), description.Describe, 40);

        var record = Assert.Single(logs.Records);
        Assert.Equal(LogLevel.Debug, record.Level);
        Assert.Equal("copy", LogProbe.Field(record, "Phase"));
        Assert.Equal("40", LogProbe.Field(record, "Percent"));
        Assert.Equal("Phase copy is 40% complete", record.Message);
    }

    [Fact]
    public void Adversarial_C_The_write_goes_through_a_cached_LoggerMessage_delegate()
    {
        // Read from metadata, deliberately. A hand-written
        // `if (logger.IsEnabled(...)) logger.LogDebug(...)` satisfies every fact above
        // and is not wrong - it is just not what Define buys: one parsed template
        // instead of one per call, and no boxing of the int. Neither of those leaves
        // any trace in a log record.
        //
        // Same stance as ex005's attribute check and blazor/ ex069's type constraint.
        var cached = typeof(Ex006_HighPerformanceGuardClause)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Action<ILogger, string, int, Exception?>))
            .ToArray();

        var field = Assert.Single(cached);

        // Static, so it is built once for the life of the process rather than per call.
        Assert.True(field.IsInitOnly, $"{field.Name} should be readonly");
        Assert.NotNull(field.GetValue(null));
    }
}
