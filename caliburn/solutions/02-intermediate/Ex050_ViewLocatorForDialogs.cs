// Exercise 050 - View Locator For Dialogs (intermediate).
// Goal:   WindowManager decides how to HOST a located view by its TYPE: a view that already
//         derives from Window is used AS-IS as the dialog's own window; anything else
//         (a UserControl, in every exercise so far) gets WRAPPED in a bare System.Windows.Window
//         Caliburn creates for it. ViewLocator.LocateForModelType(Type, DependencyObject, object)
//         is the lookup WindowManager itself uses to find that view, given only the root model's
//         TYPE - a different overload from ex013's instance-based LocateForModel.
// Drills: calling ViewLocator.LocateForModelType and checking whether what it found IS a Window,
//         rather than merely resembling one (a stub that instead checks `is FrameworkElement`
//         would be true for BOTH shapes and never distinguish them at all).
// Passes: dotnet test --filter FullyQualifiedName~Ex050_

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public static class Ex050_ViewLocatorForDialogs
{
    public static bool ResolvesToAWindow(Type modelType) =>
        ViewLocator.LocateForModelType(modelType, null, null) is Window;
}

// Fixtures below, not part of the TODO - three shapes of "located view", named so Caliburn's
// ordinary ViewModel-suffix convention (ex013) resolves each without touching any global
// ViewLocator delegate: a Window-derived view (used as-is), a plain UserControl (gets wrapped),
// and a third FrameworkElement shape that is neither (so a stub checking `is UserControl`
// instead of `is Window` still gets caught).

public class Ex050_WindowShapedViewModel : Screen { }

public class Ex050_WindowShapedView : Window
{
    public Ex050_WindowShapedView() => Content = new TextBlock { Text = "window-shaped" };
}

public class Ex050_PlainViewModel : Screen { }

public class Ex050_PlainView : UserControl
{
    public Ex050_PlainView() => Content = new TextBlock { Text = "plain" };
}

public class Ex050_GridShapedViewModel : Screen { }

public class Ex050_GridShapedView : Grid
{
    public Ex050_GridShapedView() => Children.Add(new TextBlock { Text = "grid-shaped" });
}
