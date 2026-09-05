using System.Windows;
using System.Windows.Controls;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex035_RoutedEventRoutingTests : WpfTestContext
{
    [WpfFact]
    public void RaiseItemActivatedPair_Tunnels_Down_Then_Bubbles_Up()
    {
        // The registration metadata itself - checked here rather than in a standalone test,
        // so that a stub-red run never lets one of this row's tests pass without the actual
        // subject (RaiseItemActivatedPair, below) ever running. An unasserted registration
        // name/strategy/owner is exactly the mistake this track has shipped twice before.
        var preview = Ex035_RoutedEventRouting.PreviewItemActivatedEvent;
        Assert.Equal("PreviewItemActivated", preview.Name);
        Assert.Equal(RoutingStrategy.Tunnel, preview.RoutingStrategy);
        Assert.Equal(typeof(RoutedEventHandler), preview.HandlerType);
        Assert.Equal(typeof(Ex035_RoutedEventRouting), preview.OwnerType);

        var grandparent = new StackPanel();
        var parent = new StackPanel();
        var child = new Border();
        parent.Children.Add(child);
        grandparent.Children.Add(parent);

        var order = new List<string>();
        grandparent.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("gp-preview")));
        parent.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("parent-preview")));
        child.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("child-preview")));

        child.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("child-bubble")));
        parent.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("parent-bubble")));
        grandparent.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("gp-bubble")));

        Ex035_RoutedEventRouting.RaiseItemActivatedPair(child);

        // Tunnel top-down (root to source), THEN bubble bottom-up (source to root) - the
        // one order this mechanism can produce; a bubble-then-tunnel or all-at-once
        // implementation could not reproduce this exact sequence.
        Assert.Equal(
            new[] { "gp-preview", "parent-preview", "child-preview", "child-bubble", "parent-bubble", "gp-bubble" },
            order);
    }

    [WpfFact]
    public void A_Different_Tree_Still_Tunnels_Then_Bubbles_In_The_Same_Order()
    {
        // Same reasoning as the bubble-event metadata above: checked inline so this test's
        // pass depends on RaiseItemActivatedPair actually running, not on a standalone
        // metadata-only test that would pass even against the untouched stub.
        var bubble = Ex035_RoutedEventRouting.ItemActivatedEvent;
        Assert.Equal("ItemActivated", bubble.Name);
        Assert.Equal(RoutingStrategy.Bubble, bubble.RoutingStrategy);
        Assert.Equal(typeof(RoutedEventHandler), bubble.HandlerType);
        Assert.Equal(typeof(Ex035_RoutedEventRouting), bubble.OwnerType);

        // A differently-shaped tree (only two levels, not three) than the test above - a
        // hard-coded three-entry expectation cannot satisfy both.
        var parent = new StackPanel();
        var child = new Border();
        parent.Children.Add(child);

        var order = new List<string>();
        parent.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("parent-preview")));
        child.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("child-preview")));
        child.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("child-bubble")));
        parent.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("parent-bubble")));

        Ex035_RoutedEventRouting.RaiseItemActivatedPair(child);

        Assert.Equal(new[] { "parent-preview", "child-preview", "child-bubble", "parent-bubble" }, order);
    }

    [WpfFact]
    public void Setting_Handled_During_The_Preview_Phase_Stops_The_Bubble_Phase_Except_For_HandledEventsToo()
    {
        var grandparent = new StackPanel();
        var parent = new StackPanel();
        var child = new Border();
        parent.Children.Add(child);
        grandparent.Children.Add(parent);

        var order = new List<string>();
        parent.AddHandler(Ex035_RoutedEventRouting.PreviewItemActivatedEvent, new RoutedEventHandler((_, e) =>
        {
            order.Add("parent-preview");
            e.Handled = true;
        }));

        child.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("child-bubble")));
        parent.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("parent-bubble")));
        grandparent.AddHandler(Ex035_RoutedEventRouting.ItemActivatedEvent, new RoutedEventHandler((_, _) => order.Add("gp-bubble")));
        grandparent.AddHandler(
            Ex035_RoutedEventRouting.ItemActivatedEvent,
            new RoutedEventHandler((_, e) => order.Add($"gp-bubble-handledEventsToo({e.Handled})")),
            handledEventsToo: true);

        var args = Ex035_RoutedEventRouting.RaiseItemActivatedPair(child);

        // The distinguishing check: every plain bubble handler (child-bubble, parent-bubble,
        // gp-bubble) is skipped because Handled was already true when the bubble phase
        // started - which only holds if the tunnel and bubble raises share the SAME
        // RoutedEventArgs instance. Only the handledEventsToo:true handler still runs, and
        // it sees Handled already true.
        Assert.Equal(new[] { "parent-preview", "gp-bubble-handledEventsToo(True)" }, order);
        Assert.True(args.Handled);
    }
}
