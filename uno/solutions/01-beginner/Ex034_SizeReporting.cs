// Exercise 034 - Size Reporting (beginner).
// Goal:   Tell apart the three numbers people all call "the width".
// Drills: Width as a *request* that is NaN when unset, DesiredSize as the answer to a
//         measure (margin included), and ActualWidth as the size the arrange pass gave.
// Passes: dotnet test --filter FullyQualifiedName~Ex034_
//
// "Why is Width 0?" nearly always means ActualWidth was read before layout, or Width was
// read expecting the measured size. They are different questions with different answers.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Beginner;

/// <summary>
/// The three widths of an element after a layout pass, plus whether a width was requested.
/// </summary>
public sealed record SizeReport(double RequestedWidth, double DesiredWidth, double ActualWidth, bool IsExplicit);

public static class Ex034_SizeReporting
{
    /// <summary>
    /// Lays <paramref name="element"/> out in <paramref name="available"/> and reports:
    /// <list type="bullet">
    ///   <item><c>RequestedWidth</c> - what was asked for, NaN when nothing was,</item>
    ///   <item><c>DesiredWidth</c> - what the measure pass concluded,</item>
    ///   <item><c>ActualWidth</c> - what the arrange pass handed over,</item>
    ///   <item><c>IsExplicit</c> - whether a width was requested at all.</item>
    /// </list>
    /// </summary>
    public static SizeReport Measure(FrameworkElement element, Size available)
    {
        element.Measure(available);

        // Both passes, in this order. Measure fills DesiredSize, arrange fills
        // ActualWidth - and only arrange knows how much of the slot the element was
        // actually given.
        element.Arrange(new Rect(0, 0, available.Width, available.Height));

        return new SizeReport(
            // NaN unless somebody asked for a width. That is the sentinel, not 0 - and
            // double.IsNaN is the only correct way to test for it.
            RequestedWidth: element.Width,
            DesiredWidth: element.DesiredSize.Width,
            ActualWidth: element.ActualWidth,
            IsExplicit: !double.IsNaN(element.Width));
    }
}
