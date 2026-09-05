// Exercise 018 - Binding Convention Two Way (beginner).
// Goal:   Learn that the convention engine doesn't just decide WHETHER to bind (ex017) - it
//         also picks the binding's Mode and UpdateSourceTrigger, and that these two choices
//         are NOT made the same way: Mode depends solely on whether the view-model property
//         has a public setter; the element has no say in it at all.
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
// Mode is TwoWay when the view-model property has a public setter and OneWay otherwise - the
// element's bindable property has no say in it. ConventionManager.ApplyBindingMode is an
// Action<Binding, PropertyInfo>: it is handed only the binding and the property, never the
// element or its DependencyProperty, so it cannot possibly consult whether the element's own
// bindable property is two-way capable. Both Description and Items above are get-only, which
// is the whole reason those two rows read OneWay - make Items settable instead and its
// ItemsSource binding becomes TwoWay too, even though ItemsControl.ItemsSource is no more
// "two-way capable" than TextBlock.Text is. UpdateSourceTrigger is a genuinely different
// story: it IS element-aware (ConventionManager.ApplyUpdateSourceTrigger is an
// Action<DependencyProperty, DependencyObject, Binding, PropertyInfo>), which is why it is
// PropertyChanged in every case above - including TextBox, where WPF's OWN default
// (TextBox.TextProperty's FrameworkPropertyMetadata.DefaultUpdateSourceTrigger) is
// LostFocus. Caliburn overrides it.

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
