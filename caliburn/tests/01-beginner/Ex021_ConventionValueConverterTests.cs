using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex021_ConventionValueConverterTests : CaliburnViewContext
{
    const string Xaml = """
        <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
          <StackPanel>
            <Border x:Name="IsVisible" />
            <TextBlock x:Name="Count" />
          </StackPanel>
        </UserControl>
        """;

    // ex021 is binding-only and needs no window (see brief), but hosting it with Show is
    // harmless and keeps this batch uniform with ex022-ex025, which do need one.
    (Ex021_ConventionValueConverter Subject, FrameworkElement View) Bound()
    {
        var subject = new Ex021_ConventionValueConverter();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        subject.Bind(new Ex021_Vm(), view);
        Show(view);
        return (subject, view);
    }

    [WpfFact]
    public void Bool_Onto_A_Borders_Visibility_Gets_The_BooleanToVisibilityConverter()
    {
        var (subject, view) = Bound();
        var border = (Border)view.FindName("IsVisible")!;

        var binding = subject.GetAppliedBinding(border, UIElement.VisibilityProperty)!;

        Assert.IsType<BooleanToVisibilityConverter>(binding.Converter);
    }

    [WpfFact]
    public void Int_Onto_A_TextBlocks_Text_Gets_No_Converter()
    {
        var (subject, view) = Bound();
        var textBlock = (TextBlock)view.FindName("Count")!;

        var binding = subject.GetAppliedBinding(textBlock, TextBlock.TextProperty)!;

        // A wrong implementation that reaches for a converter on every type-mismatched pair -
        // instead of only where the convention actually decides bridging is needed - would
        // pass the Border test above and fail only here.
        Assert.Null(binding.Converter);
    }

    [WpfFact]
    public void Int_Onto_A_TextBlocks_Text_Also_Gets_No_StringFormat()
    {
        var (subject, view) = Bound();
        var textBlock = (TextBlock)view.FindName("Count")!;

        var binding = subject.GetAppliedBinding(textBlock, TextBlock.TextProperty)!;

        Assert.Null(binding.StringFormat);
    }

    [WpfFact]
    public void The_Borders_Binding_Path_Is_Still_IsVisible_Not_A_Fabricated_Static_Binding()
    {
        var (subject, view) = Bound();
        var border = (Border)view.FindName("IsVisible")!;

        var binding = subject.GetAppliedBinding(border, UIElement.VisibilityProperty)!;

        // A wrong implementation could hand back some ad hoc Binding pre-loaded with the right
        // converter type without ever really binding to the view model - checking the Path
        // rules that out.
        Assert.Equal("IsVisible", binding.Path.Path);
    }

    [WpfFact]
    public void GetAppliedBinding_Returns_Null_Before_Anything_Has_Been_Bound()
    {
        var subject = new Ex021_ConventionValueConverter();
        var view = (FrameworkElement)XamlReader.Parse(Xaml);
        var border = (Border)view.FindName("IsVisible")!;

        Assert.Null(subject.GetAppliedBinding(border, UIElement.VisibilityProperty));
    }
}
