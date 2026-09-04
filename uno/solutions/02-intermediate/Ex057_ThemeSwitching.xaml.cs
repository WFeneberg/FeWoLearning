// Exercise 057 - Theme Switching (intermediate).
// Goal:   Change theme at runtime and know exactly what moves.
// Drills: RequestedTheme reaching a whole subtree, {ThemeResource} re-resolving,
//         {StaticResource} staying where it was, and why a restart used to be the advice.
// Passes: dotnet test --filter FullyQualifiedName~Ex057_
//
// Application.RequestedTheme cannot be changed once the app is running, so runtime theming
// is done per element tree. Anything resolved with StaticResource is baked in at build
// time and simply will not follow - which is the real reason "theme switching needs a
// restart" was folklore for years.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public sealed partial class Ex057_ThemeSwitching : UserControl
{
    public Ex057_ThemeSwitching() => InitializeComponent();

    /// <summary>
    /// Switches this control's subtree to <paramref name="theme"/>.
    /// </summary>
    public void ApplyTheme(ElementTheme theme) => RequestedTheme = theme;

    /// <summary>
    /// Switches only the subtree under the "Wrapper" border, leaving the rest of this
    /// control on whatever theme it had.
    /// </summary>
    public void ApplyThemeToWrapperOnly(ElementTheme theme)
    {
        // Any FrameworkElement can start a theme scope, so a preview pane can be dark
        // inside a light app without anybody touching Application.RequestedTheme - which
        // cannot be changed once the app is running anyway.
        if (FindName("Wrapper") is FrameworkElement wrapper)
        {
            wrapper.RequestedTheme = theme;
        }
    }
}
