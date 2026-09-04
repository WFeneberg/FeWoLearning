// Exercise 022 - Static Resource (beginner).
// Goal:   Share values through resource dictionaries, and see where a lookup stops.
// Drills: ResourceDictionary on a FrameworkElement, {StaticResource}, the upward lookup
//         walk, and shadowing a key in an inner scope.
// Passes: dotnet test --filter FullyQualifiedName~Ex022_
//
// {StaticResource} is resolved once, while the tree is built. Nothing re-runs it later,
// which is exactly the difference to {ThemeResource} in the next exercise.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed partial class Ex022_StaticResource : UserControl
{
    public Ex022_StaticResource() => InitializeComponent();
}
