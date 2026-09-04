// Exercise 042 - Attached Behavior (intermediate).
// Goal:   Add behaviour to a control you cannot subclass, from markup.
// Drills: an attached property whose changed callback subscribes and unsubscribes, a second
//         attached property as the behaviour's storage, and turning the behaviour off again.
// Passes: dotnet test --filter FullyQualifiedName~Ex042_
//
// This is the pattern behind every "Behavior" in every XAML toolkit: markup sets one
// attached property, and a callback wires the real work. The callback is the only place the
// subscription can be undone, so it has to handle both directions - and the false case is
// the one everybody forgets, which turns the behaviour into a leak.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Counts clicks on any <see cref="ButtonBase"/> it is switched on for, without the button
/// knowing anything about it.
/// </summary>
public static class Ex042_AttachedBehavior
{
    /// <summary>
    /// Switches the behaviour on or off for an element. Given: the registration, so the
    /// exercise is about the callback.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(Ex042_AttachedBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    /// <summary>Where the behaviour keeps its count - on the element it is attached to.</summary>
    public static readonly DependencyProperty ClickCountProperty =
        DependencyProperty.RegisterAttached(
            "ClickCount",
            typeof(int),
            typeof(Ex042_AttachedBehavior),
            new PropertyMetadata(0));

    public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

    public static int GetClickCount(DependencyObject element) => (int)element.GetValue(ClickCountProperty);

    public static void SetClickCount(DependencyObject element, int value) => element.SetValue(ClickCountProperty, value);

    private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // Markup can attach this to anything. A cast would take the app down at parse time
        // for a typo in a style, which is not a trade a behaviour gets to make.
        if (sender is not ButtonBase button)
        {
            return;
        }

        // Detach unconditionally before deciding, so a value that arrives twice cannot
        // leave two subscriptions behind. Removing a handler that is not attached is a
        // no-op, which is what makes this safe.
        button.Click -= OnClick;

        if ((bool)args.NewValue)
        {
            button.Click += OnClick;
        }
    }

    // A method group, not a lambda. `x.Click -= (s, e) => ...` compiles and removes
    // nothing: each lambda expression is a different delegate instance.
    private static void OnClick(object sender, RoutedEventArgs args)
    {
        var element = (DependencyObject)sender;
        SetClickCount(element, GetClickCount(element) + 1);
    }
}
