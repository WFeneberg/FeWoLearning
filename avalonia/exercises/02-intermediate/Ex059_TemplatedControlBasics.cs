using Avalonia;
using Avalonia.Controls.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 059 - TemplatedControlBasics (intermediate).
/// Goal:   Author a custom TemplatedControl whose visual is supplied entirely by a
///         ControlTheme - not a UserControl's own XAML content - and prove the
///         template stays live: changing LabelText after the control is shown must
///         update the rendered TextBlock, because it is reached through a
///         TemplateBinding rather than a value baked in once at build time.
/// Drills: TemplatedControl, ControlTheme, FuncControlTemplate, TemplateBinding,
///         StyledElement.Theme.
///
/// There is no .axaml file for this exercise: a ControlTheme can be built entirely
/// in code (Setters is a plain IList<SetterBase>, and FuncControlTemplate's lambda
/// receives the control plus an INameScope to build against), which is exactly the
/// self-contained shape used here - assign the ControlTheme below to this.Theme in
/// the constructor.
///
/// A hard-coded Border+TextBlock with a literal Text reproduces the resting visual
/// with no ControlTheme and no template at all - the test renders it once, then
/// changes LabelText and re-asserts. A literal never follows that change; only a
/// real TemplateBinding does.
/// Passes: dotnet test --filter FullyQualifiedName~Ex059_
public class Ex059_TemplatedControlBasics : TemplatedControl
{
    public static readonly StyledProperty<string> LabelTextProperty =
        AvaloniaProperty.Register<Ex059_TemplatedControlBasics, string>(
            nameof(LabelText), defaultValue: "");

    public string LabelText
    {
        get => GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public Ex059_TemplatedControlBasics()
    {
        throw new NotImplementedException(
            "TODO: Ex059 - build a ControlTheme whose Template is a FuncControlTemplate " +
            "rendering a Border containing a TextBlock, with the TextBlock's Text bound " +
            "to LabelText via a TemplateBinding, and assign it to this.Theme");
    }
}
