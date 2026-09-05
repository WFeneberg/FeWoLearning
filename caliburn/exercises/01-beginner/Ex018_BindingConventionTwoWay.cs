// Exercise 018 - Binding Convention Two Way (beginner).
// Goal:   Learn that the convention engine doesn't just decide WHETHER to bind (ex017) - it
//         also picks the binding's Mode and UpdateSourceTrigger, and those choices come from
//         BOTH sides: whether the view-model property is settable, and whether the element's
//         bindable property is two-way capable.
// Drills: reading a real System.Windows.Data.Binding's Mode and UpdateSourceTrigger back via
//         BindingOperations.GetBinding; that Caliburn's chosen trigger is PropertyChanged,
//         which is NOT WPF's own default for TextBox.Text (that default is LostFocus).
// Passes: dotnet test --filter FullyQualifiedName~Ex018_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a view model with a settable
// UserName (string), a get-only Description (string), a settable IsHappy (bool) and an Items
// (BindableCollection<string>) to a view whose elements carry those exact names:
//
//   element       x:Name      Mode     UpdateSourceTrigger
//   TextBox       UserName    TwoWay   PropertyChanged
//   TextBlock     Description OneWay   PropertyChanged
//   CheckBox      IsHappy     TwoWay   PropertyChanged
//   ItemsControl  Items       OneWay   PropertyChanged   (on ItemsSource)
//
// TwoWay only happens where BOTH sides agree: the property is settable AND the element's
// bindable property is itself two-way capable. UpdateSourceTrigger is PropertyChanged in
// every case - including TextBox, where WPF's OWN default (TextBox.TextProperty's
// FrameworkPropertyMetadata.DefaultUpdateSourceTrigger) is LostFocus. Caliburn overrides it.

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex018_BindingConventionTwoWay
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex018 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        throw new NotImplementedException("TODO: Ex018 - BindingOperations.GetBinding(element, bindableProperty)");
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
