// Exercise 060 - Replacing code-behind with an attached-property behavior (intermediate).
// Goal:   Code-behind that wires "when this TextBox's text changes, force it uppercase" directly
//         in a Window/UserControl constructor only ever works for that one control instance, and
//         couples the behavior to the code-behind file. An attached-property behavior is the same
//         logic, reusable on any element: set the property to opt in, clear it to opt back out -
//         the property's own PropertyChangedCallback does the subscribing and unsubscribing that
//         constructor code used to do by hand.
// Drills: RegisterAttached with a PropertyChangedCallback that subscribes an event handler when
//         the value flips to true and UNSUBSCRIBES it when the value flips back to false - a
//         behavior attached and later cleared must leave NO trace, not merely stop being useful.
//         Measured directly: forcing Text to its own ToUpperInvariant() form inside a TextChanged
//         handler needs no reentrancy guard at all here - WPF raises no further TextChanged for
//         writing back an IDENTICAL string, so once the text is already upper-invariant, writing
//         it again raises nothing, and the handler re-enters at most once regardless.
// Passes: dotnet test --filter FullyQualifiedName~Ex060_

using System.Windows;
using System.Windows.Controls;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex060_AttachedBehavior
{
    public static readonly DependencyProperty AutoUppercaseProperty = DependencyProperty.RegisterAttached(
        "AutoUppercase",
        typeof(bool),
        typeof(Ex060_AttachedBehavior),
        new PropertyMetadata(false, OnAutoUppercaseChanged));

    public static bool GetAutoUppercase(DependencyObject element) => (bool)element.GetValue(AutoUppercaseProperty);

    public static void SetAutoUppercase(DependencyObject element, bool value) => element.SetValue(AutoUppercaseProperty, value);

    private static void OnAutoUppercaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => throw new NotImplementedException("TODO: Ex060 - if d is a TextBox: when the new value means the behavior is turning ON, subscribe OnTextChanged to its TextChanged event; when it means turning OFF, unsubscribe it - a value that was attached and later cleared must leave the TextBox with no handler at all, not merely an inert one");

    private static void OnTextChanged(object sender, TextChangedEventArgs e)
        => throw new NotImplementedException("TODO: Ex060 - if sender is a TextBox, replace its Text with its own upper-invariant form");
}
