using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex064_KeyFrameAnimationTests
{
    private static Ex064_KeyFrameAnimation Show()
    {
        var view = ViewHarness.Show(new Ex064_KeyFrameAnimation(), 200, 160);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    // Collected from the whole tree rather than from view.Styles, so putting the
    // Style on an inner panel - which is what the stub suggests, and is
    // idiomatic - is not penalised. Where the markup lives is not the subject.
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

    private static Animation TheAnimation(Ex064_KeyFrameAnimation view) =>
        AllStyles(view).SelectMany(s => s.Animations).OfType<Animation>().Single();

    [AvaloniaFact]
    public void The_Animation_Is_Declared_With_The_Required_Timing()
    {
        var animation = TheAnimation(Show());

        Assert.Equal(TimeSpan.FromSeconds(5), animation.Duration);
        Assert.True(animation.IterationCount.IsInfinite);
        Assert.Equal(PlaybackDirection.Alternate, animation.PlaybackDirection);
    }

    // KeyFrame.Setters is typed as IAnimationSetter, whose Property and Value
    // are not publicly accessible in Avalonia 12.1.1 - the concrete items are
    // Avalonia.Styling.Setter, which is why this casts before reading them.
    [AvaloniaFact]
    public void Two_KeyFrames_Take_Opacity_From_One_To_Zero_Point_Two()
    {
        var animation = TheAnimation(Show());

        Assert.Equal(2, animation.Children.Count);

        var frames = animation.Children.OrderBy(f => f.Cue.CueValue).ToList();
        var values = frames
            .Select(f => Assert.Single(f.Setters.OfType<Setter>()))
            .ToList();

        Assert.Equal(0.0, frames[0].Cue.CueValue);
        Assert.Equal(1.0, frames[1].Cue.CueValue);
        Assert.All(values, s => Assert.Equal(Visual.OpacityProperty, s.Property));
        Assert.Equal(1.0, Assert.IsType<double>(values[0].Value));
        Assert.Equal(0.2, Assert.IsType<double>(values[1].Value));
    }

    // The attachment proof, and the reason this exercise needs no clock: an
    // animation that actually reached the control owns its Opacity at
    // BindingPriority.Animation, and holds it inside the range the keyframes
    // declare. A Style that merely DECLARES the animation but whose selector
    // never matches Pulser passes both structural tests above and fails here.
    //
    // How far the animation has advanced inside that range is time-dependent
    // and is deliberately not asserted; see the .axaml header.
    [AvaloniaFact]
    public void The_Animation_Owns_Pulsers_Opacity()
    {
        var pulser = Show().FindControl<Border>("Pulser")!;

        Assert.Equal(BindingPriority.Animation, pulser.GetDiagnostic(Visual.OpacityProperty).Priority);
        Assert.InRange(pulser.Opacity, 0.2, 1.0);
    }

    // Guards the selector's scope: a Style written as Selector="Border" would
    // animate this one too.
    [AvaloniaFact]
    public void The_Sibling_Borders_Opacity_Is_Not_Animated()
    {
        var still = Show().FindControl<Border>("Still")!;

        Assert.NotEqual(BindingPriority.Animation, still.GetDiagnostic(Visual.OpacityProperty).Priority);
        Assert.Equal(1.0, still.Opacity, precision: 2);
    }
}
