using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 079 - KeyBindingsAndAccelerators (advanced).
/// Goal:   Give a control keyboard accelerators the way Avalonia means you to -
///         a KeyBindings collection of KeyGesture-to-command pairs - rather than a
///         KeyDown handler with a switch in it. Get the modifier matching right:
///         Ctrl+S and Ctrl+Shift+S are different accelerators, and plain S is
///         neither.
/// Drills: InputElement.KeyBindings, KeyBinding, KeyGesture with KeyModifiers,
///         ReactiveCommand as an ICommand, KeyGesture.Parse.
/// Passes: dotnet test --filter FullyQualifiedName~Ex079_
///
/// A KeyBinding does not need the control itself to be focused. Measured: with the
/// focus on a TextBox inside this panel, accelerators declared on the PANEL still
/// fired, because the key event bubbles up from the focused element and each
/// ancestor gets to match its own bindings. That is the whole reason accelerators
/// are usually declared high up in a window rather than on every leaf.
public class Ex079_KeyBindingsAndAccelerators : StackPanel
{
    /// <summary>Given. Do not change. One entry per accelerator that fired, in order.</summary>
    public List<string> Invoked { get; } = [];

    /// <summary>Given. Do not change. A focusable child, so key events have somewhere to start.</summary>
    public TextBox Editor { get; } = new() { Width = 120 };

    /// <summary>
    /// Register three accelerators on this panel's KeyBindings:
    ///
    ///   Ctrl+S        appends "save"
    ///   Ctrl+Shift+S  appends "saveAs"
    ///   Delete        appends "delete"
    ///
    /// Each command may be a ReactiveCommand.Create(...) - a ReactiveCommand is an
    /// ICommand, which is all a KeyBinding wants.
    ///
    /// Mind the difference between Ctrl+S and Ctrl+Shift+S. A gesture matches only
    /// when the modifiers match exactly, so the two do not shadow each other - but
    /// only if you actually spell both out. The test presses all four combinations.
    /// </summary>
    private void Bind() =>
        throw new NotImplementedException(
            "TODO: Ex079 - add three KeyBindings to this panel: Ctrl+S, Ctrl+Shift+S " +
            "and Delete, whose commands append \"save\", \"saveAs\" and \"delete\" to " +
            "Invoked");

    /// <summary>
    /// The accelerator to show next to a menu item, as text. Given a gesture string
    /// such as "Ctrl+Shift+P", parse it and return the gesture's own display form.
    ///
    /// This is what a menu's InputGesture ends up rendering, and parsing rather
    /// than hand-formatting is the point: KeyGesture.Parse accepts what a user or a
    /// config file would write.
    /// </summary>
    public static string Describe(string gesture) =>
        throw new NotImplementedException(
            "TODO: Ex079 - KeyGesture.Parse the string and return its ToString()");

    public Ex079_KeyBindingsAndAccelerators()
    {
        Children.Add(Editor);
        Bind();
    }
}
