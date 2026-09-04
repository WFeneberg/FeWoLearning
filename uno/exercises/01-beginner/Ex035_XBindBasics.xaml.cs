// Exercise 035 - x:Bind Basics (beginner).
// Goal:   Use compiled bindings, and notice what their default mode costs you.
// Drills: x:Bind against the code-behind class, Mode=OneWay to opt into updates,
//         x:Bind to a method, and INotifyPropertyChanged as the thing OneWay listens to.
// Passes: dotnet test --filter FullyQualifiedName~Ex035_
//
// The trap is the default: {Binding} is OneWay, {x:Bind} is OneTime. A path that used to
// update stops updating the moment somebody "modernises" it to x:Bind, and nothing warns.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Beginner;

public sealed partial class Ex035_XBindBasics : UserControl, INotifyPropertyChanged
{
    private string _caption = "hello";

    public Ex035_XBindBasics() => InitializeComponent();

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// The bound text. Setting it must announce itself, or the OneWay binding goes stale.
    /// </summary>
    public string Caption
    {
        get => _caption;
        set =>
            // TODO: store it and raise PropertyChanged for this property. A OneWay x:Bind
            // listens to exactly this event, same as a classic binding.
            throw new NotImplementedException("TODO: Ex035 - store the caption and announce it");
    }

    /// <summary>The caption in upper case. Bound as a method call, not as a property.</summary>
    public string Shout() =>
        throw new NotImplementedException("TODO: Ex035 - return the caption upper-cased");

    private void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
