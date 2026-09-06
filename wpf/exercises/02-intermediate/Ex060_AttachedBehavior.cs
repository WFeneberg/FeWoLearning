// Exercise 060 - Replacing code-behind with an attached-property behavior (intermediate).
// Goal:   Code-behind that wires "when this TextBox's text changes, force it uppercase" directly
//         in a Window/UserControl constructor only ever works for that one control instance, and
//         couples the behavior to the code-behind file. An attached-property behavior is the same
//         logic, reusable on any element: set the property to opt in, clear it to opt back out -
//         the property's own PropertyChangedCallback does the subscribing and unsubscribing that
//         constructor code used to do by hand.
// Drills: RegisterAttached with a PropertyChangedCallback that subscribes an event handler when
//         the value flips to true and UNSUBSCRIBES it when the value flips back to false - a
//         behavior attached and later cleared must leave NO trace, not merely stop being useful -
//         and a genuinely load-bearing reentrancy guard. A first attempt at this row transformed
//         Text with a plain ToUpperInvariant() and warned about "infinite re-entrant recursion" -
//         measured directly to be FALSE: WPF raises no further TextChanged for writing back an
//         IDENTICAL string, so that transform re-enters at most once whether or not it is guarded,
//         making the guard untestable. This row's transform is deliberately NOT idempotent instead
//         (it appends a trailing marker character every time it runs) specifically so the guard is
//         real: unguarded, each reentrant write is a DIFFERENT string from the one before it, so
//         WPF's identical-string check never saves you and the handler recurses without end. A
//         wrong implementation here does not merely fail an assertion - like row 052's
//         RunWorkerCompletedEventArgs.Result trap, it can crash the test host outright
//         (StackOverflowException is unrecoverable in .NET); this is a real, measured hazard of
//         this exercise, not a theoretical one.
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
        => throw new NotImplementedException("TODO: Ex060 - if sender is a TextBox whose Text does NOT already end with a trailing '*' marker, replace Text with its own upper-invariant form followed by one appended '*' - check that condition FIRST. This transform is NOT idempotent: every application changes the string (it always appends another marker), so without the check each reentrant TextChanged still finds text needing normalization and the handler recurses without end; guarded correctly, it stops after exactly one reentrant call, whose own already-marked result satisfies the check");
}
