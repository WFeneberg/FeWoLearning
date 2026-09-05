// Exercise 022 - Action Convention Button (beginner).
// Goal:   Learn that ViewModelBinder's naming convention wires up ACTIONS the same pass it
//         wires up bindings: a Button named after a public method on the view model gets that
//         method invoked on click - and that this only really fires once the view is hosted in
//         a real window.
// Drills: ViewModelBinder.Bind attaching a real Microsoft.Xaml.Behaviors.EventTrigger carrying
//         a Caliburn.Micro.ActionMessage, keyed on the button's own x:Name; that
//         PresentationSource is what makes the trigger resolvable, not Measure/Arrange/Loaded.
// Passes: dotnet test --filter FullyQualifiedName~Ex022_

using System.Windows;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex022_ActionConventionButton
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element AND action in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) => ViewModelBinder.Bind(viewModel, view, null);
}

/// <summary>A view model exposing one public, no-argument, void-returning method to invoke by convention.</summary>
public class Ex022_Vm : PropertyChangedBase
{
    /// <summary>How many times Plain() actually ran.</summary>
    public int ClickCount { get; private set; }

    public void Plain() => ClickCount++;
}
