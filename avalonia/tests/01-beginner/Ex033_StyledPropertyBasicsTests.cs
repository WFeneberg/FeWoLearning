using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex033_StyledPropertyBasicsTests
{
    private static Ex033_StyledPropertyBasics Show() =>
        ViewHarness.Show(new Ex033_StyledPropertyBasics(), 200, 80);

    private static IEnumerable<Style> AllStyles(Visual root) =>
        root.GetSelfAndVisualDescendants()
            .OfType<StyledElement>()
            .SelectMany(e => e.Styles)
            .OfType<Style>();

    // Structural: a rule actually targeting the CaptionProperty with the
    // right value has to exist somewhere in the tree - a code-behind hack
    // that pokes CaptionText.Text directly, with no Style at all, leaves
    // this walk empty.
    [AvaloniaFact]
    public void A_Style_Rule_Targets_Caption_With_The_Styled_Value()
    {
        var view = Show();

        var hasRule = AllStyles(view).Any(style =>
            style.Setters.OfType<Setter>().Any(setter =>
                setter.Property == Ex033_StyledPropertyBasics.CaptionProperty &&
                setter.Value is "from-style"));

        Assert.True(hasRule, "expected a Style setting Caption to \"from-style\"");
    }

    [AvaloniaFact]
    public void CaptionText_Starts_At_The_Registered_Default()
    {
        var view = Show();
        var captionText = view.FindControl<TextBlock>("CaptionText")!;

        Assert.Equal("n/a", captionText.Text);
    }

    // The real discriminator: a hard-coded Text="n/a" matches the resting
    // state above but can never follow the Style being applied, because it
    // was never bound to Caption in the first place. Toggling the "styled"
    // class both ways proves the binding tracks Caption live, and the
    // BindingPriority check proves the value actually came from a Style
    // rather than a coincidentally-equal local value.
    [AvaloniaFact]
    public void Applying_The_Styled_Class_Routes_Caption_Through_The_Style_And_Back()
    {
        var view = Show();
        var captionText = view.FindControl<TextBlock>("CaptionText")!;

        view.Classes.Add("styled");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("from-style", captionText.Text);
        Assert.NotEqual(
            BindingPriority.LocalValue,
            view.GetDiagnostic(Ex033_StyledPropertyBasics.CaptionProperty).Priority);

        view.Classes.Remove("styled");
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("n/a", captionText.Text);
    }
}
