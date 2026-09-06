using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex055_DataErrorInfoValidationTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <TextBox x:Name="UserName" />
          </StackPanel>
        </UserControl>
        """;

    static (Ex055_DataErrorInfoValidation Subject, TextBox UserNameBox) Bound(object viewModel)
    {
        var subject = new Ex055_DataErrorInfoValidation();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(viewModel, view);
        return (subject, (TextBox)view.FindName("UserName")!);
    }

    [WpfFact]
    public void IDataErrorInfo_View_Model_Gets_ValidatesOnDataErrors_True_On_The_Real_Binding()
    {
        var (subject, box) = Bound(new Ex055_ValidatingVm());

        var binding = subject.GetAppliedBinding(box, TextBox.TextProperty)!;

        Assert.True(binding.ValidatesOnDataErrors);
        // Documented trap, not a discriminator on its own: this one is true for EVERY binding,
        // WPF's own default - it proves nothing about Caliburn's convention by itself.
        Assert.True(binding.ValidatesOnNotifyDataErrors);
    }

    [WpfFact]
    public void Plain_View_Model_With_No_IDataErrorInfo_Gets_ValidatesOnDataErrors_False()
    {
        var (subject, box) = Bound(new Ex055_PlainVm());

        var binding = subject.GetAppliedBinding(box, TextBox.TextProperty)!;

        // A stub that hard-codes ValidatesOnDataErrors=true for every binding (instead of
        // letting the convention decide from IDataErrorInfo) fails right here.
        Assert.False(binding.ValidatesOnDataErrors);
    }

    [WpfFact]
    public void Validating_Vms_Indexer_Reports_An_Error_When_UserName_Is_Empty()
    {
        IDataErrorInfo vm = new Ex055_ValidatingVm();

        Assert.False(string.IsNullOrEmpty(vm["UserName"]));
    }

    [WpfFact]
    public void Validating_Vms_Indexer_Reports_No_Error_Once_UserName_Is_Set()
    {
        var vm = new Ex055_ValidatingVm { UserName = "Ada" };

        // A stub with the check inverted (erroring on a NON-empty value instead of an empty one)
        // fails here while the previous test still passes.
        Assert.True(string.IsNullOrEmpty(((IDataErrorInfo)vm)["UserName"]));
    }

    [WpfFact]
    public void Validating_Vms_Indexer_Ignores_An_Unrelated_Column_Name()
    {
        IDataErrorInfo vm = new Ex055_ValidatingVm();

        // A stub that returns an error string for ANY columnName (not just "UserName") fails
        // here, even though UserName really is empty.
        Assert.True(string.IsNullOrEmpty(vm["SomethingElse"]));
    }

    [WpfFact]
    public void GetAppliedBinding_Returns_Null_Before_Anything_Has_Been_Bound()
    {
        var subject = new Ex055_DataErrorInfoValidation();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var box = (TextBox)view.FindName("UserName")!;

        Assert.Null(subject.GetAppliedBinding(box, TextBox.TextProperty));
    }
}
