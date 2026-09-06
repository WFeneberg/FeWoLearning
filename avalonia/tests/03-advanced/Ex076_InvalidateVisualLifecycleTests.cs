using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex076_InvalidateVisualLifecycleTests
{
    // One control and one window per test, never two. Measured: a second window
    // shown in the same test does not paint at all until the render timer is
    // ticked, so sharing a fixture here would make the counts depend on test
    // order rather than on the code under test.
    private static Ex076_InvalidateVisualLifecycle Shown()
    {
        var control = new Ex076_InvalidateVisualLifecycle { Width = 50, Height = 30 };
        ViewHarness.ShowWindow(control, 200, 200);
        ViewHarness.PumpRender();
        return control;
    }

    // Rendering is requested, not performed: with nothing dirty, driving frame
    // after frame repaints nothing. Nudge at the end is what makes this test red
    // against the stub, and it also shows the contrast in one place.
    [AvaloniaFact]
    public void Idle_Frames_Repaint_Nothing_But_A_Nudge_Does()
    {
        var control = Shown();

        Assert.Equal(1, control.RenderCount);

        ViewHarness.PumpRender();
        ViewHarness.PumpRender();
        Assert.Equal(1, control.RenderCount);

        control.Nudge();
        ViewHarness.PumpRender();
        Assert.Equal(2, control.RenderCount);
    }

    [AvaloniaFact]
    public void Each_Nudge_Costs_Exactly_One_Repaint()
    {
        var control = Shown();

        for (var expected = 2; expected <= 4; expected++)
        {
            control.Nudge();
            ViewHarness.PumpRender();
            Assert.Equal(expected, control.RenderCount);
        }
    }

    // The coalescing lesson: asking five times before the next frame is one
    // repaint, not five. This is why InvalidateVisual is cheap to call and why
    // hand-rolled "only invalidate if something changed" caches are usually
    // pointless.
    [AvaloniaFact]
    public void Many_Nudges_Before_One_Frame_Coalesce_Into_A_Single_Repaint()
    {
        var control = Shown();

        for (var i = 0; i < 5; i++)
        {
            control.Nudge();
        }

        ViewHarness.PumpRender();

        Assert.Equal(2, control.RenderCount);
    }

    [AvaloniaFact]
    public void Advance_Moves_The_Tick_And_Repaints_Through_AffectsRender()
    {
        var control = Shown();

        control.Advance();
        ViewHarness.PumpRender();

        Assert.Equal(1, control.Ticks);
        Assert.Equal(2, control.RenderCount);

        control.Advance();
        ViewHarness.PumpRender();

        Assert.Equal(2, control.Ticks);
        Assert.Equal(3, control.RenderCount);
    }

    // The discriminator between AffectsRender and an InvalidateVisual call inside
    // Advance: both repaint when the value moves, but only the registration knows
    // to stay quiet when it does not. Assigning the current value again is not a
    // change, so the property system raises nothing and no frame is owed.
    [AvaloniaFact]
    public void Re_Assigning_The_Same_Tick_Value_Repaints_Nothing()
    {
        var control = Shown();

        control.Advance();
        ViewHarness.PumpRender();
        var after = control.RenderCount;

        control.Ticks = control.Ticks;
        ViewHarness.PumpRender();

        Assert.Equal(after, control.RenderCount);
    }

    // The other half of the registration: a property that was not registered
    // never repaints, however often it changes.
    [AvaloniaFact]
    public void Changing_A_Property_Outside_The_Registration_Repaints_Nothing()
    {
        var control = Shown();

        control.Advance();
        ViewHarness.PumpRender();
        var after = control.RenderCount;

        control.Note = "changed";
        ViewHarness.PumpRender();
        control.Note = "changed again";
        ViewHarness.PumpRender();

        Assert.Equal("changed again", control.Note);
        Assert.Equal(after, control.RenderCount);
    }
}
