// Exercise 033 - ObservableCollection updates reaching generated containers (beginner). REFERENCE SOLUTION.
// Goal:   Bind ItemsControl.ItemsSource directly to the caller's ObservableCollection<T>
//         reference - not a copy of it - so that INotifyCollectionChanged, which
//         ObservableCollection<T> raises and a plain List<T> never does, is what keeps the
//         generated containers in sync with the collection after the ItemsControl already
//         exists.
// Drills: INotifyCollectionChanged reaching ItemContainerGenerator: adding to the SAME
//         ObservableCollection<T> instance after the ItemsControl was built generates a new
//         container for the new item, and removing an item removes its container - neither
//         happens if ItemsSource was assigned a *copy* of the collection (a wrong
//         implementation naturally reaches for `products.ToList()` or `new List<T>(...)`,
//         both of which sever the live link this row is about). IMPORTANT, measured directly:
//         a collection change reaches the generator's CONTAINER OBJECT synchronously (a
//         ContainerFromItem lookup right after Add already returns non-null, no Layout or
//         Pump needed) - but the new container's DataTemplate-instantiated CHILD CONTENT is
//         NOT realized yet at that point. Only after a second Layout(...) call (a plain
//         Pump() also works - either drains the dispatcher work the template instantiation is
//         queued on) does the templated child actually exist and show the bound text.

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Ready to use - not the subject of this row. A plain view-model item, same shape as row
/// 032's - this row is about the COLLECTION changing, not the item's own properties
/// changing (that would be a plain Binding update, already covered in row 004/012).
/// </summary>
public class Ex033_Task
{
    public string Title { get; set; } = "";
}

public static class Ex033_ObservableCollectionUpdates
{
    /// <summary>
    /// Builds an ItemsControl whose ItemsSource is the SAME <paramref name="tasks"/>
    /// reference passed in (not a copy), with an ItemTemplate binding a TextBlock to
    /// Ex033_Task.Title.
    /// </summary>
    public static ItemsControl BuildItemsControl(ObservableCollection<Ex033_Task> tasks)
    {
        var template = new DataTemplate(typeof(Ex033_Task));
        var factory = new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex033_Task.Title)));
        template.VisualTree = factory;
        return new ItemsControl { ItemsSource = tasks, ItemTemplate = template };
    }
}
