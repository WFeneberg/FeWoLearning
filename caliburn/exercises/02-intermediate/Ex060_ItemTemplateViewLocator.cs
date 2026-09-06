// Exercise 060 - Item Template View Locator (intermediate).
// Goal:   A list of plain strings gets NO ItemTemplate (ex058) - but a list of REFERENCE-TYPE
//         items (not just Caliburn view models: see the measurement below) gets
//         ConventionManager.DefaultItemTemplate assigned automatically. That template's own
//         content, per Caliburn.Micro's source, is a ContentControl whose Caliburn View.Model
//         attached property is bound directly to the item itself (no Path at all) - which is
//         what makes every row in the list render through the ViewLocator, one item at a time,
//         without you ever writing an ItemTemplate by hand.
// Drills: writing the SAME reference-type-vs-value-type/string predicate ConventionManager
//         itself uses to decide whether to assign the template - and the one sharp trap in it: a
//         naive "not a value type" check alone is WRONG, because System.String is a reference
//         type too, yet is explicitly excluded - then checking that predicate against several
//         REAL, measured bindings so it cannot silently drift from what the framework actually
//         does.
// Passes: dotnet test --filter FullyQualifiedName~Ex060_
//
// Measured on this machine (Caliburn.Micro 5.0.258), binding an ItemsControl named "Items" to
// collections of different item types:
//
//   BindableCollection<string>            ItemTemplate: null              (ex058's own finding)
//   BindableCollection<int>                ItemTemplate: null              (a value type)
//   BindableCollection<Ex060_RowItem>       ItemTemplate: ConventionManager.DefaultItemTemplate
//   BindableCollection<object> (plain)      ItemTemplate: ConventionManager.DefaultItemTemplate (SAME template)
//
// SCOPE THIS CAREFULLY: the assignment is decided by the item's CLR type shape (any reference
// type other than string), NOT by whether it is specifically a Screen/PropertyChangedBase, and
// NOT by whether the ViewLocator can actually find a matching view for it - that resolution
// happens later, per item, when the template is actually used to render. Two more preconditions,
// separately measured on this machine: if the ItemsControl already has a non-empty
// DisplayMemberPath, or an ItemTemplate already set (in either case, before ViewModelBinder.Bind
// runs), the convention leaves it alone entirely - no override, whatever was already there stays.
// One suspected precondition does NOT hold, also measured directly: the view-model PROPERTY's own
// declared type does not need to be generic - a property declared as plain, non-generic
// IEnumerable (backed by the identical reference-type items at runtime) still gets the template
// assigned exactly like a BindableCollection<T> would, so this is not a check on the property's
// static type, only on the actual items' runtime type.
//
// WHY THIS EXERCISE DOES NOT LOAD THE TEMPLATE'S CONTENT ITSELF (a real trap, also measured on
// this machine): ConventionManager.DefaultItemTemplate is ONE process-wide static DataTemplate -
// a DependencyObject, which WPF permanently pins to whichever thread first realizes it (its
// Dispatcher is fixed for the rest of the process). Every [WpfFact] in this suite runs on its
// OWN STA thread; the very first exercise anywhere in the run to bind a view-model collection
// pins this shared template to ITS thread. Calling template.LoadContent() (or even reading
// template.VisualTree or template.Triggers) from any LATER exercise's own, different thread
// then throws InvalidOperationException ("the calling thread cannot access this object") - this
// reproduced reliably once other exercises' tests ran first, even though an isolated run of just
// this file's tests hid it. That is an artifact of this multi-thread-per-test harness, not of a
// real single-UI-thread Caliburn app - which is why this exercise proves its facts by reference
// identity and by a type predicate instead of by loading the shared template's content.

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex060_ItemTemplateViewLocator
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex060 - ViewModelBinder.Bind(viewModel, view, null)");

    /// <summary>Predicts whether ConventionManager would assign DefaultItemTemplate to an
    /// ItemsControl bound to a collection of itemType - the SAME reference-type-vs-value-type
    /// rule the framework applies, string excluded even though it is a reference type.</summary>
    public bool WouldGetDefaultItemTemplate(Type itemType) =>
        throw new NotImplementedException("TODO: Ex060 - true for a reference type OTHER than string; false for a value type or string");
}

/// <summary>A view model whose Items holds plain strings - the contrast case, no ItemTemplate expected.</summary>
public class Ex060_StringsVm : PropertyChangedBase
{
    public BindableCollection<string> Items { get; } = new(["alpha", "beta"]);
}

/// <summary>A view model whose Items holds a value type - also expected to get no ItemTemplate,
/// for a DIFFERENT reason than string (it fails the value-type check, not the string exclusion).</summary>
public class Ex060_IntsVm : PropertyChangedBase
{
    public BindableCollection<int> Items { get; } = new([1, 2, 3]);
}

/// <summary>One row a list of these is meant to show - a real Caliburn screen, but this exercise
/// deliberately never registers a view for it: the template gets assigned regardless.</summary>
public class Ex060_RowItem : Screen
{
    public string Label { get; set; } = "";
}

/// <summary>A view model whose Items holds view-model rows, not strings.</summary>
public class Ex060_RowsVm : PropertyChangedBase
{
    public BindableCollection<Ex060_RowItem> Items { get; } = new([new Ex060_RowItem(), new Ex060_RowItem()]);
}

/// <summary>A view model whose Items holds a plain reference type that has NOTHING to do with
/// Caliburn at all (no PropertyChangedBase, no Screen) - proves the template assignment is about
/// the item's CLR type shape, not about it being a "view model" in any Caliburn-specific sense.</summary>
public class Ex060_PlainObjectsVm : PropertyChangedBase
{
    public BindableCollection<object> Items { get; } = new([new object(), new object()]);
}
