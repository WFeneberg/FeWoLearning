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
    public void Attach(FrameworkElement element)
    {
        if (AssociatedObject is not null)
        {
            // Idempotent from the caller's side. A behaviour attached twice subscribes
            // twice, and then every event fires twice.
            return;
        }

        // Set before OnAttached, because a behaviour has nothing to subscribe to otherwise.
        AssociatedObject = element;
        Attachments++;
        OnAttached();
    }

    /// <summary>
    /// Detaches. Detaching a detached behaviour does nothing.
    /// </summary>
    public void Detach()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        // OnDetaching first: unsubscribing needs the element that was subscribed to.
        OnDetaching();
        AssociatedObject = null;
        Detachments++;
    }

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

    private static void OnBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // Old set first, unconditionally: attaching the new one before releasing the old
        // leaves two sets of subscriptions on the same element.
        if (args.OldValue is Ex096_BehaviorCollection old)
        {
            foreach (var behavior in old)
            {
                behavior.Detach();
            }
        }

        // Markup can attach this to anything, so a cast would take the app down at parse
        // time for a typo in a style.
        if (sender is not FrameworkElement element || args.NewValue is not Ex096_BehaviorCollection added)
        {
            return;
        }

        foreach (var behavior in added)
        {
            behavior.Attach(element);
        }
    }
}
