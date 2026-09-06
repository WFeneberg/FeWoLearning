using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex056_NotifyDataErrorInfoValidationTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <TextBox x:Name="UserName" />
          </StackPanel>
        </UserControl>
        """;

    static IEnumerable<object> ToList(IEnumerable errors)
    {
        var list = new List<object>();
        foreach (var e in errors) list.Add(e);
        return list;
    }

    [WpfFact]
    public void Fresh_Vm_Has_No_Errors_And_HasErrors_Is_False()
    {
        var vm = new Ex056_AsyncValidatingVm();

        // A stub that reports errors before any validation has ever run fails right here.
        Assert.False(vm.HasErrors);
    }

    [WpfFact]
    public void GetErrors_For_UserName_Is_Empty_Before_Any_Validation_Runs()
    {
        var vm = new Ex056_AsyncValidatingVm();

        Assert.Empty(ToList(vm.GetErrors(nameof(Ex056_AsyncValidatingVm.UserName))));
    }

    [WpfFact]
    public async Task Errors_Do_Not_Appear_While_The_Async_Validation_Is_Still_Pending()
    {
        var vm = new Ex056_AsyncValidatingVm();
        var outcome = new TaskCompletionSource<bool>();

        var validating = vm.ValidateUserNameAsync(outcome.Task);

        // The outcome has not been decided yet - a stub that marks an error synchronously,
        // before ever awaiting isValid, fails right here.
        Assert.False(vm.HasErrors);
        Assert.Equal(0, vm.ErrorsChangedRaisedCount);

        outcome.SetResult(false);
        await validating;
        Pump();
    }

    [WpfFact]
    public async Task Completing_The_Validation_As_Invalid_Sets_HasErrors_And_Raises_ErrorsChanged_Exactly_Once()
    {
        var vm = new Ex056_AsyncValidatingVm();
        var outcome = new TaskCompletionSource<bool>();

        var validating = vm.ValidateUserNameAsync(outcome.Task);
        outcome.SetResult(false);
        await validating;
        Pump();

        Assert.True(vm.HasErrors);
        Assert.NotEmpty(ToList(vm.GetErrors(nameof(Ex056_AsyncValidatingVm.UserName))));
        Assert.Equal(1, vm.ErrorsChangedRaisedCount);
    }

    [WpfFact]
    public async Task Completing_A_Later_Validation_As_Valid_Clears_The_Error_And_Raises_ErrorsChanged_Again()
    {
        var vm = new Ex056_AsyncValidatingVm();
        var firstOutcome = new TaskCompletionSource<bool>();
        var validatingFirst = vm.ValidateUserNameAsync(firstOutcome.Task);
        firstOutcome.SetResult(false);
        await validatingFirst;
        Pump();

        var secondOutcome = new TaskCompletionSource<bool>();
        var validatingSecond = vm.ValidateUserNameAsync(secondOutcome.Task);
        secondOutcome.SetResult(true);
        await validatingSecond;
        Pump();

        // A stub that only ever ADDS errors (never clears one once recorded) fails right here.
        Assert.False(vm.HasErrors);
        Assert.Empty(ToList(vm.GetErrors(nameof(Ex056_AsyncValidatingVm.UserName))));
        Assert.Equal(2, vm.ErrorsChangedRaisedCount);
    }

    [WpfFact]
    public async Task GetErrors_Ignores_An_Unrelated_Property_Name()
    {
        var vm = new Ex056_AsyncValidatingVm();
        var outcome = new TaskCompletionSource<bool>();
        var validating = vm.ValidateUserNameAsync(outcome.Task);
        outcome.SetResult(false);
        await validating;
        Pump();

        // A stub that returns every recorded error regardless of propertyName fails right here,
        // even though UserName really does have one.
        Assert.Empty(ToList(vm.GetErrors("SomethingElse")));
    }

    [WpfFact]
    public void INotifyDataErrorInfo_Gets_ValidatesOnDataErrors_False_While_IDataErrorInfo_Gets_True_On_The_Real_Binding()
    {
        var subject = new Ex056_NotifyDataErrorInfoValidation();

        var notifyView = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(new Ex056_AsyncValidatingVm(), notifyView);
        var notifyBinding = subject.GetAppliedBinding((TextBox)notifyView.FindName("UserName")!, TextBox.TextProperty)!;

        var classicView = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(new Ex056_ClassicDataErrorInfoVm(), classicView);
        var classicBinding = subject.GetAppliedBinding((TextBox)classicView.FindName("UserName")!, TextBox.TextProperty)!;

        // A stub whose Bind never actually runs the convention (or a GetAppliedBinding that
        // reads the wrong element) leaves both null and fails both assertions below.
        Assert.False(notifyBinding.ValidatesOnDataErrors);
        Assert.True(classicBinding.ValidatesOnDataErrors);
        // Documented trap, not a discriminator on its own: true for BOTH - WPF's own default,
        // nothing to do with Caliburn's convention.
        Assert.True(notifyBinding.ValidatesOnNotifyDataErrors);
        Assert.True(classicBinding.ValidatesOnNotifyDataErrors);
    }
}
