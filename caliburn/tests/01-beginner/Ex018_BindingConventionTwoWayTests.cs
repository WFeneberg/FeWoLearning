using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex018_BindingConventionTwoWayTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <TextBox x:Name="UserName" />
            <TextBlock x:Name="Description" />
            <CheckBox x:Name="IsHappy" />
            <ItemsControl x:Name="Items" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex018_BindingConventionTwoWay Subject, FrameworkElement View) Bound()
    {
        var subject = new Ex018_BindingConventionTwoWay();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(new Ex018_Vm(), view);
        return (subject, view);
    }

    [WpfFact]
    public void Settable_String_On_A_TwoWay_Capable_Element_Is_TwoWay_With_PropertyChanged()
    {
        var (subject, view) = Bound();
        var userName = (TextBox)view.FindName("UserName")!;

        var binding = subject.GetAppliedBinding(userName, TextBox.TextProperty)!;

        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);
    }

    [WpfFact]
    public void GetOnly_String_Is_OneWay_Never_TwoWay_Even_Though_The_Element_Would_Support_It()
    {
        var (subject, view) = Bound();
        var description = (TextBlock)view.FindName("Description")!;

        var binding = subject.GetAppliedBinding(description, TextBlock.TextProperty)!;

        // A wrong implementation that hard-codes TwoWay for every matched element would
        // pass the test above and fail only here - the property side matters too.
        Assert.Equal(BindingMode.OneWay, binding.Mode);
    }

    [WpfFact]
    public void Settable_Bool_On_A_CheckBox_Is_Also_TwoWay_With_PropertyChanged_Not_Just_TextBoxes()
    {
        var (subject, view) = Bound();
        var isHappy = (CheckBox)view.FindName("IsHappy")!;

        var binding = subject.GetAppliedBinding(isHappy, CheckBox.IsCheckedProperty)!;

        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);
    }

    [WpfFact]
    public void A_Collection_Property_Binds_ItemsSource_OneWay()
    {
        var (subject, view) = Bound();
        var items = (ItemsControl)view.FindName("Items")!;

        var binding = subject.GetAppliedBinding(items, ItemsControl.ItemsSourceProperty)!;

        Assert.Equal(BindingMode.OneWay, binding.Mode);
        Assert.Equal("Items", binding.Path.Path);
    }

    [WpfFact]
    public void Caliburns_TextBox_Binding_Overrides_WPFs_Own_LostFocus_Default_With_PropertyChanged()
    {
        // WPF's OWN default for TextBox.Text, straight from the dependency property's
        // metadata - nothing to do with Caliburn yet.
        var metadata = (FrameworkPropertyMetadata)TextBox.TextProperty.GetMetadata(typeof(TextBox));
        Assert.Equal(UpdateSourceTrigger.LostFocus, metadata.DefaultUpdateSourceTrigger);

        var (subject, view) = Bound();
        var userName = (TextBox)view.FindName("UserName")!;
        var binding = subject.GetAppliedBinding(userName, TextBox.TextProperty)!;

        // Caliburn's convention EXPLICITLY sets PropertyChanged - it does not leave the
        // trigger at Default and let WPF fall back to LostFocus.
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, binding.UpdateSourceTrigger);
    }

    [WpfFact]
    public void GetAppliedBinding_Returns_Null_Before_Anything_Has_Been_Bound()
    {
        var subject = new Ex018_BindingConventionTwoWay();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var userName = (TextBox)view.FindName("UserName")!;

        Assert.Null(subject.GetAppliedBinding(userName, TextBox.TextProperty));
    }
}
