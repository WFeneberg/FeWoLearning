using System;
using Avalonia;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex099_CustomHeadlessTestHarnessTests
{
    // No-op installers on purpose: the builder is never started, so nothing has to
    // actually install anything for its configuration to be readable. That is also
    // what keeps composing one harmless inside the app this test runs in.
    private static AppBuilder Built(out int windowingCalls, out int renderingCalls)
    {
        var windowing = 0;
        var rendering = 0;
        var builder = Ex099_CustomHeadlessTestHarness.Build(
            () => windowing++, "TestWindowing",
            () => rendering++, "TestRendering");
        windowingCalls = windowing;
        renderingCalls = rendering;
        return builder;
    }

    [AvaloniaFact]
    public void The_Builder_Configures_The_Test_Application()
    {
        var builder = Built(out _, out _);

        Assert.Equal(typeof(Ex099_TestApplication), builder.ApplicationType);
    }

    [AvaloniaFact]
    public void Both_Subsystems_Are_Installed_Under_Their_Names()
    {
        var builder = Built(out _, out _);

        Assert.Equal("TestWindowing", builder.WindowingSubsystemName);
        Assert.Equal("TestRendering", builder.RenderingSubsystemName);
    }

    // The installers are recorded, not run: a builder describes what to do, and
    // nothing happens until it is started. A Build that invoked them eagerly would
    // pass the name assertions above and fail here.
    [AvaloniaFact]
    public void Composing_A_Builder_Runs_Neither_Installer()
    {
        Built(out var windowingCalls, out var renderingCalls);

        Assert.Equal(0, windowingCalls);
        Assert.Equal(0, renderingCalls);
    }

    [AvaloniaFact]
    public void The_Recorded_Initialisers_Are_The_Delegates_That_Were_Handed_In()
    {
        var builder = Built(out _, out _);

        Assert.NotNull(builder.WindowingSubsystemInitializer);
        Assert.NotNull(builder.RenderingSubsystemInitializer);
    }

    // Composing must not disturb the application the test is running in, which is
    // why the exercise returns it unstarted.
    [AvaloniaFact]
    public void Building_Twice_Leaves_The_Running_Application_Alone()
    {
        var current = Application.Current;

        var first = Built(out _, out _);
        var second = Built(out _, out _);

        Assert.NotSame(first, second);
        Assert.Same(current, Application.Current);
        Assert.Null(first.Instance);
    }

    // The application half. A theme in Styles is what gives controls their
    // templates; an Initialize that forgets it leaves every Template null, which
    // would quietly invalidate a large part of this track.
    [AvaloniaFact]
    public void The_Application_Adds_A_Theme_When_Initialised()
    {
        var application = new Ex099_TestApplication();

        Assert.False(application.ThemeWasAdded);

        application.Initialize();

        Assert.True(application.ThemeWasAdded);
        Assert.Contains(Ex099_TestApplication.MinimalTheme, application.Styles);
    }
}
