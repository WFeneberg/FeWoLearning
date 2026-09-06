// Exercise 049 - Window Manager Settings (intermediate).
// Goal:   The settings dictionary WindowManager.ShowDialogAsync's third parameter accepts applies
//         arbitrary Window properties BY NAME, via reflection - but not every property survives:
//         Title and ShowInTaskbar stick, while Width and Left do NOT. The mechanism is NOT that
//         Caliburn overrides the dictionary's Width/Left afterwards - EnsureWindow sets
//         SizeToContent=WidthAndHeight and a centred WindowStartupLocation on the window BEFORE
//         the dictionary is ever applied, and never touches Width/Left itself. The dictionary's
//         Width/Left really do land on the window straight after CreateWindowAsync returns; it is
//         WPF's OWN layout, sizing and positioning the window at Show() time to honour the
//         SizeToContent/WindowStartupLocation EnsureWindow set earlier, that then discards them.
// Drills: building an IDictionary<string, object> keyed EXACTLY by the Window property names
//         WindowManager looks up (a typo like "WindowTitle" instead of "Title" silently applies
//         nothing at all - there is no error, the property just never changes).
// Passes: dotnet test --filter FullyQualifiedName~Ex049_

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public static class Ex049_WindowManagerSettings
{
    public static IDictionary<string, object> BuildSettings(string title, bool showInTaskbar, double width, double left) =>
        new Dictionary<string, object>
        {
            ["Title"] = title,
            ["ShowInTaskbar"] = showInTaskbar,
            ["Width"] = width,
            ["Left"] = left,
        };
}
