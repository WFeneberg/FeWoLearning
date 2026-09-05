// Exercise 034 - DataTemplateSelector (beginner).
// Goal:   Pick a DataTemplate per item based on the item's own data, not its CLR type -
//         ItemsControl.ItemTemplateSelector calls SelectTemplate(item, container) for every
//         item, and whatever DataTemplate comes back is what that item's container uses.
// Drills: DataTemplateSelector.SelectTemplate - overriding it to branch on
//         Ex034_ExpenseItem.IsOverBudget and return one of two different, already-built
//         DataTemplates. BuildItemsControl (ready to use - not the subject of this row) wires
//         a selector into an ItemsControl instead of ItemTemplate, so the two templates seen
//         side by side in the same list is what actually proves the selector - not just a
//         style property - decided which one rendered.
// Passes: dotnet test --filter FullyQualifiedName~Ex034_

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Ready to use - not the subject of this row.
/// </summary>
public class Ex034_ExpenseItem
{
    public string Label { get; set; } = "";
    public bool IsOverBudget { get; set; }
}

public class Ex034_ExpenseTemplateSelector : DataTemplateSelector
{
    public required DataTemplate NormalTemplate { get; init; }
    public required DataTemplate OverBudgetTemplate { get; init; }

    /// <summary>
    /// Returns OverBudgetTemplate when <paramref name="item"/> is an Ex034_ExpenseItem whose
    /// IsOverBudget is true, and NormalTemplate otherwise.
    /// </summary>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
        // TODO: return item is Ex034_ExpenseItem { IsOverBudget: true } ? OverBudgetTemplate : NormalTemplate;
        => throw new NotImplementedException("TODO: Ex034 - return OverBudgetTemplate when item is an Ex034_ExpenseItem with IsOverBudget == true, NormalTemplate otherwise");
}

public static class Ex034_DataTemplateSelector
{
    private static DataTemplate BuildTemplate(string prefix)
    {
        var template = new DataTemplate(typeof(Ex034_ExpenseItem));
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex034_ExpenseItem.Label)) { StringFormat = prefix + "{0}" });
        template.VisualTree = factory;
        return template;
    }

    /// <summary>
    /// Ready to use - not the subject of this row. Builds two DataTemplates (one prefixed
    /// with <paramref name="normalPrefix"/>, one with <paramref name="overBudgetPrefix"/>),
    /// wires them into an Ex034_ExpenseTemplateSelector, and returns an ItemsControl whose
    /// ItemTemplateSelector is that selector.
    /// </summary>
    public static ItemsControl BuildItemsControl(IEnumerable<Ex034_ExpenseItem> items, string normalPrefix, string overBudgetPrefix)
    {
        var selector = new Ex034_ExpenseTemplateSelector
        {
            NormalTemplate = BuildTemplate(normalPrefix),
            OverBudgetTemplate = BuildTemplate(overBudgetPrefix),
        };

        return new ItemsControl { ItemsSource = items, ItemTemplateSelector = selector };
    }
}
