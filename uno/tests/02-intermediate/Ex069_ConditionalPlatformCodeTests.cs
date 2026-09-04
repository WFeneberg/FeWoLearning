using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex069_ConditionalPlatformCodeTests : UnoTestContext
{
    private sealed class FakePlatform(string name, bool multiWindow) : IEx069_PlatformInfo
    {
        public string Name { get; } = name;

        public bool SupportsMultipleWindows { get; } = multiWindow;
    }

    private static Ex069_ConditionalPlatformCode On(string name, bool multiWindow) =>
        new(new FakePlatform(name, multiWindow));

    [Fact]
    public void A_Multi_Window_Platform_Opens_A_Window()
    {
        Assert.Equal("window", On("desktop", multiWindow: true).OpenDocumentTarget);
    }

    [Fact]
    public void A_Single_Window_Platform_Opens_A_Tab()
    {
        Assert.Equal("tab", On("mobile", multiWindow: false).OpenDocumentTarget);
    }

    [Fact]
    public void The_Decision_Does_Not_Depend_On_The_Platform_Name()
    {
        // Two platforms with the same capability and different names agree. A name check
        // would need updating for the next platform; a capability check does not.
        Assert.Equal(
            On("desktop", multiWindow: true).OpenDocumentTarget,
            On("something-that-does-not-exist-yet", multiWindow: true).OpenDocumentTarget);
    }

    [Fact]
    public void The_Telemetry_Tag_Lower_Cases_The_Name()
    {
        Assert.Equal("android", On("Android", multiWindow: false).TelemetryTag);
    }

    [Fact]
    public void The_Telemetry_Tag_Appends_The_Capability()
    {
        Assert.Equal("windows+multiwindow", On("Windows", multiWindow: true).TelemetryTag);
    }

    [Fact]
    public void The_Logic_Works_Against_The_Real_Platform_Too()
    {
        var target = new Ex069_ConditionalPlatformCode(Ex069_PlatformInfoFactory.Current).OpenDocumentTarget;

        Assert.Contains(target, new[] { "window", "tab" });
    }
}
