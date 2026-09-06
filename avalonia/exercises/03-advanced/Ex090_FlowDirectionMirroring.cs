using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 090 - FlowDirectionMirroring (advanced).
/// Goal:   See what right-to-left actually does to a control tree: the property
///         inherits down it, a child can opt back out, and text really is laid out
///         from the other edge.
/// Drills: FlowDirection, property inheritance down the visual tree, an explicit
///         per-element override, TextBlock.TextLayout as the observable proof.
/// Passes: dotnet test --filter FullyQualifiedName~Ex090_
///
/// WHAT MIRRORING DOES *NOT* DO HERE, MEASURED, BECAUSE IT IS COUNTER-INTUITIVE.
/// Panel layout is not mirrored in any way a test can see. The same two Borders in
/// a horizontal StackPanel reported identical Bounds under LeftToRight and
/// RightToLeft (0,0,30,20 and 30,0,30,20 both times); a left-aligned child stayed
/// at x=0; RenderTransform stayed null; TransformToVisual was the identity in both
/// directions; and the internal HasMirrorTransform read false either way. Avalonia
/// mirrors non-text visuals on the render side, and this harness discards render
/// output - see the README section on rendering. So do not assert Bounds for this.
///
/// WHAT IT DOES DO, ALSO MEASURED, AND IT IS EXACT. Text layout mirrors properly.
/// A TextBlock 120 wide holding "abc" laid its single line out at Start = 0 under
/// LeftToRight and Start = 78 under RightToLeft - which is 120 minus the 42-unit
/// text width, i.e. flush against the other edge. HitTestTextPosition moved with
/// it, from x = 0 to x = 78 for the first character. That relationship, rather
/// than those numbers, is what the test asserts: the numbers depend on font
/// metrics, the relationship does not.
public static class Ex090_FlowDirectionMirroring
{
    /// <summary>Given. Do not change. Wide enough that the text cannot fill it.</summary>
    public const double LabelWidth = 120;

    /// <summary>Given. Do not change.</summary>
    public const string LabelText = "abc";

    /// <summary>
    /// A host laid out in <paramref name="direction"/>, containing a TextBlock
    /// named "Label" that shows LabelText and is LabelWidth wide.
    ///
    /// Set the direction on the HOST, not on the TextBlock: the point is that the
    /// label inherits it, and the test checks the label's own FlowDirection to
    /// prove the inheritance happened rather than being set twice.
    /// </summary>
    public static Control BuildHost(FlowDirection direction) =>
        throw new NotImplementedException(
            "TODO: Ex090 - a panel whose FlowDirection is direction, holding a " +
            "TextBlock named Label with Text LabelText and Width LabelWidth");

    /// <summary>
    /// A right-to-left host containing two TextBlocks: one named "Inherited" that
    /// takes the host's direction, and one named "OptedOut" that is explicitly
    /// LeftToRight. Both show LabelText at LabelWidth.
    ///
    /// This is the escape hatch that matters in practice - a code snippet, a
    /// serial number or a URL stays left-to-right inside an otherwise mirrored
    /// page - and it works because an explicit local value outranks an inherited
    /// one.
    /// </summary>
    public static Control BuildMixedHost() =>
        throw new NotImplementedException(
            "TODO: Ex090 - a RightToLeft panel holding a TextBlock named Inherited " +
            "with no direction of its own, and one named OptedOut set explicitly to " +
            "FlowDirection.LeftToRight");
}
