using Avalonia.Controls;
using Avalonia.Input;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex080_
public class Ex080_FocusManagement : StackPanel
{
    /// <summary>Given. Do not change.</summary>
    public Button Alpha { get; } = new() { Content = "Alpha" };

    /// <summary>Given. Do not change.</summary>
    public Button Beta { get; } = new() { Content = "Beta" };

    /// <summary>Given. Do not change.</summary>
    public Button Gamma { get; } = new() { Content = "Gamma" };

    /// <summary>Given. Do not change.</summary>
    public Button Delta { get; } = new() { Content = "Delta" };

    private void Configure()
    {
        Beta.TabIndex = 1;
        Alpha.TabIndex = 2;
        Delta.TabIndex = 3;

        // Out of the tab order, still focusable: IsTabStop gates traversal,
        // Focusable would gate focus itself.
        Gamma.IsTabStop = false;
    }

    public bool MoveNext() =>
        TopLevel.GetTopLevel(this)?.FocusManager?.TryMoveFocus(NavigationDirection.Next) ?? false;

    public Ex080_FocusManagement()
    {
        Children.Add(Alpha);
        Children.Add(Beta);
        Children.Add(Gamma);
        Children.Add(Delta);
        Configure();
    }
}
