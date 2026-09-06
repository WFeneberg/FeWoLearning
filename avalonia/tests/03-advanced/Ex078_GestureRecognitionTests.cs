using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex078_GestureRecognitionTests
{
    private static readonly Point Centre = new(100, 100);

    private static (Ex078_GestureRecognition Control, Window Window) Shown()
    {
        var control = new Ex078_GestureRecognition { Width = 40, Height = 20 };
        var window = ViewHarness.ShowWindow(control, 200, 200);
        window.MouseMove(Centre);
        Dispatcher.UIThread.RunJobs();
        return (control, window);
    }

    private static void Click(Window window)
    {
        window.MouseDown(Centre, MouseButton.Left);
        window.MouseUp(Centre, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void A_Press_And_Release_Is_One_Tap()
    {
        var (control, window) = Shown();

        Click(window);

        Assert.Equal(["tap"], control.Log);
    }

    // Avalonia raises Tapped for the second click as well and DoubleTapped on top
    // of it, rather than replacing one with the other - which is worth pinning,
    // because a handler that acts on both fires twice for one double click.
    [AvaloniaFact]
    public void The_Second_Click_Adds_A_Double_Tap_On_Top_Of_Its_Own_Tap()
    {
        var (control, window) = Shown();

        Click(window);
        Click(window);

        Assert.Equal(["tap", "doubleTap"], control.Log);
    }

    [AvaloniaFact]
    public void A_Wheel_Turn_Is_Its_Own_Gesture_And_Carries_A_Delta()
    {
        var (control, window) = Shown();

        window.MouseWheel(Centre, new Vector(0, -3));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["wheel"], control.Log);
        Assert.Equal(-3, control.Scrolled);
    }

    // Accumulating rather than overwriting, and the sign is kept: two turns the
    // same way add up, and one back subtracts.
    [AvaloniaFact]
    public void Wheel_Deltas_Accumulate()
    {
        var (control, window) = Shown();

        window.MouseWheel(Centre, new Vector(0, -3));
        window.MouseWheel(Centre, new Vector(0, -2));
        window.MouseWheel(Centre, new Vector(0, 4));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(3, control.Log.Count(entry => entry == "wheel"));
        Assert.Equal(-1, control.Scrolled);
    }

    // The X axis is deliberately ignored by the contract, so a horizontal turn
    // still counts as a wheel gesture but moves nothing.
    [AvaloniaFact]
    public void Only_The_Y_Axis_Of_A_Wheel_Turn_Is_Accumulated()
    {
        var (control, window) = Shown();

        window.MouseWheel(Centre, new Vector(5, 0));
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(["wheel"], control.Log);
        Assert.Equal(0, control.Scrolled);
    }

    // Registering a recognizer is a declaration, not a handler: it is what makes
    // the control eligible for the gesture at all. Nothing in this harness can
    // produce a pull, so its firing is not asserted - see the exercise header.
    [AvaloniaFact]
    public void The_Control_Registers_A_Pull_Gesture_Recognizer()
    {
        var (control, _) = Shown();

        Assert.Single(control.GestureRecognizers.OfType<PullGestureRecognizer>());
    }
}
