using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex060_ItemTemplateViewLocatorTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <ItemsControl x:Name="Items" />
        </UserControl>
        """;

    static ItemsControl Bound(object viewModel)
    {
        var subject = new Ex060_ItemTemplateViewLocator();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(viewModel, view);
        return (ItemsControl)view.FindName("Items")!;
    }

    [WpfFact]
    public void A_Collection_Of_Plain_Strings_Gets_No_ItemTemplate()
    {
        var itemsControl = Bound(new Ex060_StringsVm());

        Assert.Null(itemsControl.ItemTemplate);
    }

    [WpfFact]
    public void A_Collection_Of_A_Value_Type_Gets_No_ItemTemplate_Either()
    {
        var itemsControl = Bound(new Ex060_IntsVm());

        Assert.Null(itemsControl.ItemTemplate);
    }

    [WpfFact]
    public void A_Collection_Of_View_Model_Rows_Gets_The_Frameworks_Own_DefaultItemTemplate()
    {
        var itemsControl = Bound(new Ex060_RowsVm());

        // A stub that assigns SOME non-null DataTemplate of its own (instead of letting the
        // convention decide) fails right here - it must be the exact framework instance.
        Assert.NotNull(itemsControl.ItemTemplate);
        Assert.Same(ConventionManager.DefaultItemTemplate, itemsControl.ItemTemplate);
    }

    [WpfFact]
    public void A_Collection_Of_Plain_Unrelated_Reference_Type_Objects_Gets_The_Same_Template()
    {
        // Scoped claim: this is decided by the item's CLR type shape (any reference type other
        // than string), NOT by it being a Caliburn Screen/PropertyChangedBase specifically - a
        // plain object with no relation to Caliburn gets the identical template.
        var itemsControl = Bound(new Ex060_PlainObjectsVm());

        Assert.Same(ConventionManager.DefaultItemTemplate, itemsControl.ItemTemplate);
    }

    [WpfFact]
    public void WouldGetDefaultItemTemplate_Is_False_For_String_Even_Though_String_Is_A_Reference_Type()
    {
        var subject = new Ex060_ItemTemplateViewLocator();

        // The trap: a naive "!itemType.IsValueType" predicate would wrongly say true here,
        // since System.String is a reference type - the framework excludes it anyway.
        Assert.False(subject.WouldGetDefaultItemTemplate(typeof(string)));
    }

    [WpfFact]
    public void WouldGetDefaultItemTemplate_Is_False_For_A_Value_Type()
    {
        var subject = new Ex060_ItemTemplateViewLocator();

        Assert.False(subject.WouldGetDefaultItemTemplate(typeof(int)));
        Assert.False(subject.WouldGetDefaultItemTemplate(typeof(Guid)));
    }

    [WpfFact]
    public void WouldGetDefaultItemTemplate_Is_True_For_Any_Other_Reference_Type()
    {
        var subject = new Ex060_ItemTemplateViewLocator();

        // A stub that only recognises Caliburn's own Screen/PropertyChangedBase types (instead
        // of the framework's broader, type-shape-only rule) fails on the plain object() case.
        Assert.True(subject.WouldGetDefaultItemTemplate(typeof(Ex060_RowItem)));
        Assert.True(subject.WouldGetDefaultItemTemplate(typeof(object)));
    }
}
