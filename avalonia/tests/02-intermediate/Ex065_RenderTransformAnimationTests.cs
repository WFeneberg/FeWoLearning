using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex065_RenderTransformAnimationTests
{
    private static Ex065_RenderTransformAnimation Show()
    {
        var view = ViewHarness.Show(new Ex065_RenderTransformAnimation(), 200, 200);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    // As in ex064: collected from the whole tree, because where the Style lives
    // is not what this exercise is about.
    private static IEnumerable<Style> AllStyles(Visual root) =>
        new StyledElement[] { root }
            .Concat(root.GetVisualDescendants().OfType<StyledElement>())
            .SelectMany(e => Flatten(e.Styles));

    private static IEnumerable<Style> Flatten(IEnumerable<IStyle> styles)
    {
        foreach (var style in styles)
        {
            if (style is Style typed) yield return typed;
            foreach (var nested in Flatten(style.Children.OfType<IStyle>())) yield return nested;
        }
    }

    // The half that needs no clock and no animation: a Transform's Value is the
    // Matrix it contributes, and a 2 by 3 scale is exactly M11 = 2, M22 = 3.
    [AvaloniaFact]
    public void The_Fixed_ScaleTransform_Produces_Its_Matrix()
    {
        var scaled = Show().FindControl<Border>("Scaled")!;

        var transform = Assert.IsType<ScaleTransform>(scaled.RenderTransform);
        Assert.Equal(2.0, transform.ScaleX);
        Assert.Equal(3.0, transform.ScaleY);
        Assert.Equal(2.0, scaled.RenderTransform!.Value.M11, precision: 6);
        Assert.Equal(3.0, scaled.RenderTransform!.Value.M22, precision: 6);
    }

    // Structural half of the animation. The Setter's Property is spelled
    // RotateTransform.Angle in markup, which resolves to the AngleProperty
    // OWNED BY RotateTransform - not to any property of the Border - and that
    // ownership is the thing worth asserting. Animating RenderTransform itself
    // is not an alternative spelling: it throws at style-attach time, so a
    // wrong answer there never reaches an assertion at all.
    [AvaloniaFact]
    public void The_Animation_Turns_A_Full_Circle_Over_Five_Seconds()
    {
        var animation = AllStyles(Show()).SelectMany(s => s.Animations).OfType<Animation>().Single();

        Assert.Equal(TimeSpan.FromSeconds(5), animation.Duration);
        Assert.True(animation.IterationCount.IsInfinite);
        Assert.Equal(2, animation.Children.Count);

        var frames = animation.Children.OrderBy(f => f.Cue.CueValue).ToList();
        var setters = frames.Select(f => Assert.Single(f.Setters.OfType<Setter>())).ToList();

        Assert.All(setters, s => Assert.Equal(RotateTransform.AngleProperty, s.Property));
        Assert.Equal(0.0, Assert.IsType<double>(setters[0].Value));
        Assert.Equal(360.0, Assert.IsType<double>(setters[1].Value));
    }

    // The attachment proof. A Border with no transform animation has a null
    // RenderTransform; one carrying a hand-written <RotateTransform Angle="45"/>
    // has a plain RotateTransform. Measured: animating a transform sub-property
    // makes Avalonia install a TransformGroup holding one transform of each
    // kind, so finding a RotateTransform inside a group is evidence that the
    // animation actually reached this control - which a declared-but-unmatched
    // selector would not achieve.
    //
    // The angle's value is NOT asserted: it is whatever the wall clock happened
    // to be at attach time and it never moves afterwards. See the .axaml header.
    [AvaloniaFact]
    public void The_Animation_Installed_A_Rotating_Transform_On_Spinner()
    {
        var spinner = Show().FindControl<Border>("Spinner")!;

        var group = Assert.IsType<TransformGroup>(spinner.RenderTransform);
        Assert.Single(group.Children.OfType<RotateTransform>());
    }

    // Part of the answer, not decoration: the default origin is the top left
    // corner, so a Border without this swings around a corner instead of
    // turning in place.
    //
    // The units are the trap, and they are why this is asserted as
    // RelativePoint.Center rather than as the numbers. Measured:
    // RenderTransformOrigin="0.5,0.5" parses to RelativeUnit.Absolute - half a
    // device pixel from the corner, near enough the default to change nothing.
    // Only the percentage spelling, "50%,50%", is relative to the control's own
    // size.
    [AvaloniaFact]
    public void Spinner_Turns_About_Its_Own_Centre()
    {
        var spinner = Show().FindControl<Border>("Spinner")!;

        Assert.Equal(RelativePoint.Center, spinner.RenderTransformOrigin);
        Assert.Equal(RelativeUnit.Relative, spinner.RenderTransformOrigin.Unit);
    }
}
