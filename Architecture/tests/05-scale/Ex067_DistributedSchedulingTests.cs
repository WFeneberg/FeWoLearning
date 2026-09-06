using FeWoLearning.Architecture.Exercises.Scale.Ex067;
using FeWoLearning.Architecture.Exercises.Support;

namespace FeWoLearning.Architecture.Tests.Scale;

public class Ex067_DistributedSchedulingTests
{
    private static readonly TimeSpan Hourly = TimeSpan.FromHours(1);
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 9, 17, 43, TimeSpan.Zero);

    /// <summary>Three instances sharing one run log, each with its own slightly-off clock.</summary>
    private static (RunLog Log, ManualClock[] Clocks, DistributedScheduler[] Instances) Fleet(int count = 3)
    {
        var log = new RunLog();
        var clocks = Enumerable.Range(0, count)
            .Select(i => new ManualClock(Start.AddMilliseconds(i * 40))) // clock skew, as in life
            .ToArray();

        return (log, clocks, [.. clocks.Select(c => new DistributedScheduler(c, log, Hourly))]);
    }

    [Fact]
    public void The_Occurrence_Is_The_Clock_Floored_To_The_Interval()
    {
        var (_, _, instances) = Fleet(1);

        Assert.Equal(new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero), instances[0].OccurrenceFor(Start));
    }

    [Fact]
    public void Mechanism_Skewed_Clocks_Still_Name_The_Same_Occurrence()
    {
        // What makes this work without coordination. Every instance computes the same
        // answer from its own clock, so the only thing that has to be shared - and atomic -
        // is the claim.
        var (_, clocks, instances) = Fleet();

        var named = instances.Select((s, i) => s.OccurrenceFor(clocks[i].UtcNow)).Distinct();

        Assert.Single(named);
    }

    [Fact]
    public void Mechanism_Only_One_Instance_In_The_Fleet_Runs_The_Job()
    {
        var (_, _, instances) = Fleet();
        var runs = 0;

        var ran = instances.Select(s => s.TryRun("nightly-report", () => runs++)).ToList();

        Assert.Equal(1, runs);
        Assert.Single(ran, r => r);
    }

    [Fact]
    public void The_Next_Interval_Runs_It_Again_Once()
    {
        var (_, clocks, instances) = Fleet();
        var runs = 0;
        foreach (var instance in instances) instance.TryRun("nightly-report", () => runs++);

        foreach (var clock in clocks) clock.Advance(Hourly);
        foreach (var instance in instances) instance.TryRun("nightly-report", () => runs++);

        Assert.Equal(2, runs);
    }

    [Fact]
    public void Mechanism_A_Gap_Of_Several_Intervals_Runs_Once_Not_Once_Per_Missed_Interval()
    {
        // The clause that costs a night's sleep. An instance returning after a two-hour
        // outage and finding twelve unclaimed occurrences will, given the chance, run
        // twelve reports at once - against a system that has only just come back and is
        // already struggling. The occurrence being asked for is THIS one.
        var (log, clocks, instances) = Fleet();
        var runs = 0;
        foreach (var instance in instances) instance.TryRun("nightly-report", () => runs++);

        foreach (var clock in clocks) clock.Advance(Hourly * 12);
        foreach (var instance in instances) instance.TryRun("nightly-report", () => runs++);

        Assert.Equal(2, runs);
        Assert.Equal(2, log.ClaimCount);
    }

    [Fact]
    public void Adversarial_Two_Jobs_Do_Not_Block_Each_Other()
    {
        // Claiming on the occurrence alone rather than on (job, occurrence) means the
        // first job of the hour silently cancels every other scheduled job that hour -
        // and each of them looks individually fine in its own test.
        var (_, _, instances) = Fleet(1);
        var reports = 0;
        var cleanups = 0;

        Assert.True(instances[0].TryRun("nightly-report", () => reports++));
        Assert.True(instances[0].TryRun("cleanup", () => cleanups++));

        Assert.Equal(1, reports);
        Assert.Equal(1, cleanups);
    }

    [Fact]
    public void Adversarial_A_Failing_Job_Has_Still_Claimed_Its_Occurrence()
    {
        // Deliberate, not an oversight: claiming before running makes this at-most-once.
        // A failure is not silently picked up elsewhere - somebody has to notice - and
        // two instances cannot both run a job because the first was merely slow.
        var (_, _, instances) = Fleet();

        Assert.Throws<InvalidOperationException>(
            () => instances[0].TryRun("nightly-report", () => throw new InvalidOperationException("boom")));

        var runs = 0;
        Assert.False(instances[1].TryRun("nightly-report", () => runs++));
        Assert.Equal(0, runs);
    }
}
