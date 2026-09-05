using System;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex063_StyleSetterAndTransitionTests
{
    private static Ex063_StyleSetterAndTransition Show()
    {
        var view = ViewHarness.Show(new Ex063_StyleSetterAndTransition(), 200, 160);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    private static Border Fader(Ex063_StyleSetterAndTransition view) =>
        view.FindControl<Border>("Fader")!;

    private static Border Instant(Ex063_StyleSetterAndTransition view) =>
        view.FindControl<Border>("Instant")!;

    // The plain Setter half of the row. The rendered colour alone would be
    // satisfied by Background="#FF3366" written on the Border, so the priority
    // is asserted alongside it - only a Style produces BindingPriority.Style.
    [AvaloniaFact]
    public void Background_Arrives_From_A_Style_Setter()
    {
        var fader = Fader(Show());

        Assert.Equal(Color.Parse("#FF3366"), (fader.Background as ISolidColorBrush)?.Color);
        Assert.Equal(BindingPriority.Style, fader.GetDiagnostic(Border.BackgroundProperty).Priority);
    }

    // The transition half, structurally. Transitions is itself a styled
    // property, so the same priority argument applies: a <Border.Transitions>
    // element nested in the markup lands at LocalValue and is rejected here,
    // even though it would defer the value below identically.
    [AvaloniaFact]
    public void A_DoubleTransition_On_Opacity_Arrives_From_The_Same_Style()
    {
        var fader = Fader(Show());

        Assert.Equal(BindingPriority.Style, fader.GetDiagnostic(Animatable.TransitionsProperty).Priority);
        var transition = Assert.Single(fader.Transitions!.OfType<DoubleTransition>());
        Assert.Equal(nameof(Visual.Opacity), transition.Property!.Name);
        Assert.Equal(TimeSpan.FromSeconds(5), transition.Duration);
    }

    // The behavioural consequence, and the only part of a transition this
    // harness can observe honestly: assigning a new value does not take effect
    // at once, because the transition owns the property for its duration. The
    // untransitioned sibling is the control - it reads the new value
    // immediately - which is what makes this a discriminator rather than a
    // restatement of the structural check above.
    //
    // Progress past this first instant is NOT asserted; see the .axaml header
    // for why it cannot be.
    [AvaloniaFact]
    public void The_Transition_Defers_A_New_Opacity_While_The_Sibling_Applies_It_At_Once()
    {
        var view = Show();
        var fader = Fader(view);
        var instant = Instant(view);

        fader.Opacity = 0.0;
        instant.Opacity = 0.0;
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(1.0, fader.Opacity, precision: 2);
        Assert.Equal(0.0, instant.Opacity, precision: 2);
    }

    // Guards the selector's scope. A Style written as Selector="Border" sweeps
    // up both Borders and satisfies every assertion above, but it would also
    // give Instant a transition it has no business having - and would break the
    // deferral comparison for the wrong reason.
    [AvaloniaFact]
    public void The_Sibling_Border_Is_Left_Untouched_By_The_Style()
    {
        var instant = Instant(Show());

        Assert.Null(instant.Transitions);
        Assert.Equal(Color.Parse("#CCCCCC"), (instant.Background as ISolidColorBrush)?.Color);
    }
}
