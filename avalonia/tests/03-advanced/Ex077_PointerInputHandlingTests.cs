using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex077_PointerInputHandlingTests
{
    // The control is 40x20 in a 200x200 window, so it is arranged at 80,90 and a
    // window point maps to a control point 80,90 smaller. Both coordinate spaces
    // appear below on purpose: the window one is what input takes, the control one
    // is what the exercise must report.
    private static readonly Point Centre = new(100, 100);

    private static (Ex077_PointerInputHandling Control, Window Window) Shown()
    {
        var control = new Ex077_PointerInputHandling { Width = 40, Height = 20 };
        var window = ViewHarness.ShowWindow(control, 200, 200);
        return (control, window);
    }

    private static void Drain() => Dispatcher.UIThread.RunJobs();

    [AvaloniaFact]
    public void A_Left_Press_Starts_A_Drag_At_The_Control_Relative_Position()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Left);
        Drain();

        Assert.True(control.IsDragging);
        Assert.Equal(new Point(20, 10), control.Origin);
        Assert.Equal(new Point(20, 10), control.Current);
        Assert.Equal(default, control.Delta);
    }

    [AvaloniaFact]
    public void Moving_While_Dragging_Tracks_The_Pointer_And_The_Delta()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Left);
        window.MouseMove(new Point(112, 106));
        Drain();

        Assert.True(control.IsDragging);
        Assert.Equal(new Point(32, 16), control.Current);
        Assert.Equal(new Vector(12, 6), control.Delta);
        Assert.Equal(1, control.TrackedMoves);
    }

    // The state machine's point: a pointer crossing the control without a button
    // down is the common case, and it must not move the drag state. An
    // implementation that updates Current unconditionally passes the test above
    // and fails here.
    //
    // The positive half at the end is not padding. A test that only asserts that
    // nothing happened cannot tell "the code correctly ignored this" from "no
    // input arrived at all", and the second is a real failure mode here: before
    // this control had a Background it received nothing, so every negative claim
    // about it held vacuously. Anchoring the negative to a real drag in the same
    // test fixes that, and makes it stricter too - an implementation tracking
    // moves unconditionally ends on two, not one.
    [AvaloniaFact]
    public void Moving_Without_A_Press_Tracks_Nothing()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseMove(new Point(112, 106));
        Drain();

        Assert.False(control.IsDragging);
        Assert.Equal(0, control.TrackedMoves);
        Assert.Equal(default, control.Current);
        Assert.Equal(default, control.Origin);

        window.MouseDown(new Point(112, 106), MouseButton.Left);
        window.MouseMove(new Point(120, 110));
        Drain();

        Assert.True(control.IsDragging);
        Assert.Equal(new Point(32, 16), control.Origin);
        Assert.Equal(1, control.TrackedMoves);
    }

    // Measured: a right-button press really does reach OnPointerPressed, with
    // IsRightButtonPressed set and IsLeftButtonPressed clear - so this is a
    // genuine branch the solution has to take, not a case the harness swallows.
    //
    // Anchored to a real drag for the same reason as above - a negative that holds
    // because nothing arrived proves nothing. It also sharpens the claim: had the
    // right press begun a drag, the move before the left press would already have
    // been tracked and the count would end on two.
    [AvaloniaFact]
    public void A_Right_Press_Starts_Nothing()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Right);
        window.MouseMove(new Point(112, 106));
        Drain();

        Assert.False(control.IsDragging);
        Assert.Equal(0, control.TrackedMoves);
        Assert.Equal(default, control.Origin);

        window.MouseUp(new Point(112, 106), MouseButton.Right);
        window.MouseDown(new Point(112, 106), MouseButton.Left);
        window.MouseMove(new Point(120, 110));
        Drain();

        Assert.True(control.IsDragging);
        Assert.Equal(new Point(32, 16), control.Origin);
        Assert.Equal(1, control.TrackedMoves);
    }

    [AvaloniaFact]
    public void Releasing_Ends_The_Drag_But_Keeps_What_It_Measured()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Left);
        window.MouseMove(new Point(112, 106));
        window.MouseUp(new Point(112, 106), MouseButton.Left);
        Drain();

        Assert.False(control.IsDragging);
        Assert.Equal(new Point(20, 10), control.Origin);
        Assert.Equal(new Point(32, 16), control.Current);
        Assert.Equal(new Vector(12, 6), control.Delta);
    }

    // After a release the control is idle again, so a later move must be ignored
    // exactly as one before the first press was. This catches an implementation
    // that ends the drag by clearing Origin instead of the flag.
    [AvaloniaFact]
    public void Moving_After_A_Release_Tracks_Nothing_Further()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Left);
        window.MouseMove(new Point(112, 106));
        window.MouseUp(new Point(112, 106), MouseButton.Left);
        Drain();
        var tracked = control.TrackedMoves;

        window.MouseMove(new Point(130, 120));
        Drain();

        Assert.Equal(tracked, control.TrackedMoves);
        Assert.Equal(new Point(32, 16), control.Current);
    }

    [AvaloniaFact]
    public void A_Second_Drag_Starts_From_Its_Own_Origin()
    {
        var (control, window) = Shown();

        window.MouseMove(Centre);
        window.MouseDown(Centre, MouseButton.Left);
        window.MouseMove(new Point(112, 106));
        window.MouseUp(new Point(112, 106), MouseButton.Left);
        window.MouseDown(new Point(105, 95), MouseButton.Left);
        window.MouseMove(new Point(95, 95));
        Drain();

        Assert.True(control.IsDragging);
        Assert.Equal(new Point(25, 5), control.Origin);
        Assert.Equal(new Point(15, 5), control.Current);
        Assert.Equal(new Vector(-10, 0), control.Delta);
    }
}
