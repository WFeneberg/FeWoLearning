// Exercise 075 - Computed Value Caching (advanced).
// Goal:   Compute an expensive derived value once, and know exactly when to forget it.
// Drills: a cache invalidated by the changed callbacks of the properties it reads, a
//         property that is *not* an input not invalidating it, and no work at all until
//         somebody asks.
// Passes: dotnet test --filter FullyQualifiedName~Ex075_
//
// A control's derived value - a formatted string, a geometry, a measured text size - is
// read on every layout pass and changes far less often. Caching it is easy; invalidating
// it correctly is the exercise, and the two failure modes are opposite: forgetting an input
// leaves a stale value on screen, and invalidating on everything makes the cache useless
// while looking like it works.

using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// Formats a range as text, cached until one of the two bounds changes.
/// </summary>
public partial class Ex075_ComputedValueCaching : DependencyObject
{
    private string? _cached;

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(int),
            typeof(Ex075_ComputedValueCaching),
            new PropertyMetadata(0, OnInputChanged));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(Ex075_ComputedValueCaching),
            new PropertyMetadata(10, OnInputChanged));

    /// <summary>
    /// Not an input to the computed value. Changing it must not throw the cache away.
    /// </summary>
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(Ex075_ComputedValueCaching),
            new PropertyMetadata(""));

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>How many times the value has actually been computed.</summary>
    public int Computations { get; private set; }

    /// <summary>
    /// The range as <c>"min..max"</c>, computed on first use and then reused until an input
    /// changes.
    /// </summary>
    public string RangeText =>
        // TODO: return the cache when it is there, otherwise compute it, count the
        // computation, store it and return it.
        throw new NotImplementedException("TODO: Ex075 - compute once, then reuse");

    /// <summary>Forgets the cached value.</summary>
    public void Invalidate() =>
        throw new NotImplementedException("TODO: Ex075 - drop the cache");

    private static void OnInputChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        // TODO: an input moved, so the cached value is wrong now.
        throw new NotImplementedException("TODO: Ex075 - invalidate when an input changes");
}
