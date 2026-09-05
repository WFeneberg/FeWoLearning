using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

// ConventionManager.AddElementConvention has no public removal, so every fact in this class
// registers Ex020_RatingControl's convention for itself (idempotently - re-adding the same
// values is harmless) rather than relying on another test having done it first. That keeps
// every fact correct regardless of xunit's execution order within the class.
public class Ex020_CustomElementConventionTests : CaliburnViewContext
{
    static (StackPanel Panel, T Control) BuildView<T>(string name) where T : FrameworkElement, new()
    {
        var control = new T { Name = name };
        var panel = new StackPanel();
        panel.Children.Add(control);
        return (panel, control);
    }

    [WpfFact]
    public void Without_A_Registered_Convention_Naming_An_Element_Binds_Visibility_Instead_Of_The_Controls_Own_Property()
    {
        var subject = new Ex020_CustomElementConvention();
        var (panel, control) = BuildView<Ex020_UnregisteredControl>("Rating");

        subject.Bind(new Ex020_Vm(), panel);

        var visibilityBinding = BindingOperations.GetBinding(control, FrameworkElement.VisibilityProperty);
        Assert.NotNull(visibilityBinding);
        Assert.Equal("Rating", visibilityBinding!.Path.Path);
        Assert.Null(BindingOperations.GetBinding(control, Ex020_UnregisteredControl.ValueProperty));
    }

    [WpfFact]
    public void RegisterRatingControlConvention_Makes_GetElementConvention_Report_The_Value_Property()
    {
        var subject = new Ex020_CustomElementConvention();

        subject.RegisterRatingControlConvention();

        var convention = ConventionManager.GetElementConvention(typeof(Ex020_RatingControl));
        Assert.Equal(typeof(Ex020_RatingControl), convention.ElementType);
        Assert.Equal("Value", convention.GetBindableProperty(new Ex020_RatingControl())?.Name);
        Assert.Equal("Value", convention.ParameterProperty);
    }

    [WpfFact]
    public void After_Registering_Naming_An_Element_Binds_The_Controls_Value_Property_Not_Visibility_Anymore()
    {
        var subject = new Ex020_CustomElementConvention();
        subject.RegisterRatingControlConvention();
        var (panel, control) = BuildView<Ex020_RatingControl>("Rating");

        subject.Bind(new Ex020_Vm(), panel);

        var valueBinding = BindingOperations.GetBinding(control, Ex020_RatingControl.ValueProperty);
        Assert.NotNull(valueBinding);
        Assert.Equal("Rating", valueBinding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, valueBinding!.Mode);
        Assert.Equal(UpdateSourceTrigger.PropertyChanged, valueBinding!.UpdateSourceTrigger);

        // Not merely joined by the fix - the nonsensical Visibility wiring is gone.
        Assert.Null(BindingOperations.GetBinding(control, FrameworkElement.VisibilityProperty));
    }

    [WpfFact]
    public void Registering_The_RatingControl_Convention_Does_Not_Affect_An_Unrelated_Custom_Control()
    {
        var subject = new Ex020_CustomElementConvention();

        subject.RegisterRatingControlConvention();

        var convention = ConventionManager.GetElementConvention(typeof(Ex020_UnregisteredControl));
        Assert.Equal(typeof(FrameworkElement), convention.ElementType);
        Assert.Equal("Visibility", convention.GetBindableProperty(new Ex020_UnregisteredControl())?.Name);
    }

    [WpfFact]
    public void A_Subclass_Of_The_Registered_Control_Finds_The_Same_Convention_Through_The_Hierarchy_Walk()
    {
        var subject = new Ex020_CustomElementConvention();

        subject.RegisterRatingControlConvention();

        var convention = ConventionManager.GetElementConvention(typeof(Ex020_FancyRatingControl));
        Assert.Equal(typeof(Ex020_RatingControl), convention.ElementType);
    }
}
