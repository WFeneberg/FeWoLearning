// Exercise 054 - Element Name Binding (intermediate).
// Goal:   Wire two elements together without a view model in between.
// Drills: {Binding ElementName=...}, forward references inside one name scope, and
//         FindName as the same lookup the binding does.
// Passes: dotnet test --filter FullyQualifiedName~Ex054_
//
// Useful for genuinely view-local relationships - a label that mirrors a slider, a panel
// sized by a splitter. It is also the first thing people reach for when the value belongs
// in a view model, and then the view knows things it should not.

using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public sealed partial class Ex054_ElementNameBinding : UserControl
{
    public Ex054_ElementNameBinding() => InitializeComponent();
}
