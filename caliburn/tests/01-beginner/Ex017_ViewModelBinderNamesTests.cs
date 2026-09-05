using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex017_ViewModelBinderNamesTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <TextBox x:Name="UserName" />
            <TextBlock x:Name="Description" />
            <CheckBox x:Name="IsHappy" />
            <TextBox x:Name="Bogus" />
          </StackPanel>
        </UserControl>
        """;

    static FrameworkElement NewView() => (FrameworkElement)XamlReader.Parse(Xaml);

    [WpfFact]
    public void Before_Binding_Nothing_Has_A_Binding_Yet()
    {
        var view = NewView();
        var userName = (TextBox)view.FindName("UserName")!;
        Assert.Null(BindingOperations.GetBinding(userName, TextBox.TextProperty));

        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        Assert.NotNull(BindingOperations.GetBinding(userName, TextBox.TextProperty));
    }

    [WpfFact]
    public void An_Element_Named_After_A_Settable_Property_Gets_A_Binding_To_That_Property()
    {
        var view = NewView();
        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        var userName = (TextBox)view.FindName("UserName")!;
        var binding = BindingOperations.GetBinding(userName, TextBox.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal("UserName", binding!.Path.Path);
    }

    [WpfFact]
    public void An_Element_Named_After_A_GetOnly_Property_Gets_A_Binding_Too()
    {
        var view = NewView();
        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        var description = (TextBlock)view.FindName("Description")!;
        var binding = BindingOperations.GetBinding(description, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal("Description", binding!.Path.Path);
    }

    [WpfFact]
    public void A_Settable_Bool_Property_Binds_A_CheckBox_Too_Not_Just_Strings()
    {
        var view = NewView();
        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        var isHappy = (CheckBox)view.FindName("IsHappy")!;
        var binding = BindingOperations.GetBinding(isHappy, CheckBox.IsCheckedProperty);

        Assert.NotNull(binding);
        Assert.Equal("IsHappy", binding!.Path.Path);
    }

    [WpfFact]
    public void An_Element_Named_After_Something_The_ViewModel_Does_Not_Have_Gets_No_Binding_At_All()
    {
        var view = NewView();
        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        var bogus = (TextBox)view.FindName("Bogus")!;

        // Not bound on its "natural" property, and not silently redirected to some other
        // dependency property either - ex019/ex020 show ConventionManager would fall back
        // to Visibility if ASKED; ViewModelBinder never asks for a name it cannot match.
        Assert.Null(BindingOperations.GetBinding(bogus, TextBox.TextProperty));
        Assert.Null(BindingOperations.GetBinding(bogus, FrameworkElement.VisibilityProperty));
    }

    [WpfFact]
    public void A_Differently_Named_ViewModel_Property_Produces_A_Binding_With_That_Same_Name_Not_A_Hardcoded_Path()
    {
        const string xaml = """
            <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
              <TextBox x:Name="Nickname" />
            </UserControl>
            """;
        var view = (FrameworkElement)XamlReader.Parse(xaml);

        new Ex017_ViewModelBinderNames().Bind(new Ex017_SecondVm(), view);

        var nickname = (TextBox)view.FindName("Nickname")!;
        var binding = BindingOperations.GetBinding(nickname, TextBox.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal("Nickname", binding!.Path.Path);
    }
}
