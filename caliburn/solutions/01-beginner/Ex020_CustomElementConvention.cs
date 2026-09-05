// Exercise 020 - Custom Element Convention (beginner).
// Goal:   Teach a custom control how to be a first-class citizen of the convention engine:
//         register an ElementConvention so naming an element after a view-model property
//         binds the property you actually meant, instead of the FrameworkElement fallback's
//         Visibility (ex019).
// Drills: ConventionManager.AddElementConvention<T>(bindableProperty, parameterProperty,
//         eventName) - registering ONE convention, keyed by type, that ViewModelBinder then
//         finds through the exact same type-hierarchy walk ex019 measured.
// Passes: dotnet test --filter FullyQualifiedName~Ex020_

using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex020_CustomElementConvention
{
    /// <summary>Registers the convention that lets ViewModelBinder bind Ex020_RatingControl's
    /// Value property by name, instead of falling back to Visibility.</summary>
    // eventName is null here because nothing in this exercise attaches an action convention
    // (a Message.Attach-style command trigger) to Ex020_RatingControl - that argument only
    // feeds ElementConvention.CreateTrigger, which becomes
    // () => new EventTrigger { EventName = null }. A control meant for real action wiring
    // needs a real routed event name there, or actions silently never fire.
    public void RegisterRatingControlConvention() =>
        ConventionManager.AddElementConvention<Ex020_RatingControl>(Ex020_RatingControl.ValueProperty, "Value", null!);

    /// <summary>Applies Caliburn's naming convention to bind every matching named element in the view.</summary>
    public void Bind(object viewModel, FrameworkElement view) => ViewModelBinder.Bind(viewModel, view, null);
}

/// <summary>A small custom control with nothing Caliburn knows about out of the box - falls
/// back to the FrameworkElement/Visibility convention (ex019) until this exercise registers
/// one for it.</summary>
public class Ex020_RatingControl : Control
{
    // BindsTwoWayByDefault is right for a real, hand-authored-XAML DP - kept for that reason -
    // but it is NOT why this exercise's tests observe TwoWay: ex018 measures that
    // ConventionManager.ApplyBindingMode looks only at whether the VIEW-MODEL property
    // (Rating) has a public setter, never at this flag or the element at all. Set this to
    // false and the binding this exercise's tests assert is still TwoWay.
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
