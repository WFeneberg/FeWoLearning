// Exercise 032 - ItemsControl, ItemsSource and DataTemplate (beginner).
// Goal:   Bind a plain ItemsControl to a list of view-model objects through ItemsSource, and
//         give each one a DataTemplate so what actually renders per item is templated markup
//         bound to the item's own properties, not the item's raw ToString().
// Drills: ItemsControl.ItemsSource, DataTemplate (built here with FrameworkElementFactory,
//         the code-only way to build a template with no XAML file in this exercise), and what
//         "generated containers" means concretely: ItemsControl.ItemContainerGenerator builds
//         one container per item during the panel's own measure pass, and
//         ItemContainerGenerator.ContainerFromItem(item) is how code reaches a specific
//         item's container afterward - here that container is a ContentPresenter, and its
//         templated child is the TextBlock the DataTemplate describes. IMPORTANT (measured
//         directly, and easy to miss): a bare ItemsControl built by plain code never resolves
//         its default template at all - ItemContainerGenerator.Status stays NotStarted
//         forever, even after Layout(...) - unless something completes WPF's
//         ISupportInitialize protocol first, which is what the test's
//         CompleteInitialization(...) call is for; see WpfTestContext for the measured detail.
// Passes: dotnet test --filter FullyQualifiedName~Ex032_

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Ready to use - not the subject of this row. A plain view-model item ItemsControl binds
/// against; deliberately not INotifyPropertyChanged (that is row 033's subject, not this
/// row's).
/// </summary>
public class Ex032_Product
{
    public string Name { get; set; } = "";
}

public static class Ex032_ItemsControlDataTemplate
{
    /// <summary>
    /// Builds an ItemsControl whose ItemsSource is <paramref name="products"/> and whose
    /// ItemTemplate is a DataTemplate targeting Ex032_Product, with a single TextBlock bound
    /// to Ex032_Product.Name as its visual tree.
    /// </summary>
    public static ItemsControl BuildProductList(IEnumerable<Ex032_Product> products)
        // TODO: var template = new DataTemplate(typeof(Ex032_Product));
        //       var factory = new FrameworkElementFactory(typeof(TextBlock));
        //       factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex032_Product.Name)));
        //       template.VisualTree = factory;
        //       return new ItemsControl { ItemsSource = products, ItemTemplate = template };
        => throw new NotImplementedException("TODO: Ex032 - build a DataTemplate(typeof(Ex032_Product)) whose VisualTree is a FrameworkElementFactory(typeof(TextBlock)) bound (SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex032_Product.Name)))) to the product's Name; return a new ItemsControl with ItemsSource = products and ItemTemplate set to that template");
}
