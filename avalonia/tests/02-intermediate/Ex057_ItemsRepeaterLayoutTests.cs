using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex057_ItemsRepeaterLayoutTests
{
    private static (Ex057_ItemsRepeaterLayout View, Ex057_ItemsRepeaterLayoutViewModel Vm) Arrange()
    {
        var vm = new Ex057_ItemsRepeaterLayoutViewModel();
        var view = ViewHarness.Show(new Ex057_ItemsRepeaterLayout { DataContext = vm }, 200, 200);
        return (view, vm);
    }

    // Mechanism check: a WrapPanel filled with the same 80x20 items reproduces
    // the identical rectangles measured below - FindControl<ItemsRepeater> is
    // what a WrapPanel can never satisfy. Measured on this machine:
    // FindControl<T> against a same-named, differently-typed control THROWS
    // outright (InvalidOperationException), rather than returning null.
    [AvaloniaFact]
    public void Repeater_Is_An_ItemsRepeater_With_A_UniformGridLayout()
    {
        var (view, vm) = Arrange();
        var repeater = view.FindControl<ItemsRepeater>("Repeater");

        Assert.NotNull(repeater);
        Assert.Same(vm.Items, repeater!.ItemsSource);
        Assert.IsType<UniformGridLayout>(repeater.Layout);
    }

    // The real discriminator: a real two-column uniform grid, not just any
    // wrap. Measured on this machine: five 80x20 items in a 200-wide host land
    // at exactly these five points.
    [AvaloniaFact]
    public void Five_Items_Land_In_A_Two_Column_Uniform_Grid()
    {
        var (view, _) = Arrange();
        var repeater = view.FindControl<ItemsRepeater>("Repeater")!;
        Dispatcher.UIThread.RunJobs();

        var borders = repeater.GetVisualDescendants().OfType<Border>().ToList();
        Assert.Equal(5, borders.Count);

        var points = borders.Select(b => (b.Bounds.X, b.Bounds.Y)).ToList();
        Assert.Equal(
            [(0d, 0d), (80d, 0d), (0d, 20d), (80d, 20d), (0d, 40d)],
            points);

        var texts = repeater.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
        Assert.Equal(["a", "b", "c", "d", "e"], texts);
    }
}
