// Exercise 006 - Two Way Binding (beginner).
// Goal:   Let the element push values back into the source, not just read from it.
// Drills: BindingMode.TwoWay, FrameworkElement.SetBinding, and the fact that a two-way
//         binding does not loop forever.
// Passes: dotnet test --filter FullyQualifiedName~Ex006_
//
// A TextBox bound to a view model is the everyday version of this. Here the "editor" is
// this class, so the test can write to it without synthesising keyboard input.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Exercises.Beginner;

public partial class Ex006_TwoWayBinding : Control
{
    // TODO: register a dependency property for Draft: name "Draft", type string, owner
    // Ex006_TwoWayBinding, default value "". Call the field DraftProperty.

    /// <summary>The value being edited.</summary>
    public string Draft
    {
        get => throw new NotImplementedException("TODO: Ex006 - read Draft from the dependency property");
        set => throw new NotImplementedException("TODO: Ex006 - write Draft into the dependency property");
    }

    /// <summary>
    /// Binds <see cref="Draft"/> to <c>source.Caption</c> in both directions: the source
    /// seeds this element, and later edits here land back on the source.
    /// </summary>
    public void BindDraftTo(object source) =>
        // TODO: build a Binding with Path=Caption, Source=source, Mode=TwoWay and attach
        // it with SetBinding. Copying the value by hand fails in both directions.
        throw new NotImplementedException("TODO: Ex006 - bind Draft two-way to source.Caption");
}
