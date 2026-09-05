// Exercise 022 - Action Convention Button (beginner).
// Goal:   Learn that ViewModelBinder's naming convention wires up ACTIONS the same pass it
//         wires up bindings: a Button named after a public method on the view model gets that
//         method invoked on click - and that this only really fires once the view is hosted in
//         a real window.
// Drills: ViewModelBinder.Bind attaching a real Microsoft.Xaml.Behaviors.EventTrigger carrying
//         a Caliburn.Micro.ActionMessage, keyed on the button's own x:Name; that
//         PresentationSource is what makes the trigger resolvable, not Measure/Arrange/Loaded.
// Passes: dotnet test --filter FullyQualifiedName~Ex022_
//
// Measured on this machine (Caliburn.Micro 5.0.258): binding a view model exposing a public
// void Plain() method to a view containing a Button x:Name="Plain" attaches a real trigger -
// Interaction.GetTriggers(button) holds one Microsoft.Xaml.Behaviors.EventTrigger whose
// EventName is "Click" and whose single action is a Caliburn.Micro.ActionMessage with
// MethodName "Plain" - and this trigger exists immediately after binding, with no window
// involved yet. A second button whose name matches no method gets no trigger at all: the
// convention does not wire every button, only the ones it can name a method for.
//
// The trigger existing is not the same as it firing. Raising ButtonBase.ClickEvent on the
// "Plain" button BEFORE the view has a real PresentationSource - Measure/Arrange
// (this track's Layout helper) included - invokes nothing: Plain() is never called. Only once
// the view is hosted in a real window (this track's Show helper) does the SAME raised Click
// actually invoke Plain(), and it does so exactly once per raised click - not once ever,
// not zero, not twice.

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex022_ActionConventionButton
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND action in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex022 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A view model exposing one public, no-argument, void-returning method to invoke by convention.</summary>
public class Ex022_Vm : PropertyChangedBase
{
    /// <summary>How many times Plain() actually ran.</summary>
    public int ClickCount { get; private set; }

    public void Plain() => ClickCount++;
}
