using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex034_DataTemplateSelectorTests : WpfTestContext
{
    private static (DataTemplate normal, DataTemplate overBudget) NewTemplates()
        => (new DataTemplate(typeof(Ex034_ExpenseItem)), new DataTemplate(typeof(Ex034_ExpenseItem)));

    [WpfFact]
    public void SelectTemplate_Returns_The_OverBudget_Template_When_IsOverBudget_Is_True()
    {
        var (normal, overBudget) = NewTemplates();
        var selector = new Ex034_ExpenseTemplateSelector { NormalTemplate = normal, OverBudgetTemplate = overBudget };

        var result = selector.SelectTemplate(new Ex034_ExpenseItem { Label = "Rent", IsOverBudget = true }, new Border());

        Assert.Same(overBudget, result);
    }

    [WpfFact]
    public void SelectTemplate_Returns_The_Normal_Template_When_IsOverBudget_Is_False()
    {
        var (normal, overBudget) = NewTemplates();
        var selector = new Ex034_ExpenseTemplateSelector { NormalTemplate = normal, OverBudgetTemplate = overBudget };

        var result = selector.SelectTemplate(new Ex034_ExpenseItem { Label = "Coffee", IsOverBudget = false }, new Border());

        Assert.Same(normal, result);
    }

    private static string TemplatedText(ItemContainerGenerator generator, object item)
    {
        var container = generator.ContainerFromItem(item);
        Assert.NotNull(container);
        var presenter = Assert.IsType<ContentPresenter>(container);
        Assert.True(VisualTreeHelper.GetChildrenCount(presenter) > 0);
        return Assert.IsType<TextBlock>(VisualTreeHelper.GetChild(presenter, 0)).Text;
    }

    [WpfFact]
    public void The_Selector_Drives_Which_Template_An_ItemsControl_Actually_Realizes_Per_Item()
    {
        var items = new[]
        {
            new Ex034_ExpenseItem { Label = "Coffee", IsOverBudget = false },
            new Ex034_ExpenseItem { Label = "Rent", IsOverBudget = true },
        };
        var ic = Ex034_DataTemplateSelector.BuildItemsControl(items, normalPrefix: "OK:", overBudgetPrefix: "OVER:");
        CompleteInitialization(ic);
        Layout(ic);

        Assert.Equal("OK:Coffee", TemplatedText(ic.ItemContainerGenerator, items[0]));
        Assert.Equal("OVER:Rent", TemplatedText(ic.ItemContainerGenerator, items[1]));
    }

    [WpfFact]
    public void A_Different_Item_Set_And_Prefixes_Select_Correctly_Too()
    {
        // Different prefixes and reversed IsOverBudget assignment vs the test above - a
        // selector that always returns the same template, or one that has the branches
        // backwards, fails one of these two tests.
        var items = new[]
        {
            new Ex034_ExpenseItem { Label = "Groceries", IsOverBudget = true },
            new Ex034_ExpenseItem { Label = "Bus pass", IsOverBudget = false },
        };
        var ic = Ex034_DataTemplateSelector.BuildItemsControl(items, normalPrefix: "Fine-", overBudgetPrefix: "Blown-");
        CompleteInitialization(ic);
        Layout(ic);

        Assert.Equal("Blown-Groceries", TemplatedText(ic.ItemContainerGenerator, items[0]));
        Assert.Equal("Fine-Bus pass", TemplatedText(ic.ItemContainerGenerator, items[1]));
    }
}
