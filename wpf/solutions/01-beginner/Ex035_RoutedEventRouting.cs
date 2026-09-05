// Exercise 035 - Routed event routing: bubble, tunnel and Handled (beginner). REFERENCE SOLUTION.
// Goal:   Register a genuine custom routed event pair - a tunnelling Preview* counterpart
//         plus the bubbling event it precedes, mirroring WPF's own PreviewMouseDown/
//         MouseDown convention - and raise both from one call so the tunnel phase runs top
//         down before the bubble phase runs bottom up, with Handled able to cut the bubble
//         phase short.
// Drills: EventManager.RegisterRoutedEvent (both events are ready to use here - the
//         registration call itself has no branching logic to get wrong; PreviewItemActivatedEvent
//         and ItemActivatedEvent below are asserted directly for Name, RoutingStrategy,
//         HandlerType and OwnerType, since an unasserted registration is exactly the mistake
//         this track has shipped twice before) and RaiseEvent/Handled: RaiseItemActivatedPair
//         is the actual subject - it must raise the SAME RoutedEventArgs instance for both the
//         tunnel and the bubble phase (by reassigning args.RoutedEvent between the two
//         RaiseEvent calls, not constructing a second RoutedEventArgs), which is what makes a
//         Handled=true set during the tunnel phase still be true once the bubble phase starts
//         - and AddHandler(..., handledEventsToo: true) is the only way a handler still sees
//         an event once something upstream already marked it Handled.

using System.Windows;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public static class Ex035_RoutedEventRouting
{
    /// <summary>
    /// Ready to use - not the subject of this row. The tunnelling ("Preview") half of the
    /// pair - tunnel events run from the root down to the event source.
    /// </summary>
    public static readonly RoutedEvent PreviewItemActivatedEvent = EventManager.RegisterRoutedEvent(
        "PreviewItemActivated", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(Ex035_RoutedEventRouting));

    /// <summary>
    /// Ready to use - not the subject of this row. The bubbling half of the pair - bubble
    /// events run from the event source up to the root.
    /// </summary>
    public static readonly RoutedEvent ItemActivatedEvent = EventManager.RegisterRoutedEvent(
        "ItemActivated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Ex035_RoutedEventRouting));

    /// <summary>
    /// Raises PreviewItemActivatedEvent (tunnel) from <paramref name="source"/>, then raises
    /// ItemActivatedEvent (bubble) from the SAME source using the SAME RoutedEventArgs
    /// instance - so a handler that sets Handled=true during the tunnel phase leaves the
    /// bubble phase already Handled when it starts. Returns that RoutedEventArgs instance.
    /// </summary>
    public static RoutedEventArgs RaiseItemActivatedPair(UIElement source)
    {
        var args = new RoutedEventArgs(PreviewItemActivatedEvent, source);
        source.RaiseEvent(args);
        args.RoutedEvent = ItemActivatedEvent;
        source.RaiseEvent(args);
        return args;
    }
}
