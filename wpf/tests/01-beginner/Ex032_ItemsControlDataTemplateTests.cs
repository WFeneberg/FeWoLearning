using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex032_ItemsControlDataTemplateTests : WpfTestContext
{
    private static string TemplatedText(ItemContainerGenerator generator, object item)
    {
        var container = generator.ContainerFromItem(item);
        Assert.NotNull(container);
        var presenter = Assert.IsType<ContentPresenter>(container);
        Assert.True(VisualTreeHelper.GetChildrenCount(presenter) > 0, "The ContentPresenter has no templated child - the DataTemplate never applied.");
        var textBlock = Assert.IsType<TextBlock>(VisualTreeHelper.GetChild(presenter, 0));
        return textBlock.Text;
    }

    [WpfFact]
    public void BuildProductList_Wires_ItemsSource_And_A_DataTemplate_Targeting_The_Product()
    {
        var products = new[] { new Ex032_Product { Name = "Kettle" } };
        var ic = Ex032_ItemsControlDataTemplate.BuildProductList(products);

        Assert.Same(products, ic.ItemsSource);
        Assert.NotNull(ic.ItemTemplate);
        Assert.Equal(typeof(Ex032_Product), ic.ItemTemplate!.DataType);
    }

    [WpfFact]
    public void Every_Item_Gets_Its_Own_Generated_Container_With_The_Templated_Text()
    {
        var products = new[]
        {
            new Ex032_Product { Name = "Kettle" },
            new Ex032_Product { Name = "Toaster" },
            new Ex032_Product { Name = "Blender" },
        };
        var ic = Ex032_ItemsControlDataTemplate.BuildProductList(products);
        CompleteInitialization(ic);
        Layout(ic);

        Assert.Equal(GeneratorStatus.ContainersGenerated, ic.ItemContainerGenerator.Status);

        // Not just the first item - every item must have realized its own container.
        Assert.Equal("Kettle", TemplatedText(ic.ItemContainerGenerator, products[0]));
        Assert.Equal("Toaster", TemplatedText(ic.ItemContainerGenerator, products[1]));
        Assert.Equal("Blender", TemplatedText(ic.ItemContainerGenerator, products[2]));
    }

    [WpfFact]
    public void A_Different_Product_List_Renders_Different_Templated_Text()
    {
        // Different names than every test above - a hard-coded "Kettle"/"Toaster"/"Blender"
        // cannot satisfy this one too.
        var products = new[]
        {
            new Ex032_Product { Name = "Lamp" },
            new Ex032_Product { Name = "Mirror" },
        };
        var ic = Ex032_ItemsControlDataTemplate.BuildProductList(products);
        CompleteInitialization(ic);
        Layout(ic);

        Assert.Equal("Lamp", TemplatedText(ic.ItemContainerGenerator, products[0]));
        Assert.Equal("Mirror", TemplatedText(ic.ItemContainerGenerator, products[1]));
    }
}
