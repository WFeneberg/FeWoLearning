using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Advanced;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Advanced;

public class Ex085_VirtualizationBudgetTests
{
    private static ListBox Shown(ListBox list, double windowHeight)
    {
        ViewHarness.ShowWindow(list, 220, windowHeight);
        ViewHarness.PumpRender();
        return list;
    }

    private static int Realized(ListBox list) => list.GetRealizedContainers()?.Count() ?? -1;

    [AvaloniaFact]
    public void Both_Lists_Show_The_Same_Five_Hundred_Rows()
    {
        Assert.Equal(500, Ex085_VirtualizationBudget.Rows.Count);
        Assert.Equal(Ex085_VirtualizationBudget.Rows, Ex085_VirtualizationBudget.BuildVirtualizing(120).ItemsSource);
        Assert.Equal(Ex085_VirtualizationBudget.Rows, Ex085_VirtualizationBudget.BuildNonVirtualizing(120).ItemsSource);
    }

    // The budget itself. The bound is deliberately generous - a row's height
    // depends on font metrics, and this harness measured about 37 units, giving 4
    // realized containers in a 120-unit viewport - but two orders of magnitude
    // below 500 is not a matter of measurement noise.
    [AvaloniaFact]
    public void The_Virtualizing_List_Realizes_A_Viewports_Worth()
    {
        var list = Shown(Ex085_VirtualizationBudget.BuildVirtualizing(120), 160);

        Assert.InRange(Realized(list), 1, 25);
    }

    [AvaloniaFact]
    public void A_Taller_Viewport_Realizes_More()
    {
        var small = Realized(Shown(Ex085_VirtualizationBudget.BuildVirtualizing(60), 100));
        var large = Realized(Shown(Ex085_VirtualizationBudget.BuildVirtualizing(300), 340));

        Assert.True(large > small, $"expected a 300-unit viewport to realize more than a 60-unit one, got {large} against {small}");
    }

    // The contrast that makes the number mean something: the same 500 rows with a
    // plain StackPanel realize every single container, whatever the viewport.
    [AvaloniaFact]
    public void The_Non_Virtualizing_List_Realizes_Everything()
    {
        var list = Shown(Ex085_VirtualizationBudget.BuildNonVirtualizing(120), 160);

        Assert.Equal(500, Realized(list));
    }

    [AvaloniaFact]
    public void Only_The_Virtualizing_List_Has_A_Virtualizing_Panel()
    {
        var virtualizing = Shown(Ex085_VirtualizationBudget.BuildVirtualizing(120), 160);
        Assert.IsType<VirtualizingStackPanel>(virtualizing.ItemsPanelRoot);

        var plain = Shown(Ex085_VirtualizationBudget.BuildNonVirtualizing(120), 160);
        Assert.IsType<StackPanel>(plain.ItemsPanelRoot);
    }

    // Recycling, which is the other half of virtualization and the part people
    // forget: the containers are not merely few, they are REUSED. Measured, after
    // scrolling to row 300 the realized range was 297..300 and the container that
    // had been showing row 0 was gone.
    [AvaloniaFact]
    public void Scrolling_Recycles_Containers_Rather_Than_Adding_More()
    {
        var list = Shown(Ex085_VirtualizationBudget.BuildVirtualizing(120), 160);
        var before = Realized(list);

        list.ScrollIntoView(300);
        Dispatcher.UIThread.RunJobs();
        ViewHarness.PumpRender();

        Assert.NotNull(list.ContainerFromIndex(300));
        Assert.Null(list.ContainerFromIndex(0));
        Assert.InRange(Realized(list), 1, before + 2);
    }

    [AvaloniaFact]
    public void The_Non_Virtualizing_List_Keeps_Every_Container_After_Scrolling()
    {
        var list = Shown(Ex085_VirtualizationBudget.BuildNonVirtualizing(120), 160);

        list.ScrollIntoView(300);
        Dispatcher.UIThread.RunJobs();
        ViewHarness.PumpRender();

        Assert.NotNull(list.ContainerFromIndex(300));
        Assert.NotNull(list.ContainerFromIndex(0));
        Assert.Equal(500, Realized(list));
    }
}
