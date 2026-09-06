using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex015_ActivitySourceAndListenerTests
{
    /// <summary>
    /// Registers the exercise's own listener and collects what it delivers. Note this
    /// deliberately does NOT use the harness TraceProbe: the listener under test is
    /// half of what the row grades.
    /// </summary>
    private sealed class ExerciseListener : IDisposable
    {
        private readonly List<Activity> _stopped = [];
        private readonly ActivityListener _listener;

        public ExerciseListener()
        {
            _listener = Ex015_ActivitySourceAndListener.CreateListener(
                activity => { lock (_stopped) _stopped.Add(activity); });

            ActivitySource.AddActivityListener(_listener);
        }

        public IReadOnlyList<Activity> Stopped
        {
            get { lock (_stopped) return _stopped.ToArray(); }
        }

        public void Dispose() => _listener.Dispose();
    }

    [Fact]
    public void Adversarial_A_With_nobody_listening_the_activity_is_null()
    {
        // The fact that costs everyone a day exactly once. `using var a =
        // Source.StartActivity("work"); a?.SetTag(...)` compiles, runs, and silently
        // does nothing in production because nothing was listening - and the
        // null-conditional that makes it safe is what makes it invisible.
        //
        // Tracing is opt-in at the LISTENER, not at the source.
        using var ctx = new TelemetryContext();

        var activity = Ex015_ActivitySourceAndListener.DoWork("import", 7);

        Assert.Null(activity);
        Assert.Null(Activity.Current);
    }

    [Fact]
    public void With_a_listener_the_activity_is_real_and_carries_its_tag()
    {
        using var ctx = new TelemetryContext();
        using var listener = new ExerciseListener();

        var activity = Ex015_ActivitySourceAndListener.DoWork("import", 7);

        Assert.NotNull(activity);
        Assert.Equal("import", activity.DisplayName);
        Assert.Equal(Ex015_ActivitySourceAndListener.SourceName, activity.Source.Name);
        Assert.Equal("7", activity.GetTagItem(Ex015_ActivitySourceAndListener.ItemCountTag)?.ToString());
    }

    [Fact]
    public void The_listener_delivers_the_finished_activity()
    {
        using var ctx = new TelemetryContext();
        using var listener = new ExerciseListener();

        Ex015_ActivitySourceAndListener.DoWork("import", 7);

        var delivered = Assert.Single(listener.Stopped);
        Assert.Equal("import", delivered.DisplayName);

        // Stopped, not merely started: a listener wired to ActivityStarted reports a
        // duration of zero and no tag set after the first line of the method.
        Assert.NotEqual(default, delivered.Duration);
        Assert.Null(Activity.Current);
    }

    [Fact]
    public void Adversarial_B_The_listener_is_scoped_to_this_exercises_source()
    {
        // A listener whose ShouldListenTo returns true unconditionally receives every
        // activity in the process - the framework's, other libraries', other
        // exercises' - and its consumer then reports counts that have nothing to do
        // with the code under test.
        using var ctx = new TelemetryContext();
        using var listener = new ExerciseListener();
        using var stranger = new ActivitySource("fewolearning.telemetry.ex015.stranger");

        using (stranger.StartActivity("not mine")) { }
        Ex015_ActivitySourceAndListener.DoWork("import", 7);

        Assert.Equal("import", Assert.Single(listener.Stopped).DisplayName);
    }

    [Fact]
    public void Adversarial_C_The_sampler_asks_for_all_data_so_tags_survive()
    {
        // The same lesson one level down. ActivitySamplingResult.PropagationData
        // creates a real activity - it has a trace id, it propagates, it looks entirely
        // healthy - and its IsAllDataRequested is false.
        //
        // Measured 2026-09-06: that flag does NOT stop SetTag from writing. The tag is
        // still there afterwards, so a test asserting only the tag cannot tell the two
        // samplers apart - which is exactly why this fact asserts the flag itself.
        // IsAllDataRequested is a hint to the CALLER, and ignoring it means building
        // detail the listener said it did not want, for an activity an SDK downstream
        // will discard anyway.
        using var ctx = new TelemetryContext();
        using var listener = new ExerciseListener();

        var activity = Ex015_ActivitySourceAndListener.DoWork("import", 7);

        Assert.NotNull(activity);
        Assert.True(
            activity.IsAllDataRequested,
            "the sampling result must be AllData or AllDataAndRecorded - PropagationData "
            + "leaves this false, and every caller that honours it then records nothing");
        Assert.NotEmpty(activity.TagObjects);
    }
}
