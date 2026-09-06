using System.Diagnostics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Collects completed activities from EXACTLY ONE <see cref="ActivitySource"/> name.
/// Scoping by name is the second line of defence behind the serial run: even if a
/// reset is missed, a probe cannot see another exercise's spans.
/// </summary>
public sealed class TraceProbe : IDisposable
{
    private readonly List<Activity> _stopped = [];
    private readonly ActivityListener _listener;

    public TraceProbe(string sourceName)
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == sourceName,
            // AllDataAndRecorded, not AllData: without the Recorded flag the activity
            // is created but Activity.Recorded is false, and any implementation that
            // guards its tagging on Recorded then emits nothing - so the test would
            // fail for a reason that has nothing to do with the exercise.
            Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => { lock (_stopped) _stopped.Add(activity); },
        };

        ActivitySource.AddActivityListener(_listener);
    }

    /// <summary>Completed activities, oldest first.</summary>
    public IReadOnlyList<Activity> Stopped
    {
        get { lock (_stopped) return _stopped.ToArray(); }
    }

    /// <summary>The single completed activity, asserting there is exactly one.</summary>
    public Activity Single() => Assert.Single(Stopped);

    public void Dispose() => _listener.Dispose();
}
