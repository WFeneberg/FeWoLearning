// Exercise 020 - Custom Element Convention (beginner).
// Goal:   Teach a custom control how to be a first-class citizen of the convention engine:
//         register an ElementConvention so naming an element after a view-model property
//         binds the property you actually meant, instead of the FrameworkElement fallback's
//         Visibility (ex019).
// Drills: ConventionManager.AddElementConvention<T>(bindableProperty, parameterProperty,
//         eventName) - registering ONE convention, keyed by type, that ViewModelBinder then
//         finds through the exact same type-hierarchy walk ex019 measured.
// Passes: dotnet test --filter FullyQualifiedName~Ex020_
//
// AddElementConvention writes into a PRIVATE static dictionary with no public removal, so
// this registration outlives this test and leaks into the rest of the process - by design,
// not an oversight (see caliburn/README.md's forward-risk register). It is inert: it is
// keyed on Ex020_RatingControl, a type this exercise itself declares below, and no other
// exercise's test ever asks ConventionManager about that type, so the leak can never affect
// anything else.
//
// Measured on this machine (Caliburn.Micro 5.0.258), before any registration: naming an
// Ex020_RatingControl "Rating" and binding it to a view model with a settable int Rating
// property produces a real (if nonsensical) TwoWay Binding of Rating onto the control's
// Visibility property - the FrameworkElement fallback ex019 measured, applied blindly by
// name. After ConventionManager.AddElementConvention<Ex020_RatingControl>(
// Ex020_RatingControl.ValueProperty, "Value", null) the SAME naming produces a TwoWay,
// PropertyChanged Binding of Rating onto the control's own Value property instead - and the
// Visibility binding is gone, not merely joined by a second one.

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex020_CustomElementConvention
{
    /// <summary>Registers the convention that lets ViewModelBinder bind Ex020_RatingControl's
    /// Value property by name, instead of falling back to Visibility.</summary>
    public void RegisterRatingControlConvention() =>
        throw new NotImplementedException(
            "TODO: Ex020 - ConventionManager.AddElementConvention<Ex020_RatingControl>(Ex020_RatingControl.ValueProperty, \"Value\", null)");

    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) =>
        throw new NotImplementedException("TODO: Ex020 - ViewModelBinder.Bind(viewModel, view, null)");
}

/// <summary>A small custom control with nothing Caliburn knows about out of the box - falls
/// back to the FrameworkElement/Visibility convention (ex019) until this exercise registers
/// one for it.</summary>
public class Ex020_RatingControl : Control
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(int), typeof(Ex020_RatingControl),
        new FrameworkPropertyMetadata(0) { BindsTwoWayByDefault = true });

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}

/// <summary>A second custom control, structurally identical, that this exercise never
/// registers a convention for - the contrast case proving the fix is scoped to the ONE type
/// it targets, not a blanket change to every custom control.</summary>
public class Ex020_UnregisteredControl : Control
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(int), typeof(Ex020_UnregisteredControl),
        new FrameworkPropertyMetadata(0) { BindsTwoWayByDefault = true });

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}

/// <summary>A subclass of the registered control - proves the registration is found through
/// the same type-hierarchy walk ex019 measured, not only for the exact registered type.</summary>
public class Ex020_FancyRatingControl : Ex020_RatingControl;

/// <summary>A view model with one settable int property, named to match the control above.</summary>
public class Ex020_Vm : PropertyChangedBase
{
    int _rating = 3;
    public int Rating { get => _rating; set => Set(ref _rating, value); }
}
