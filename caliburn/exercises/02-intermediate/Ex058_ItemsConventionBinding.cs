// Exercise 058 - Items Convention Binding (intermediate).
// Goal:   Naming an ItemsControl after a view-model collection binds its ItemsSource - and an
//         ItemsControl whose name matches NOTHING on the view model binds NOTHING AT ALL, not
//         even the Visibility fallback that a mismatched name gets on other element types
//         (ex019/ex020): ItemsControl's own convention IS ItemsSource, so when that lookup fails
//         there is no second property left to fall back to.
// Drills: reading back BOTH the presence and the absence of a real Binding via
//         BindingOperations.GetBinding - proving a binding exists on the matched element and
//         proving NEITHER ItemsSource NOR Visibility exists on the unmatched one, plus reading
//         DisplayMemberPath and ItemTemplate directly off the ItemsControl (properties, not
//         bindings) to see what the convention leaves untouched for a plain string collection.
// Passes: dotnet test --filter FullyQualifiedName~Ex058_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding a view model exposing a settable...
// no: a GET-ONLY BindableCollection<string> Items to a view with two ItemsControls, one named
// "Items" and one named "NoSuchProperty":
//
//   ItemsControl "Items"          ItemsSource: Path=Items, Mode=OneWay (get-only -> OneWay, per
//                                 the rule ex018 established: mode comes solely from whether the
//                                 view-model property has a setter)
//                                 DisplayMemberPath: "" (untouched)
//                                 ItemTemplate: null (untouched - see ex060 for when this is NOT null)
//   ItemsControl "NoSuchProperty" ItemsSource: no binding at all
//                                 Visibility: no binding at all either

using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex058_ItemsConventionBinding
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex058 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Reads back the REAL Binding (if any) the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        throw new NotImplementedException("TODO: Ex058 - BindingOperations.GetBinding(element, bindableProperty)");
}

/// <summary>Exposes exactly one collection, named to match one of the view's two ItemsControls
/// and nothing on the other.</summary>
public class Ex058_Vm : PropertyChangedBase
{
    public BindableCollection<string> Items { get; } = new(["alpha", "beta", "gamma"]);
}
