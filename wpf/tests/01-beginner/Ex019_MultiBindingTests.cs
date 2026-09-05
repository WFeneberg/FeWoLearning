using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex019_MultiBindingTests : WpfTestContext
{
    [WpfFact]
    public void Convert_Combines_Both_Values_In_Order()
    {
        var converter = new Ex019_FullNameConverter();

        var result = converter.Convert(new object[] { "Ada", "Lovelace" }, typeof(string), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Ada Lovelace", result);
    }

    [WpfFact]
    public void The_Display_Shows_Both_Names_Combined()
    {
        var source = new Ex019_PersonNameSource { FirstName = "Ada", LastName = "Lovelace" };
        var target = new TextBlock();

        Ex019_MultiBinding.Bind(target, source);
        Layout(target);
        Pump();

        Assert.Equal("Ada Lovelace", target.Text);
    }

    [WpfFact]
    public void A_Later_FirstName_Change_Still_Reaches_The_Target()
    {
        var source = new Ex019_PersonNameSource { FirstName = "Ada", LastName = "Lovelace" };
        var target = new TextBlock();
        Ex019_MultiBinding.Bind(target, source);
        Layout(target);
        Pump();

        source.FirstName = "Grace";
        Pump();

        Assert.Equal("Grace Lovelace", target.Text);
    }

    [WpfFact]
    public void A_Later_LastName_Change_Also_Reaches_The_Target()
    {
        var source = new Ex019_PersonNameSource { FirstName = "Ada", LastName = "Lovelace" };
        var target = new TextBlock();
        Ex019_MultiBinding.Bind(target, source);
        Layout(target);
        Pump();

        // Changing only the second binding's source, not the first - a learner who
        // wired up just one Binding and hard-coded the other name in the converter
        // would pass the FirstName-change test above but fail this one, or vice versa.
        source.LastName = "Hopper";
        Pump();

        Assert.Equal("Ada Hopper", target.Text);
    }

    [WpfFact]
    public void The_MultiBinding_Is_Declared_With_Both_Paths_In_Order_And_The_Converter()
    {
        var source = new Ex019_PersonNameSource();
        var target = new TextBlock();

        Ex019_MultiBinding.Bind(target, source);

        var multiBinding = BindingOperations.GetMultiBinding(target, TextBlock.TextProperty);

        Assert.NotNull(multiBinding);
        Assert.IsType<Ex019_FullNameConverter>(multiBinding!.Converter);
        Assert.Equal(2, multiBinding.Bindings.Count);

        var first = Assert.IsType<Binding>(multiBinding.Bindings[0]);
        var second = Assert.IsType<Binding>(multiBinding.Bindings[1]);
        Assert.Equal(nameof(Ex019_PersonNameSource.FirstName), first.Path.Path);
        Assert.Equal(nameof(Ex019_PersonNameSource.LastName), second.Path.Path);
    }
}
