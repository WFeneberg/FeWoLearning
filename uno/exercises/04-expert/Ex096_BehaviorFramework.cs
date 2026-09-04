// Exercise 096 - Behavior Framework (expert).
// Goal:   Build the attach/detach machinery that every XAML behaviour toolkit provides.
// Drills: a base class with a strongly typed AssociatedObject, an attached collection that
//         attaches on assignment, and detaching everything the previous collection owned.
// Passes: dotnet test --filter FullyQualifiedName~Ex096_
//
// ex042 wrote one behaviour by hand. This is the framework underneath: markup assigns a
// collection, and the framework guarantees Attach happens once, Detach happens exactly
// once, and neither happens twice - because a behaviour that attaches twice subscribes
// twice, and one that never detaches keeps the element alive.
//
// The rule worth stating: attaching is idempotent from the caller's point of view, and the
// framework owns the lifetime, not the behaviour.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>What every behaviour implements.</summary>
public abstract class Ex096_Behavior
{
    /// <summary>The element this behaviour is attached to, or null while detached.</summary>
    public FrameworkElement? AssociatedObject { get; private set; }

    /// <summary>How many times this behaviour has been attached.</summary>
    public int Attachments { get; private set; }

    /// <summary>How many times it has been detached.</summary>
    public int Detachments { get; private set; }

    /// <summary>
    /// Attaches to <paramref name="element"/>. Attaching an already-attached behaviour does
    /// nothing - and must not run <see cref="OnAttached"/> a second time.
    /// </summary>
    public void Attach(FrameworkElement element) =>
        // TODO: guard, record the element, count it, and call OnAttached.
        throw new NotImplementedException("TODO: Ex096 - attach once");

    /// <summary>
    /// Detaches. Detaching a detached behaviour does nothing.
    /// </summary>
    public void Detach() =>
        // TODO: guard, call OnDetaching *before* letting go of the element - it still needs
        // it to unsubscribe - then clear it and count the detachment.
        throw new NotImplementedException("TODO: Ex096 - detach once");

    /// <summary>Subscribe here.</summary>
    protected virtual void OnAttached()
    {
    }

    /// <summary>Unsubscribe here. <see cref="AssociatedObject"/> is still set.</summary>
    protected virtual void OnDetaching()
    {
    }
}

/// <summary>A list of behaviours, as markup would declare it.</summary>
public sealed class Ex096_BehaviorCollection : List<Ex096_Behavior>;

public static class Ex096_BehaviorFramework
{
    /// <summary>
    /// The attached property markup assigns. Given: the registration, so the exercise is
    /// the lifetime handling in the callback.
    /// </summary>
    public static readonly DependencyProperty BehaviorsProperty =
        DependencyProperty.RegisterAttached(
            "Behaviors",
            typeof(Ex096_BehaviorCollection),
            typeof(Ex096_BehaviorFramework),
            new PropertyMetadata(null, OnBehaviorsChanged));

    public static Ex096_BehaviorCollection? GetBehaviors(DependencyObject element) =>
        (Ex096_BehaviorCollection?)element.GetValue(BehaviorsProperty);

    public static void SetBehaviors(DependencyObject element, Ex096_BehaviorCollection? value) =>
        element.SetValue(BehaviorsProperty, value);

    private static void OnBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        // TODO: detach everything in the old collection, then attach everything in the new
        // one to this element. Anything that is not a FrameworkElement is ignored - markup
        // can attach this anywhere.
        //
        // Detach first, and in that order: attaching the new set before releasing the old
        // one leaves two sets of subscriptions on the same element.
        throw new NotImplementedException("TODO: Ex096 - swap the behaviour sets");
}
