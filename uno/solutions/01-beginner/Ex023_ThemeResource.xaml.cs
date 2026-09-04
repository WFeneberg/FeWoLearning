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
        // On the element, not on Application: this is what lets one subtree be dark inside
        // an otherwise light app. ElementTheme.Default would hand the decision back to the
        // parent chain rather than pinning it here.
        RequestedTheme = theme;
}
