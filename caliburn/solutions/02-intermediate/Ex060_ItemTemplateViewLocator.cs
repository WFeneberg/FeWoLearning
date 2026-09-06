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

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex060_ItemTemplateViewLocator
{
    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        ViewModelBinder.Bind(viewModel, view, null);

    /// <summary>Predicts whether ConventionManager would assign DefaultItemTemplate to an
    /// ItemsControl bound to a collection of itemType - the SAME reference-type-vs-value-type
    /// rule the framework applies, string excluded even though it is a reference type.</summary>
    public bool WouldGetDefaultItemTemplate(Type itemType) =>
        !itemType.IsValueType && itemType != typeof(string);
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
