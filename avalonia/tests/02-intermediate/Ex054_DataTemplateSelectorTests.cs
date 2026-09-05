using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex054_DataTemplateSelectorTests
{
    private static (Ex054_DataTemplateSelector View, Ex054_DataTemplateSelectorViewModel Vm) Arrange()
    {
        var vm = new Ex054_DataTemplateSelectorViewModel();
        var view = ViewHarness.Show(new Ex054_DataTemplateSelector { DataContext = vm }, 300, 200);
        return (view, vm);
    }

    // Mechanism check: a control named Pets that is not actually an ItemsControl
    // (e.g. a hard-coded StackPanel given the same name) fails this typed lookup
    // outright, and a genuinely type-keyed dispatch needs more than one template
    // registered.
    [AvaloniaFact]
    public void Pets_Is_An_ItemsControl_With_TypeKeyed_DataTemplates()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Pets");

        Assert.NotNull(itemsControl);
        Assert.Same(vm.Pets, itemsControl!.ItemsSource);
        Assert.Equal(2, itemsControl.DataTemplates.Count);
    }

    [AvaloniaFact]
    public void Renders_Each_Item_Through_Its_Own_Type_Keyed_Template_In_Order()
    {
        var (view, _) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Pets")!;

        Assert.Equal(3, itemsControl.GetRealizedContainers().Count());
        var texts = itemsControl.GetVisualDescendants().OfType<TextBlock>()
            .Select(t => t.Text)
            .ToList();
        Assert.Equal(["dog: Rex", "cat: Tom", "dog: Fido"], texts);
    }

    // The real discriminator against a hard-coded panel: a repeated type
    // appended live (breaking the initial dog/cat/dog alternation, so an
    // index-based fake dispatch could not coincidentally get it right either)
    // must render through the same DataTemplates dispatch, reactively.
    [AvaloniaFact]
    public void Reacts_To_A_Newly_Added_Item_Of_A_Repeated_Type()
    {
        var (view, vm) = Arrange();
        var itemsControl = view.FindControl<ItemsControl>("Pets")!;

        vm.Pets.Add(new Ex054_Dog { Name = "Buddy" });
        Dispatcher.UIThread.RunJobs();

        Assert.Equal(4, itemsControl.GetRealizedContainers().Count());
        var texts = itemsControl.GetVisualDescendants().OfType<TextBlock>()
            .Select(t => t.Text)
            .ToList();
        Assert.Equal(["dog: Rex", "cat: Tom", "dog: Fido", "dog: Buddy"], texts);
    }
}
