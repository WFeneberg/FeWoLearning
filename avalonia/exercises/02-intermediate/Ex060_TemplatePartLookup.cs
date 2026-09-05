using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 060 - TemplatePartLookup (intermediate).
/// Goal:   Override OnApplyTemplate to find two named parts inside the ControlTheme
///         below - PART_Display and PART_Increment - and wire the button so
///         clicking it increments Count and refreshes the display text.
/// Drills: OnApplyTemplate, TemplateAppliedEventArgs, INameScope.Find.
///
/// The ControlTheme (a Border > StackPanel > TextBlock PART_Display + Button
/// PART_Increment) is given below and already assigned to this.Theme in the
/// constructor - unlike ex059, building the theme is not what this exercise
/// grades. IncrementPart and DisplayPart are given properties too: the TODO is to
/// populate them from e.NameScope inside OnApplyTemplate and hook up the click.
///
/// Note this stub's constructor does NOT throw - OnApplyTemplate only runs during
/// a layout pass (on Show/attach), not at construction, so throwing there is what
/// keeps this exercise out of the gallery smoke test (see GalleryCatalog).
///
/// A handler added in the constructor that listens for the Button's Click event
/// bubbling up to this control (this.AddHandler(Button.ClickEvent, ...)) can
/// increment Count and even find a TextBlock by walking the visual tree, entirely
/// without ever looking anything up by name - the test does not take the
/// resulting Count or display text alone as proof. It also asserts IncrementPart
/// and DisplayPart are non-null and are the SAME objects as the real button and
/// text block in the tree - properties that only OnApplyTemplate itself can ever
/// populate, since e.NameScope does not exist anywhere else.
/// Passes: dotnet test --filter FullyQualifiedName~Ex060_
public class Ex060_TemplatePartLookup : TemplatedControl
{
    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<Ex060_TemplatePartLookup, int>(nameof(Count), defaultValue: 0);

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    /// <summary>Given. Populate from OnApplyTemplate below. Do not populate elsewhere.</summary>
    public Button? IncrementPart { get; protected set; }

    /// <summary>Given. Populate from OnApplyTemplate below. Do not populate elsewhere.</summary>
    public TextBlock? DisplayPart { get; protected set; }

    /// <summary>Given. Do not change.</summary>
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
        throw new NotImplementedException(
            "TODO: Ex060 - use e.NameScope.Find<Button>(\"PART_Increment\") and " +
            "e.NameScope.Find<TextBlock>(\"PART_Display\") to populate IncrementPart and " +
            "DisplayPart, then wire IncrementPart.Click to increment Count and set " +
            "DisplayPart.Text to Count.ToString()");
    }
}
