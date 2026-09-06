// Exercise 058 - Items Convention Binding (intermediate).
// Goal:   Naming an ItemsControl after a view-model collection binds its ItemsSource (Mode is
//         OneWay here because the collection property is get-only - ex018 already established
//         that Mode rule in general, so this exercise does not re-litigate it). The genuinely
//         NEW fact for an ItemsControl specifically: for a plain collection of strings, the
//         convention wires ItemsSource and NOTHING ELSE - DisplayMemberPath and ItemTemplate are
//         both left at their WPF defaults (contrast ex060, where a collection of view models
//         DOES get an ItemTemplate assigned).
// Drills: writing a predicate that inspects an ALREADY-BOUND ItemsControl and answers whether
//         the convention left its presentation properties (DisplayMemberPath, ItemTemplate) at
//         their defaults - a real AND over two independent properties, not satisfied by checking
//         either one alone.
// Passes: dotnet test --filter FullyQualifiedName~Ex058_

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex058_ItemsConventionBinding
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Reads back the REAL Binding (if any) the convention produced for one element/property pair.</summary>
    public Binding? GetAppliedBinding(FrameworkElement element, DependencyProperty bindableProperty) =>
        BindingOperations.GetBinding(element, bindableProperty);

    /// <summary>True iff itemsControl's OWN presentation properties are BOTH still at their WPF
    /// defaults - DisplayMemberPath empty AND ItemTemplate null. Answers "did the convention wire
    /// ItemsSource and leave presentation alone", given an ItemsControl that has already been
    /// bound (or never bound at all).</summary>
    public bool LeavesPresentationAtDefaults(ItemsControl itemsControl) =>
        string.IsNullOrEmpty(itemsControl.DisplayMemberPath) && itemsControl.ItemTemplate == null;
}

/// <summary>Exposes exactly one collection, named to match one of the view's two ItemsControls
/// and nothing on the other.</summary>
public class Ex058_Vm : PropertyChangedBase
{
    public BindableCollection<string> Items { get; } = new(["alpha", "beta", "gamma"]);
}
