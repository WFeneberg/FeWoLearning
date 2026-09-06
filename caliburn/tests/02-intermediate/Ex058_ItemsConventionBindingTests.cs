using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex058_ItemsConventionBindingTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <ItemsControl x:Name="Items" />
            <ItemsControl x:Name="NoSuchProperty" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex058_ItemsConventionBinding Subject, ItemsControl Matched, ItemsControl Unmatched) Bound()
    {
        var subject = new Ex058_ItemsConventionBinding();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(new Ex058_Vm(), view);
        return (subject, (ItemsControl)view.FindName("Items")!, (ItemsControl)view.FindName("NoSuchProperty")!);
    }

    [WpfFact]
    public void Matched_Name_Binds_ItemsSource_OneWay_To_The_Collection()
    {
        var (subject, matched, _) = Bound();

        var binding = subject.GetAppliedBinding(matched, ItemsControl.ItemsSourceProperty);

        Assert.NotNull(binding);
        Assert.Equal("Items", binding!.Path.Path);
        Assert.Equal(System.Windows.Data.BindingMode.OneWay, binding.Mode);
    }

    [WpfFact]
    public void Matched_ItemsControl_Actually_Materializes_The_Three_Items()
    {
        var (_, matched, _) = Bound();

        // A stub whose Bind never really runs the convention leaves ItemsSource null and Items
        // empty, even if GetAppliedBinding is implemented correctly.
        Assert.Equal(3, matched.Items.Count);
    }

    [WpfFact]
    public void A_Plain_String_Collection_Leaves_DisplayMemberPath_And_ItemTemplate_Untouched()
    {
        var (_, matched, _) = Bound();

        Assert.Equal("", matched.DisplayMemberPath);
        // Contrast ex060: a collection of view models DOES get an ItemTemplate assigned.
        Assert.Null(matched.ItemTemplate);
    }

    [WpfFact]
    public void Unmatched_Name_Gets_No_ItemsSource_Binding_At_All()
    {
        var (subject, _, unmatched) = Bound();

        Assert.Null(subject.GetAppliedBinding(unmatched, ItemsControl.ItemsSourceProperty));
    }

    [WpfFact]
    public void Unmatched_Name_Gets_No_Visibility_Fallback_Either()
    {
        var (subject, _, unmatched) = Bound();

        // Unlike a Button or TextBlock (ex019/ex020), where an unmatched name still falls back
        // to a Visibility binding, ItemsControl's own convention IS ItemsSource - a stub that
        // wires a Visibility binding here as a fallback fails right here.
        Assert.Null(subject.GetAppliedBinding(unmatched, UIElement.VisibilityProperty));
    }

    [WpfFact]
    public void GetAppliedBinding_Returns_Null_Before_Anything_Has_Been_Bound()
    {
        var subject = new Ex058_ItemsConventionBinding();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var matched = (ItemsControl)view.FindName("Items")!;

        Assert.Null(subject.GetAppliedBinding(matched, ItemsControl.ItemsSourceProperty));
    }
}
