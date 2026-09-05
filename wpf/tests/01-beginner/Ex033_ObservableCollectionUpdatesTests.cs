using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex033_ObservableCollectionUpdatesTests : WpfTestContext
{
    private static string TemplatedText(ItemContainerGenerator generator, object item)
    {
        var container = generator.ContainerFromItem(item);
        Assert.NotNull(container);
        var presenter = Assert.IsType<ContentPresenter>(container);
        Assert.True(VisualTreeHelper.GetChildrenCount(presenter) > 0);
        return Assert.IsType<TextBlock>(VisualTreeHelper.GetChild(presenter, 0)).Text;
    }

    [WpfFact]
    public void Adding_To_The_Original_Collection_After_Building_Reaches_The_UI_After_A_Second_Layout()
    {
        var tasks = new ObservableCollection<Ex033_Task> { new() { Title = "Wash dishes" } };
        var ic = Ex033_ObservableCollectionUpdates.BuildItemsControl(tasks);
        CompleteInitialization(ic);
        Layout(ic);

        Assert.Single(ic.Items);
        Assert.Equal("Wash dishes", TemplatedText(ic.ItemContainerGenerator, tasks[0]));

        var newTask = new Ex033_Task { Title = "Take out trash" };
        // Mutating the ORIGINAL collection reference, not a copy - a wrong implementation
        // that assigned ItemsSource = tasks.ToList() would leave ic.Items.Count at 1 and
        // ContainerFromItem(newTask) at null here, no matter how many more times Layout(...)
        // runs.
        tasks.Add(newTask);

        // The second Layout(...) pass this row is about - the container's templated child
        // is not observable before this.
        Layout(ic);

        Assert.Equal(2, ic.Items.Count);
        Assert.Equal("Take out trash", TemplatedText(ic.ItemContainerGenerator, newTask));
    }

    [WpfFact]
    public void A_Different_Collection_And_Added_Item_Behave_The_Same_Way()
    {
        // Different starting items and a different added title than the test above - a
        // hard-coded "Take out trash" cannot satisfy both.
        var tasks = new ObservableCollection<Ex033_Task> { new() { Title = "Water plants" }, new() { Title = "Feed cat" } };
        var ic = Ex033_ObservableCollectionUpdates.BuildItemsControl(tasks);
        CompleteInitialization(ic);
        Layout(ic);

        var addedTask = new Ex033_Task { Title = "Pay rent" };
        tasks.Add(addedTask);
        Layout(ic);

        Assert.Equal(3, ic.Items.Count);
        Assert.Equal("Pay rent", TemplatedText(ic.ItemContainerGenerator, addedTask));
    }

    [WpfFact]
    public void Removing_An_Item_Removes_Its_Generated_Container()
    {
        var first = new Ex033_Task { Title = "A" };
        var second = new Ex033_Task { Title = "B" };
        var tasks = new ObservableCollection<Ex033_Task> { first, second };
        var ic = Ex033_ObservableCollectionUpdates.BuildItemsControl(tasks);
        CompleteInitialization(ic);
        Layout(ic);

        Assert.NotNull(ic.ItemContainerGenerator.ContainerFromItem(first));
        Assert.NotNull(ic.ItemContainerGenerator.ContainerFromItem(second));

        tasks.Remove(first);
        Layout(ic);

        Assert.Single(ic.Items);
        Assert.Null(ic.ItemContainerGenerator.ContainerFromItem(first));
        Assert.NotNull(ic.ItemContainerGenerator.ContainerFromItem(second));
    }
}
