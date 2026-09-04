// Exercise 041 - Wrap Panel (intermediate).
// Goal:   A complete panel: measure, break lines, arrange, and report an honest size.
// Drills: both overrides working from the same rule, line breaking against the available
//         width, and a DesiredSize that matches what the arrange pass will actually do.
// Passes: dotnet test --filter FullyQualifiedName~Ex041_
//
// WinUI has no WrapPanel - this is the panel everybody writes first. The trap is that
// measure and arrange must agree: if the measure pass breaks lines differently from the
// arrange pass, the panel reports a height it does not use and the layout jitters.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// Places children left to right at their desired size, starting a new line whenever the
/// next child would not fit. Each line is as tall as its tallest child.
/// </summary>
public partial class Ex041_WrapPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize) =>
        // TODO: measure every child with the available size, walk them in order accumulating
        // a line width, and break to a new line when the next child would exceed
        // availableSize.Width. Return the width of the widest line and the sum of the line
        // heights.
        //
        // A child wider than the available width gets a line of its own rather than an
        // infinite loop.
        throw new NotImplementedException("TODO: Ex041 - measure and break into lines");

    protected override Size ArrangeOverride(Size finalSize) =>
        // TODO: arrange with the same rule, against finalSize this time, each child at its
        // own DesiredSize. Return finalSize.
        throw new NotImplementedException("TODO: Ex041 - arrange the lines");
}
