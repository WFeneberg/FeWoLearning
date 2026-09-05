using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex019_ElementConventionLookupTests : CaliburnViewContext
{
    [WpfFact]
    public void CheckBox_Is_Served_By_The_ToggleButton_Convention_Not_Its_Own()
    {
        var subject = new Ex019_ElementConventionLookup();

        var convention = subject.Lookup(typeof(CheckBox));

        Assert.Equal(typeof(ToggleButton), convention.ElementType);
        Assert.Equal(nameof(ToggleButton.IsChecked), convention.GetBindableProperty(new CheckBox())?.Name);
        Assert.Equal(nameof(ToggleButton.IsChecked), convention.ParameterProperty);
    }

    [WpfFact]
    public void ComboBox_And_ListBox_Both_Resolve_To_The_Same_Selector_Convention()
    {
        var subject = new Ex019_ElementConventionLookup();

        var combo = subject.Lookup(typeof(ComboBox));
        var list = subject.Lookup(typeof(ListBox));

        Assert.Equal(typeof(Selector), combo.ElementType);
        Assert.Equal(typeof(Selector), list.ElementType);
        Assert.Equal("ItemsSource", combo.GetBindableProperty(new ComboBox())?.Name);
        Assert.Equal("SelectedItem", combo.ParameterProperty);
    }

    [WpfFact]
    public void TextBox_Resolves_To_Its_Own_Convention_Not_A_Fallback()
    {
        var subject = new Ex019_ElementConventionLookup();

        var convention = subject.Lookup(typeof(TextBox));

        Assert.Equal(typeof(TextBox), convention.ElementType);
        Assert.Equal("Text", convention.GetBindableProperty(new TextBox())?.Name);
    }

    [WpfFact]
    public void A_Type_With_No_Convention_Of_Its_Own_Falls_Back_To_FrameworkElement_And_Visibility()
    {
        var subject = new Ex019_ElementConventionLookup();

        // Border, Grid, Viewbox and friends all measure the same way - Border stands in
        // for the whole family here.
        var convention = subject.Lookup(typeof(Border));

        Assert.Equal(typeof(FrameworkElement), convention.ElementType);
        Assert.Equal("Visibility", convention.GetBindableProperty(new Border())?.Name);
        Assert.Equal("DataContext", convention.ParameterProperty);
    }

    [WpfFact]
    public void Lookup_Never_Returns_Null_Even_For_A_FrameworkElement_Caliburn_Has_Never_Heard_Of()
    {
        var subject = new Ex019_ElementConventionLookup();

        var convention = subject.Lookup(typeof(Ex019_NeverSeenElement));

        Assert.NotNull(convention);
        Assert.Equal(typeof(FrameworkElement), convention.ElementType);
    }

    [WpfFact]
    public void A_Custom_CheckBox_Subclass_Still_Walks_Up_To_The_ToggleButton_Convention()
    {
        var subject = new Ex019_ElementConventionLookup();

        // A naive lookup keyed only by EXACT type (a plain Dictionary<Type,...> with no
        // walk up the base-type chain) would find nothing for a subclass no one ever
        // registered - the real ConventionManager keeps climbing until it lands somewhere.
        var convention = subject.Lookup(typeof(Ex019_CustomCheckBox));

        Assert.Equal(typeof(ToggleButton), convention.ElementType);
    }
}
