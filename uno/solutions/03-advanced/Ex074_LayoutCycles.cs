// Exercise 074 - Layout Cycles (advanced).
// Goal:   Ask for a second measure from inside arrange without asking forever.
// Drills: why invalidating measure during arrange loops, a generation guard that allows
//         exactly one re-measure per real change, and re-arming it when the input changes.
// Passes: dotnet test --filter FullyQualifiedName~Ex074_
//
// Sometimes a panel only learns its real size during arrange - text that wrapped, a child
// that reported one thing and used another. Asking for another measure pass is legitimate.
// Asking on *every* arrange is a layout cycle: measure, arrange, invalidate, measure, ...
// and the app pins a core at 100% with no exception anywhere.
//
// The guard is the whole exercise: one request per genuinely new situation, and none for
// an arrange that changed nothing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A panel that wants to be as wide as its widest child, but only finds out how wide the
/// children really are once they have been arranged.
/// </summary>
public partial class Ex074_LayoutCycles : Panel
{
    /// <summary>How many times MeasureOverride has run.</summary>
    public int MeasurePasses { get; private set; }

    /// <summary>How many times ArrangeOverride has run.</summary>
    public int ArrangePasses { get; private set; }

    /// <summary>How many times arrange has asked for another measure pass.</summary>
    public int ReMeasureRequests { get; private set; }

    /// <summary>The width the last measure pass reported.</summary>
    public double MeasuredWidth { get; private set; }

    /// <summary>
    /// The width arrange discovered, or NaN before the first arrange. Set by the test to
    /// simulate "the children turned out wider than measure thought".
    /// </summary>
    public double DiscoveredWidth { get; set; } = double.NaN;

    protected override Size MeasureOverride(Size availableSize)
    {
        MeasurePasses++;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
        }

        // If arrange has discovered a width, honour it - that is what the re-measure was
        // requested for.
        MeasuredWidth = double.IsNaN(DiscoveredWidth)
            ? Children.Select(child => child.DesiredSize.Width).DefaultIfEmpty(0).Max()
            : DiscoveredWidth;

        return new Size(MeasuredWidth, Children.Sum(child => child.DesiredSize.Height));
    }

    // What the last re-measure was asked about. The guard is not a bool: a latch that
    // never re-arms would leave the panel at the first width it ever discovered.
    private double _requestedFor = double.NaN;

    protected override Size ArrangeOverride(Size finalSize)
    {
        ArrangePasses++;

        var y = 0d;

        foreach (var child in Children)
        {
            child.Arrange(new Rect(0, y, child.DesiredSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        if (double.IsNaN(DiscoveredWidth) || DiscoveredWidth.Equals(MeasuredWidth))
        {
            // Nothing new. This branch is the one that keeps the loop from existing.
            return finalSize;
        }

        if (DiscoveredWidth.Equals(_requestedFor))
        {
            // Already asked about this value. Asking again is the cycle: measure, arrange,
            // invalidate, measure, ... with the app pinning a core and no exception to
            // point at.
            return finalSize;
        }

        _requestedFor = DiscoveredWidth;
        ReMeasureRequests++;
        InvalidateMeasure();

        return finalSize;
    }
}
