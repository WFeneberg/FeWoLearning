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
    public void Matched_ItemsControl_Actually_Materializes_The_Three_Items()
    {
        var (_, matched, _) = Bound();

        // A stub whose Bind never really runs the convention leaves ItemsSource null and Items
        // empty, even if the rest of this file's assertions read the right properties.
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
    public void LeavesPresentationAtDefaults_Is_True_For_A_Freshly_Bound_ItemsControl()
    {
        var (subject, matched, _) = Bound();

        Assert.True(subject.LeavesPresentationAtDefaults(matched));
    }

    [WpfFact]
    public void LeavesPresentationAtDefaults_Is_False_When_DisplayMemberPath_Is_Set()
    {
        var subject = new Ex058_ItemsConventionBinding();
        var itemsControl = new ItemsControl { DisplayMemberPath = "Label" };

        // A stub that only inspects ItemTemplate (ignoring DisplayMemberPath) fails right here.
        Assert.False(subject.LeavesPresentationAtDefaults(itemsControl));
    }

    [WpfFact]
    public void LeavesPresentationAtDefaults_Is_False_When_ItemTemplate_Is_Set()
    {
        var subject = new Ex058_ItemsConventionBinding();
        var itemsControl = new ItemsControl { ItemTemplate = new DataTemplate() };

        // A stub that only inspects DisplayMemberPath (ignoring ItemTemplate) fails right here.
        Assert.False(subject.LeavesPresentationAtDefaults(itemsControl));
    }

    [WpfFact]
    public void Unmatched_Name_Gets_No_ItemsSource_Binding_At_All()
    {
        var (subject, _, unmatched) = Bound();

        // The general rule this restates for ItemsControl is ex017's, not a fact peculiar to
        // ItemsControl: ViewModelBinder skips an element entirely once its name fails to match
        // any view-model property, before ever consulting a convention for it.
        Assert.Null(subject.GetAppliedBinding(unmatched, ItemsControl.ItemsSourceProperty));
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
