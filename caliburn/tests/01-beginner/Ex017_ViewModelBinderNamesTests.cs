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

        // Not bound on its natural property - TextBox's own convention is Text, so this
        // alone only proves TextBox.Text stayed unbound, nothing more.
        var bogus = (TextBox)view.FindName("Bogus")!;
        Assert.Null(BindingOperations.GetBinding(bogus, TextBox.TextProperty));
    }

    [WpfFact]
    public void An_Unmatched_Name_Gets_No_Binding_Even_On_An_Element_Whose_Own_Convention_Is_The_Visibility_Fallback()
    {
        // Border has no convention of its own - ex019/ex020 measure that ConventionManager
        // falls back to a Visibility binding for it. Asserting "no Visibility binding" on a
        // TextBox (whose own convention is Text, never Visibility) would be true whether or
        // not ViewModelBinder ever consulted that fallback - it proves nothing. Border makes
        // the assertion real, because a wrong ViewModelBinder that silently redirected an
        // unmatched name to the fallback convention WOULD produce a Visibility binding here.
        const string xaml = """
            <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
              <StackPanel>
                <Border x:Name="UserName" />
                <Border x:Name="Bogus2" />
              </StackPanel>
            </UserControl>
            """;
        var view = (FrameworkElement)XamlReader.Parse(xaml);

        new Ex017_ViewModelBinderNames().Bind(new Ex017_Vm(), view);

        // The half that makes the assertion below falsifiable: the fallback pathway is
        // genuinely reachable - a Border named after a property the view model DOES have
        // gets a real Visibility binding, a preview of ex020's lesson.
        var matched = (Border)view.FindName("UserName")!;
        var matchedBinding = BindingOperations.GetBinding(matched, FrameworkElement.VisibilityProperty);
        Assert.NotNull(matchedBinding);
        Assert.Equal("UserName", matchedBinding!.Path.Path);

        // The unmatched Border gets nothing - ViewModelBinder never asks ConventionManager
        // for a name it cannot match to a property in the first place.
        var unmatched = (Border)view.FindName("Bogus2")!;
        Assert.Null(BindingOperations.GetBinding(unmatched, FrameworkElement.VisibilityProperty));
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
