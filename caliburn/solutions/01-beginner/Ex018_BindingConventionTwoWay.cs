// Exercise 018 - Binding Convention Two Way (beginner).
// Goal:   Learn that the convention engine doesn't just decide WHETHER to bind (ex017) - it
//         also picks the binding's Mode and UpdateSourceTrigger, and that these two choices
//         are NOT made the same way: Mode depends solely on whether the view-model property
//         has a public setter; the element has no say in it at all.
// Drills: reading a real System.Windows.Data.Binding's Mode and UpdateSourceTrigger back via
//         BindingOperations.GetBinding; that Caliburn's chosen trigger is PropertyChanged,
//         which is NOT WPF's own default for TextBox.Text (that default is LostFocus).
// Passes: dotnet test --filter FullyQualifiedName~Ex018_

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex018_BindingConventionTwoWay
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) => ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        BindingOperations.GetBinding(element, bindableProperty);
}

/// <summary>A view model exposing one property per row of the measured table above.</summary>
public class Ex018_Vm : PropertyChangedBase
{
    string _userName = "Ada";
    public string UserName { get => _userName; set => Set(ref _userName, value); }

    public string Description => "read-only";

    bool _isHappy;
    public bool IsHappy { get => _isHappy; set => Set(ref _isHappy, value); }

    public BindableCollection<string> Items { get; } = new();
}
