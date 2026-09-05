// Exercise 021 - Convention Value Converter (beginner).
// Goal:   Learn that ViewModelBinder's naming convention does not just decide WHETHER and HOW
//         (Mode/UpdateSourceTrigger, ex018) to bind - it also decides whether the binding
//         needs a Converter, and inserts one automatically only where the types actually need
//         bridging.
// Drills: ConventionManager.ApplyValueConverter - the hook ViewModelBinder consults while
//         building each binding; reading a real Converter back off the finished
//         System.Windows.Data.Binding via BindingOperations.GetBinding.
// Passes: dotnet test --filter FullyQualifiedName~Ex021_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a view model with a settable bool
// IsVisible and a settable int Count to a view whose elements carry those exact names:
//
//   element     x:Name      bindable property   Converter
//   Border      IsVisible   Visibility           System.Windows.Controls.BooleanToVisibilityConverter
//   TextBlock   Count       Text                 (none)
//
// Border falls back to the FrameworkElement/Visibility convention (ex019) because nothing
// registers one for Border specifically - the exact fallback ex020 exercised. A bool bound
// onto a Visibility property needs bridging (bool is not a Visibility), so
// ConventionManager.ApplyValueConverter hands it WPF's own BooleanToVisibilityConverter -
// nobody in this exercise writes a converter. An int bound onto TextBlock.Text needs no such
// bridging (WPF's own binding engine turns an int into displayable text on its own), so the
// convention leaves Converter AND StringFormat both null - it does not reach for a converter
// just because the two types differ syntactically, only where it has decided bridging is
// actually required.

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex021_ConventionValueConverter
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex021 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        throw new NotImplementedException("TODO: Ex021 - BindingOperations.GetBinding(element, bindableProperty)");
}

/// <summary>A view model exposing one settable property per row of the measured table above.</summary>
public class Ex021_Vm : PropertyChangedBase
{
    bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }

    int _count = 3;
    public int Count { get => _count; set => Set(ref _count, value); }
}
