// Exercise 021 - Convention Value Converter (beginner).
// Goal:   Learn that ViewModelBinder's naming convention does not just decide WHETHER and HOW
//         (Mode/UpdateSourceTrigger, ex018) to bind - it also decides whether the binding
//         needs a Converter, and inserts one automatically only where the types actually need
//         bridging.
// Drills: ConventionManager.ApplyValueConverter - the hook ViewModelBinder consults while
//         building each binding; reading a real Converter back off the finished
//         System.Windows.Data.Binding via BindingOperations.GetBinding.
// Passes: dotnet test --filter FullyQualifiedName~Ex021_

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex021_ConventionValueConverter
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) => ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        BindingOperations.GetBinding(element, bindableProperty);
}

/// <summary>A view model exposing one settable property per row of the measured table above.</summary>
public class Ex021_Vm : PropertyChangedBase
{
    bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }

    int _count = 3;
    public int Count { get => _count; set => Set(ref _count, value); }
}
