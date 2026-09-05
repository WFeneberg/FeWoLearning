using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex060_
public class Ex060_TemplatePartLookup : TemplatedControl
{
    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<Ex060_TemplatePartLookup, int>(nameof(Count), defaultValue: 0);

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public Button? IncrementPart { get; protected set; }

    public TextBlock? DisplayPart { get; protected set; }

    private static readonly ControlTheme CounterTheme = new(typeof(Ex060_TemplatePartLookup))
    {
        Setters =
        {
            new Setter(TemplateProperty, new FuncControlTemplate<Ex060_TemplatePartLookup>((_, scope) =>
            {
                var display = new TextBlock { Name = "PART_Display", Text = "0" };
                var button = new Button { Name = "PART_Increment", Content = "+" };
                scope.Register("PART_Display", display);
                scope.Register("PART_Increment", button);
                return new StackPanel { Children = { display, button } };
            })),
        },
    };

    public Ex060_TemplatePartLookup()
    {
        Theme = CounterTheme;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        IncrementPart = e.NameScope.Find<Button>("PART_Increment");
        DisplayPart = e.NameScope.Find<TextBlock>("PART_Display");

        if (IncrementPart is not null)
        {
            IncrementPart.Click += (_, _) =>
            {
                Count++;
                if (DisplayPart is not null)
                {
                    DisplayPart.Text = Count.ToString();
                }
            };
        }
    }
}
