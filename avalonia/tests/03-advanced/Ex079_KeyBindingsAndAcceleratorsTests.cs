using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex079_KeyBindingsAndAcceleratorsTests
{
    private static (Ex079_KeyBindingsAndAccelerators Panel, Window Window) Shown()
    {
        var panel = new Ex079_KeyBindingsAndAccelerators();
        var window = ViewHarness.ShowWindow(panel, 240, 120);

        // The focus starts on the child, so the key event begins there and bubbles
        // up to the panel's own bindings - which is how accelerators declared high
        // in a tree reach keys pressed low in it.
        panel.Editor.Focus();
        Dispatcher.UIThread.RunJobs();
        return (panel, window);
    }

    // KeyPress needs a PhysicalKey in Avalonia 12.1.1 - the three-argument overload
    // that older samples use no longer exists - so the physical key is passed
    // explicitly rather than left to a default.
    private static void Press(Window window, Key key, PhysicalKey physical, RawInputModifiers modifiers)
    {
        window.KeyPress(key, modifiers, physical, null);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void Ctrl_S_Invokes_Save()
    {
        var (panel, window) = Shown();

        Press(window, Key.S, PhysicalKey.S, RawInputModifiers.Control);

        Assert.Equal(["save"], panel.Invoked);
    }

    // The pair that shadows each other when the gestures are written carelessly:
    // a Ctrl+S binding that ignores Shift swallows this too, and then "saveAs"
    // never fires.
    [AvaloniaFact]
    public void Ctrl_Shift_S_Invokes_SaveAs_And_Not_Save()
    {
        var (panel, window) = Shown();

        Press(window, Key.S, PhysicalKey.S, RawInputModifiers.Control | RawInputModifiers.Shift);

        Assert.Equal(["saveAs"], panel.Invoked);
    }

    [AvaloniaFact]
    public void An_Accelerator_Without_Its_Modifier_Does_Not_Fire()
    {
        var (panel, window) = Shown();

        Press(window, Key.S, PhysicalKey.S, RawInputModifiers.None);

        Assert.Empty(panel.Invoked);
    }

    [AvaloniaFact]
    public void A_Modifierless_Accelerator_Fires_On_Its_Own()
    {
        var (panel, window) = Shown();

        Press(window, Key.Delete, PhysicalKey.Delete, RawInputModifiers.None);

        Assert.Equal(["delete"], panel.Invoked);
    }

    // Adding the modifier to a gesture that does not ask for one must NOT match,
    // for the same exact-match reason as above, read from the other side.
    [AvaloniaFact]
    public void A_Modifierless_Accelerator_Does_Not_Fire_With_A_Modifier()
    {
        var (panel, window) = Shown();

        Press(window, Key.Delete, PhysicalKey.Delete, RawInputModifiers.Control);

        Assert.Empty(panel.Invoked);
    }

    [AvaloniaFact]
    public void Several_Accelerators_Fire_In_The_Order_They_Were_Pressed()
    {
        var (panel, window) = Shown();

        Press(window, Key.Delete, PhysicalKey.Delete, RawInputModifiers.None);
        Press(window, Key.S, PhysicalKey.S, RawInputModifiers.Control);
        Press(window, Key.S, PhysicalKey.S, RawInputModifiers.Control | RawInputModifiers.Shift);

        Assert.Equal(["delete", "save", "saveAs"], panel.Invoked);
    }

    [AvaloniaTheory]
    [InlineData("Ctrl+Shift+P", "Ctrl+Shift+P")]
    [InlineData("Ctrl+S", "Ctrl+S")]
    [InlineData("Delete", "Delete")]
    [InlineData("Alt+F4", "Alt+F4")]
    public void Describe_Parses_A_Gesture_And_Renders_It_Back(string input, string expected)
    {
        Assert.Equal(expected, Ex079_KeyBindingsAndAccelerators.Describe(input));
    }
}
