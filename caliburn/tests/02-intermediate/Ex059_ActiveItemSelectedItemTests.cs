using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex059_ActiveItemSelectedItemTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <ListBox x:Name="Items" />
            <ContentControl x:Name="ActiveItem" />
          </StackPanel>
        </UserControl>
        """;

    static async Task<(Ex059_ActiveItemSelectedItem Subject, Ex059_Conductor Conductor, ListBox ListBox, ContentControl ContentControl, Ex059_Child First, Ex059_Child Second)> BoundAsync()
    {
        var subject = new Ex059_ActiveItemSelectedItem();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var conductor = new Ex059_Conductor();
        subject.Bind(conductor, view);

        var first = new Ex059_Child();
        var second = new Ex059_Child();
        await conductor.ActivateBothAsync(first, second);

        return (subject, conductor, (ListBox)view.FindName("Items")!, (ContentControl)view.FindName("ActiveItem")!, first, second);
    }

    [WpfFact]
    public async Task ListBox_ItemsSource_Is_Bound_OneWay_To_Items()
    {
        var (subject, _, listBox, _, _, _) = await BoundAsync();

        var binding = subject.GetAppliedBinding(listBox, ItemsControl.ItemsSourceProperty);

        Assert.NotNull(binding);
        Assert.Equal("Items", binding!.Path.Path);
        Assert.Equal(BindingMode.OneWay, binding.Mode);
    }

    [WpfFact]
    public async Task ListBox_SelectedItem_Is_Bound_TwoWay_To_ActiveItem_Though_Nobody_Wrote_That_Binding()
    {
        var (subject, _, listBox, _, _, _) = await BoundAsync();

        var binding = subject.GetAppliedBinding(listBox, Selector.SelectedItemProperty);

        // A stub whose Bind never really runs the convention leaves this null.
        Assert.NotNull(binding);
        Assert.Equal("ActiveItem", binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
    }

    [WpfFact]
    public async Task ContentControl_Named_ActiveItem_Gets_No_Content_Binding()
    {
        var (subject, _, _, contentControl, _, _) = await BoundAsync();

        // A stub that (wrongly) expects ActiveItem to bind through plain Content fails here -
        // Content itself carries no binding at all.
        Assert.Null(subject.GetAppliedBinding(contentControl, ContentControl.ContentProperty));
    }

    [WpfFact]
    public async Task ContentControl_Named_ActiveItem_Gets_ViewModel_Bound_TwoWay_Through_Views_Own_Attached_Property()
    {
        var (subject, _, _, contentControl, _, _) = await BoundAsync();

        var binding = subject.GetAppliedBinding(contentControl, View.ModelProperty);

        Assert.NotNull(binding);
        Assert.Equal("ActiveItem", binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
    }

    [WpfFact]
    public async Task Activating_The_Second_Child_Through_The_Conductor_Selects_It_In_The_ListBox()
    {
        var (_, conductor, listBox, _, _, second) = await BoundAsync();
        Pump();

        // Nobody wrote any selection-syncing code - the convention's own two-way binding is what
        // moves this. A stub whose Bind never runs the convention leaves SelectedItem null.
        Assert.Same(second, conductor.ActiveItem);
        Assert.Same(second, listBox.SelectedItem);
    }

    [WpfFact]
    public async Task Selecting_The_First_Child_In_The_ListBox_Activates_It_On_The_Conductor()
    {
        var (_, conductor, listBox, _, first, _) = await BoundAsync();
        Pump();

        listBox.SelectedItem = first;
        Pump();

        // Driving the UI-side property (not the view model) still reaches ActiveItem - this is
        // what makes the binding genuinely TWO-WAY rather than a one-way display convenience.
        Assert.Same(first, conductor.ActiveItem);
    }

    [WpfFact]
    public void GetAppliedBinding_Returns_Null_Before_Anything_Has_Been_Bound()
    {
        var subject = new Ex059_ActiveItemSelectedItem();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var listBox = (ListBox)view.FindName("Items")!;

        Assert.Null(subject.GetAppliedBinding(listBox, ItemsControl.ItemsSourceProperty));
    }
}
