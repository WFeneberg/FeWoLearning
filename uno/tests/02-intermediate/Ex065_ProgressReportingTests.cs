using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex065_ProgressReportingTests : UnoTestContext
{
    [Fact]
    public void Records_A_Report()
    {
        var sink = new Ex065_ProgressReporting();

        Assert.Equal(0, sink.Percent);
        Assert.False(sink.IsRunning);

        sink.Report(42);

        Assert.Equal(42, sink.Percent);
    }

    [Fact]
    public void Clamps_Above_One_Hundred()
    {
        var sink = new Ex065_ProgressReporting();

        sink.Report(110);

        // An operation that reports 110% is a bug in the operation. A bar that renders it
        // is a bug here - both ends check.
        Assert.Equal(100, sink.Percent);
    }

    [Fact]
    public void Clamps_Below_Zero()
    {
        var sink = new Ex065_ProgressReporting();

        sink.Report(-5);

        Assert.Equal(0, sink.Percent);
    }

    [Fact]
    public void Ignores_A_Nan_Report()
    {
        var sink = new Ex065_ProgressReporting();
        sink.Report(40);

        sink.Report(double.NaN);

        // A division by zero upstream ("0 of 0 files") must not blank the bar.
        Assert.Equal(40, sink.Percent);
    }

    [Fact]
    public void Announces_A_Change()
    {
        var sink = new Ex065_ProgressReporting();
        var names = new List<string?>();
        sink.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        sink.Report(42);

        Assert.Contains(nameof(Ex065_ProgressReporting.Percent), names);
    }

    [Fact]
    public void Does_Not_Announce_An_Unchanged_Value()
    {
        var sink = new Ex065_ProgressReporting();
        sink.Report(42);
        var names = new List<string?>();
        sink.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        sink.Report(42);

        Assert.Empty(names);
    }

    [Fact]
    public async Task Hands_Itself_To_The_Work_As_The_Progress_Sink()
    {
        var sink = new Ex065_ProgressReporting();

        await sink.RunAsync(progress =>
        {
            progress.Report(50);
            progress.Report(100);
            return Task.CompletedTask;
        });

        // The operation knows nothing but IProgress<double> - which is what lets it be
        // tested with a recorder and shipped against a UI.
        Assert.Equal(100, sink.Percent);
    }

    [Fact]
    public async Task Is_Running_For_The_Duration()
    {
        var gate = new TaskCompletionSource();
        var sink = new Ex065_ProgressReporting();

        var running = sink.RunAsync(_ => gate.Task);
        Assert.True(sink.IsRunning);

        gate.SetResult();
        await running;

        Assert.False(sink.IsRunning);
    }

    [Fact]
    public async Task Stops_Running_When_The_Work_Throws()
    {
        var sink = new Ex065_ProgressReporting();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sink.RunAsync(_ => throw new InvalidOperationException("boom")));

        // Without the finally a failed operation leaves a spinner on screen for ever.
        Assert.False(sink.IsRunning);
    }

    [Fact]
    public async Task The_Exception_Is_Left_To_The_Caller()
    {
        var sink = new Ex065_ProgressReporting();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sink.RunAsync(_ => throw new InvalidOperationException("boom")));

        // A progress sink is not an error handler: swallowing here would hide the failure
        // from whoever actually knows what to do about it.
        Assert.Equal("boom", error.Message);
    }
}
