using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex059_
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

    private static readonly ControlTheme BadgeTheme = new(typeof(Ex059_TemplatedControlBasics))
    {
        Setters =
        {
            new Setter(TemplateProperty, new FuncControlTemplate<Ex059_TemplatedControlBasics>((_, scope) =>
            {
                var text = new TextBlock { Name = "PART_LabelText" };
                text.Bind(TextBlock.TextProperty, new TemplateBinding(LabelTextProperty));

                var border = new Border { Name = "PART_Border", Child = text };

                scope.Register("PART_Border", border);
                scope.Register("PART_LabelText", text);

                return border;
            })),
        },
    };

    public Ex059_TemplatedControlBasics()
    {
        Theme = BadgeTheme;
    }
}
