// Exercise 099 - Capstone Control (expert).
// Goal:   One control that does everything this track has taught, and does it correctly.
// Drills: dependency properties with a coerced value, a template part contract, two
//         orthogonal state groups, an automation peer with two patterns, and a mergeable
//         style dictionary - all in one type.
// Passes: dotnet test --filter FullyQualifiedName~Ex099_
//
// A rating control: click a star, the value changes; the value is clamped to the maximum;
// the look comes from a style a consumer merges; a screen reader can read and set it.
//
// Nothing here is new. What is new is that all of it has to hold at once - which is where
// the ordering rules from ex027, ex037, ex078 and ex081 stop being separate lessons: the
// part lookup, the subscription move and the state re-entry all happen in one
// OnApplyTemplate, in a fixed order.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace FeWoLearning.Uno.Exercises.Expert;

public partial class Ex099_RatingControl : Control
{
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(Ex099_RatingControl),
            new PropertyMetadata(5, OnMaximumChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(int),
            typeof(Ex099_RatingControl),
            new PropertyMetadata(0, OnValueChanged));

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(Ex099_RatingControl),
            new PropertyMetadata(false, OnReadOnlyChanged));

    public Ex099_RatingControl() => DefaultStyleKey = typeof(Ex099_RatingControl);

    /// <summary>The highest value this control accepts. At least 1.</summary>
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// The current rating, always between 0 and <see cref="Maximum"/> - a value outside
    /// that range is corrected rather than rejected.
    /// </summary>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Whether the control refuses to change its own value.</summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>Raised whenever <see cref="Value"/> actually changes.</summary>
    public event EventHandler<int>? ValueChanged;

    /// <summary>The part that raises the value, once a template has been applied.</summary>
    public ButtonBase? IncrementPart { get; private set; }

    /// <summary>The state names last requested, availability first.</summary>
    public IReadOnlyList<string> LastRequestedStates => _requested;

    private readonly List<string> _requested = [];

    /// <summary>
    /// Raises the rating by one, unless the control is read-only or already at its maximum.
    /// </summary>
    public void Increment()
    {
        if (IsReadOnly || Value >= Maximum)
        {
            return;
        }

        Value++;
    }

    /// <summary>
    /// Enters both state groups: "Editable"/"ReadOnly" and "Empty"/"Rated", in that order.
    /// </summary>
    public void UpdateVisualState()
    {
        Request(IsReadOnly ? "ReadOnly" : "Editable");
        Request(Value > 0 ? "Rated" : "Empty");
    }

    private void Request(string stateName)
    {
        _requested.Add(stateName);
        VisualStateManager.GoToState(this, stateName, useTransitions: false);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Release before the lookup, or this detaches from the part just found - and the
        // old one keeps reacting from outside the tree.
        if (IncrementPart is not null)
        {
            IncrementPart.Click -= OnIncrementClick;
        }

        IncrementPart = GetTemplateChild("PART_Increment") as ButtonBase;

        if (IncrementPart is not null)
        {
            IncrementPart.Click += OnIncrementClick;
        }

        // Last: the state groups belong to the template and start empty, so a control that
        // was already rated has to say so again.
        UpdateVisualState();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new Ex099_RatingPeer(this);

    private void OnIncrementClick(object sender, RoutedEventArgs args) => Increment();

    private static void OnMaximumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var control = (Ex099_RatingControl)sender;
        var clamped = Math.Clamp(control.Value, 0, Math.Max(1, control.Maximum));

        if (clamped != control.Value)
        {
            // Assigning the *same* value would raise no change and run no callback, so the
            // correction has to be computed here rather than delegated by re-assignment.
            control.Value = clamped;
            return;
        }

        control.UpdateVisualState();
    }

    private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var control = (Ex099_RatingControl)sender;
        var requested = (int)args.NewValue;
        var clamped = Math.Clamp(requested, 0, Math.Max(1, control.Maximum));

        if (clamped != requested)
        {
            // Re-enters this callback with the corrected value, where the clamp is a no-op
            // and the announcement happens once. A loop here would be a stack overflow.
            control.Value = clamped;
            return;
        }

        control.RaiseValueChanged(clamped);
        control.UpdateVisualState();
    }

    private static void OnReadOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex099_RatingControl)sender).UpdateVisualState();

    private void RaiseValueChanged(int value) => ValueChanged?.Invoke(this, value);
}

/// <summary>The peer: reads the rating out and sets it.</summary>
public sealed class Ex099_RatingPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    public Ex099_RatingPeer(Ex099_RatingControl owner)
        : base(owner)
    {
    }

    private Ex099_RatingControl Rating => (Ex099_RatingControl)Owner;

    public bool IsReadOnly => Rating.IsReadOnly;

    public double Maximum => Rating.Maximum;

    public double Minimum => 0;

    public double Value => Rating.Value;

    public double SmallChange => 1;

    public double LargeChange => 1;

    /// <summary>Sets the rating, unless the control is read-only.</summary>
    public void SetValue(double value)
    {
        if (Rating.IsReadOnly)
        {
            // The peer is an alternative path to the same behaviour, not a way around it.
            return;
        }

        Rating.Value = (int)Math.Round(value);
    }

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Slider;

    protected override string GetClassNameCore() => nameof(Ex099_RatingControl);
}
