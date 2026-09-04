// Exercise 023 - Theme Resource (beginner).
// Goal:   Let one element tree answer differently in light and dark.
// Drills: ResourceDictionary.ThemeDictionaries with the fixed "Light"/"Dark" keys,
//         {ThemeResource} re-evaluating on a theme change, and FrameworkElement
//         .RequestedTheme scoping that change to a subtree.
// Passes: dotnet test --filter FullyQualifiedName~Ex023_

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed partial class Ex023_ThemeResource : UserControl
{
    public Ex023_ThemeResource() => InitializeComponent();

    /// <summary>
    /// Switches this control's subtree to <paramref name="theme"/>. Every
    /// {ThemeResource} beneath it re-resolves; the rest of the app is untouched.
    /// </summary>
    public void ApplyTheme(ElementTheme theme) =>
        // TODO: one assignment. The property is on FrameworkElement, and setting it here
        // is what keeps the change local to this control.
        throw new NotImplementedException("TODO: Ex023 - apply the theme to this subtree");
}
