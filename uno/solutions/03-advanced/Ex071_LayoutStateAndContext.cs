// Exercise 071 - Layout State And Context (advanced).
// Goal:   Give a layout per-host state without making the layout itself stateful.
// Drills: why a field on a Layout is a bug, keying state by LayoutContext, and
//         ConditionalWeakTable so the state dies with the host.
// Passes: dotnet test --filter FullyQualifiedName~Ex071_
//
// A Layout is a strategy object and one instance can serve several hosts - two repeaters,
// or the same repeater re-hosted. Anything remembered between passes therefore belongs to
// the host, not to the layout: a field would be shared, and the second host would read the
// first host's measurements.
//
// WinUI's intended mechanism for this is LayoutContext.LayoutState, filled in
// InitializeForContextCore. Uno does not call that hook for a NonVirtualizingLayout (see
// uno/README.md), so the state is kept in a ConditionalWeakTable keyed by the context -
// which has the same lifetime behaviour: no host, no entry, and nothing to clean up.

using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>What a layout remembers about one host between passes.</summary>
public sealed class Ex071_LayoutState
{
    /// <summary>How many measure passes this host has had.</summary>
    public int MeasurePasses { get; set; }

    /// <summary>The widest child seen so far for this host.</summary>
    public double WidestChild { get; set; }
}

/// <summary>
/// Stacks children vertically and keeps its bookkeeping per host.
/// </summary>
public sealed class Ex071_LayoutStateAndContext : NonVirtualizingLayout
{
    // Weak keys: an entry disappears when its host does, so a long-lived layout does not
    // accumulate the measurements of every host it ever served.
    private readonly ConditionalWeakTable<LayoutContext, Ex071_LayoutState> _states = new();

    /// <summary>How many hosts this layout instance has state for.</summary>
    public int KnownContexts => _states.Count();

    /// <summary>
    /// The state for <paramref name="context"/>, created on first use.
    /// </summary>
    public Ex071_LayoutState StateFor(LayoutContext context) =>
        _states.GetValue(context, _ => new Ex071_LayoutState());

    protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
    {
        // Per host, every time. A `_widestChild` field here would be shared by every
        // repeater this layout serves, and which one won would depend on the order the
        // passes happened to run in.
        var state = StateFor(context);
        state.MeasurePasses++;

        var height = 0d;

        foreach (var child in context.Children)
        {
            child.Measure(availableSize);
            state.WidestChild = Math.Max(state.WidestChild, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(state.WidestChild, height);
    }

    protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
    {
        // Given: the arrange half is not this exercise's subject.
        var y = 0d;

        foreach (var child in context.Children)
        {
            child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        return finalSize;
    }

    /// <summary>
    /// An <see cref="ItemsRepeater"/> over <paramref name="items"/> using
    /// <paramref name="layout"/>, each item a Border of the item's own width, 10 high,
    /// aligned top-left.
    /// </summary>
    public static ItemsRepeater CreateRepeater(IEnumerable<double> items, Ex071_LayoutStateAndContext layout) => new()
    {
        ItemsSource = items.ToList(),
        ItemTemplate = ItemTemplate,
        Layout = layout,
    };

    /// <summary>Given: the item template.</summary>
    protected static readonly DataTemplate ItemTemplate = (DataTemplate)XamlReader.Load(
        """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <Border Width="{Binding}" Height="10" HorizontalAlignment="Left" VerticalAlignment="Top" />
        </DataTemplate>
        """);
}
