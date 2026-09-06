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
//
// Measured on this machine (Caliburn.Micro 5.0.258), via an instrumented EnsureWindow override:
//   EnsureWindow exit:  SizeToContent=WidthAndHeight  StartupLoc=CenterScreen  Width=NaN  Left=NaN
//   after Show():       Width=14  Left=953  Title='Hello'
// EnsureWindow runs FIRST and never assigns Width/Left (still NaN when it returns) - the
// dictionary is applied to CreateWindowAsync's result AFTER that, so Title/ShowInTaskbar/Width/
// Left are all genuinely set on the window at that point. Width and Left are simply gone again by
// the time Show() has finished laying the window out - a different number each run (measured 14/
// 953 one run, 411/755 another, 229/846 a third - dependent on the window's actual content and the
// screen), never the numbers the dictionary requested, but never because EnsureWindow "overrode"
// them either.

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public static class Ex049_WindowManagerSettings
{
    /// <summary>The TODO: return a dictionary with exactly these four keys - "Title" -> title,
    /// "ShowInTaskbar" -> showInTaskbar, "Width" -> width, "Left" -> left.</summary>
    public static IDictionary<string, object> BuildSettings(string title, bool showInTaskbar, double width, double left) =>
        throw new NotImplementedException("TODO: Ex049 - build the settings dictionary (Title/ShowInTaskbar/Width/Left keys)");
}
