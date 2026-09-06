// Exercise 059 - Active Item Selected Item (intermediate).
// Goal:   The selection convention wires a ListBox's SelectedItem to a conductor's ActiveItem -
//         TWO-WAY, with no XAML written for it at all. Nobody writes SelectedItem="{Binding
//         ActiveItem}"; ConventionManager derives the selection property's name FROM the
//         collection's own name (Items -> ActiveItem) and finds it on the conductor. Also learn
//         the companion fact: a ContentControl named "ActiveItem" is bound through Caliburn's OWN
//         attached View.Model property, TWO-WAY - NOT Content, which gets no binding at all. That
//         attached property is how a conductor's currently active item gets rendered through the
//         ViewLocator elsewhere in a real app.
// Drills: activating a Conductor<T>.Collection.OneActive with two children in sequence (the
//         conductor must itself be active first, exactly as ex034 established) so the convention
//         has an ActiveItem to wire selection to, then reading back the REAL Bindings the
//         convention produced on BOTH controls via BindingOperations.GetBinding, plus proving the
//         wiring is genuinely live by driving it from BOTH directions - through the conductor,
//         and through the ListBox itself.
// Passes: dotnet test --filter FullyQualifiedName~Ex059_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a Conductor<T>.Collection.OneActive
// with two ACTIVE children to a view containing <ListBox x:Name="Items" /> and
// <ContentControl x:Name="ActiveItem" />:
//
//   ListBox.ItemsSource      Path=Items,      Mode=OneWay
//   ListBox.SelectedItem     Path=ActiveItem, Mode=TwoWay   <- nobody wrote this
//   ContentControl.Content              no binding at all
//   ContentControl.View.Model  Path=ActiveItem, Mode=TwoWay <- Caliburn's own attached property

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex059_ActiveItemSelectedItem
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex059 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads back the REAL Binding the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        throw new NotImplementedException("TODO: Ex059 - BindingOperations.GetBinding(element, bindableProperty)");
}

/// <summary>An item this conductor shows - nothing about closing or refusing is the lesson here.</summary>
public class Ex059_Child : Screen { }

public class Ex059_Conductor : Conductor<Ex059_Child>.Collection.OneActive
{
    /// <summary>Activates this conductor first if it is not already active (a conductor only
    /// activates children while it is itself active - ex033/ex034), then activates first and
    /// then second in order, leaving second as ActiveItem.</summary>
    public Task ActivateBothAsync(Ex059_Child first, Ex059_Child second) =>
        throw new NotImplementedException("TODO: Ex059 - activate this conductor if needed, then activate first, then second");
}
