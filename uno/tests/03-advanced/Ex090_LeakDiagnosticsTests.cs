using FeWoLearning.Uno.Exercises.Advanced;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Advanced;

public class Ex090_LeakDiagnosticsTests : UnoTestContext
{
    /// <summary>
    /// Creates and tracks an element from a method of its own, so no local in the test body
    /// keeps it alive - which a debug build very much would do.
    /// </summary>
    private static WeakReference<FrameworkElement> TrackOne(Ex090_LeakDiagnostics registry)
    {
        var element = new Border { Width = 10, Height = 10 };
        registry.Track(element);
        return new WeakReference<FrameworkElement>(element);
    }

    [Fact]
    public void A_Tracked_Element_Is_Reported_Alive()
    {
        var registry = new Ex090_LeakDiagnostics();
        var element = new Border { Width = 10, Height = 10 };

        registry.Track(element);

        Assert.Equal(1, registry.Entries);
        Assert.Same(element, Assert.Single(registry.Alive()));
    }

    [Fact]
    public void Tracking_Does_Not_Keep_The_Element_Alive()
    {
        var registry = new Ex090_LeakDiagnostics();

        var reference = TrackOne(registry);

        // The registry is the long-lived object here. Holding its subjects strongly is how
        // a diagnostics feature ends up being the leak it was meant to find.
        Assert.True(Ex090_LeakDiagnostics.WasReleased(reference));
    }

    [Fact]
    public void A_Collected_Element_Is_Not_Reported_Alive()
    {
        var registry = new Ex090_LeakDiagnostics();
        TrackOne(registry);

        Ex090_LeakDiagnostics.WasReleased(new WeakReference<object>(new object()));

        Assert.Empty(registry.Alive());
    }

    [Fact]
    public void Reporting_Prunes_The_Dead_Entries()
    {
        var registry = new Ex090_LeakDiagnostics();
        TrackOne(registry);
        Assert.Equal(1, registry.Entries);

        Ex090_LeakDiagnostics.WasReleased(new WeakReference<object>(new object()));
        registry.Alive();

        Assert.Equal(0, registry.Entries);
    }

    [Fact]
    public void A_Live_Element_Survives_A_Collection()
    {
        var registry = new Ex090_LeakDiagnostics();
        var element = new Border { Width = 10, Height = 10 };
        registry.Track(element);

        Ex090_LeakDiagnostics.WasReleased(new WeakReference<object>(new object()));

        Assert.Same(element, Assert.Single(registry.Alive()));
    }

    [Fact]
    public void A_Reachable_Object_Is_Not_Reported_Released()
    {
        var held = new object();

        // The instrument has to be able to say no, or a passing leak test proves nothing.
        Assert.False(Ex090_LeakDiagnostics.WasReleased(new WeakReference<object>(held)));
        Assert.NotNull(held);
    }

    [Fact]
    public void A_Panel_That_Holds_A_Child_Keeps_It_Alive()
    {
        var panel = new Ex090_WatchfulPanel();
        var reference = AttachOne(panel);

        // The leak, demonstrated: the panel outlives the child, and its list is what holds
        // on. Nothing about the child says so.
        Assert.False(Ex090_LeakDiagnostics.WasReleased(reference));
        Assert.Equal(1, panel.Held);
    }

    [Fact]
    public void Detaching_Releases_The_Child()
    {
        var panel = new Ex090_WatchfulPanel();
        var child = new Border { Width = 10, Height = 10 };
        panel.Attach(child);

        panel.Detach(child);

        Assert.Equal(0, panel.Held);
    }

    [Fact]
    public void A_Detached_Child_Can_Be_Collected()
    {
        var panel = new Ex090_WatchfulPanel();
        var reference = AttachAndDetachOne(panel);

        Assert.True(Ex090_LeakDiagnostics.WasReleased(reference));
    }

    [Fact]
    public void Detaching_Something_Never_Attached_Is_Harmless()
    {
        var panel = new Ex090_WatchfulPanel();

        panel.Detach(new Border());

        Assert.Equal(0, panel.Held);
    }

    private static WeakReference<Border> AttachOne(Ex090_WatchfulPanel panel)
    {
        var child = new Border { Width = 10, Height = 10 };
        panel.Attach(child);
        return new WeakReference<Border>(child);
    }

    private static WeakReference<Border> AttachAndDetachOne(Ex090_WatchfulPanel panel)
    {
        var child = new Border { Width = 10, Height = 10 };
        panel.Attach(child);
        panel.Detach(child);
        return new WeakReference<Border>(child);
    }
}
