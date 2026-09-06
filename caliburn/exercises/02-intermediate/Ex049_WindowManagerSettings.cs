// Exercise 049 - Window Manager Settings (intermediate).
// Goal:   The settings dictionary WindowManager.ShowDialogAsync's third parameter accepts applies
//         arbitrary Window properties BY NAME, via reflection - but not every property survives:
//         Title and ShowInTaskbar stick, while Width and Left do NOT - Caliburn's EnsureWindow
//         applies SizeToContent and a centred WindowStartupLocation to the hosting window
//         AFTERWARDS, overriding whatever the dictionary asked for on those two.
// Drills: building an IDictionary<string, object> keyed EXACTLY by the Window property names
//         WindowManager looks up (a typo like "WindowTitle" instead of "Title" silently applies
//         nothing at all - there is no error, the property just never changes).
// Passes: dotnet test --filter FullyQualifiedName~Ex049_
//
// Measured on this machine (Caliburn.Micro 5.0.258): a settings dictionary with Title,
// ShowInTaskbar, Width and Left all present applied the first two exactly as asked, but Width
// and Left came back as whatever SizeToContent=WidthAndHeight and WindowStartupLocation=
// CenterScreen produced instead - genuinely different numbers each run (dependent on the
// window's actual content and the screen), never the numbers the dictionary requested.

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public static class Ex049_WindowManagerSettings
{
    /// <summary>The TODO: return a dictionary with exactly these four keys - "Title" -> title,
    /// "ShowInTaskbar" -> showInTaskbar, "Width" -> width, "Left" -> left.</summary>
    public static IDictionary<string, object> BuildSettings(string title, bool showInTaskbar, double width, double left) =>
        throw new NotImplementedException("TODO: Ex049 - build the settings dictionary (Title/ShowInTaskbar/Width/Left keys)");
}
