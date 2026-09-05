// Exercise 027 - DataTrigger and MultiDataTrigger (beginner).
// Goal:   Switch a Style Setter on and off based on bound data rather than a control's own
//         visual state - Style.Triggers holds DataTrigger (one bound condition) and
//         MultiDataTrigger (every condition must hold) alongside the plain Setters from
//         row 022.
// Drills: Style.Triggers, DataTrigger (Binding + Value, with its own Setters that apply only
//         while the binding equals Value), MultiDataTrigger (a Conditions collection - every
//         one must match before its Setters apply), and DependencyPropertyHelper.
//         GetValueSource reporting BaseValueSource.StyleTrigger while a trigger's Setter is
//         active - measured as distinct from BaseValueSource.Style for a plain, untriggered
//         Setter with the same effective value.
// Passes: dotnet test --filter FullyQualifiedName~Ex027_

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FeWoLearning.Wpf.Exercises.Beginner;

/// <summary>
/// Ready to use - not the subject of this row. A plain view model DataTrigger/
/// MultiDataTrigger bind against.
/// </summary>
public class Ex027_TaskItem : INotifyPropertyChanged
{
    private bool _isUrgent;
    private bool _isAssigned;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsUrgent
    {
        get => _isUrgent;
        set
        {
            if (_isUrgent == value) return;
            _isUrgent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUrgent)));
        }
    }

    public bool IsAssigned
    {
        get => _isAssigned;
        set
        {
            if (_isAssigned == value) return;
            _isAssigned = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAssigned)));
        }
    }
}

public static class Ex027_DataTrigger
{
    /// <summary>
    /// Builds a Style targeting typeof(Button): a base Setter for Width = <paramref name="baseWidth"/>,
    /// plus a DataTrigger that sets Width = <paramref name="triggerWidth"/> while the bound
    /// Ex027_TaskItem.IsUrgent equals true.
    /// </summary>
    public static Style BuildStyleWithDataTrigger(double baseWidth, double triggerWidth)
        // TODO: var style = new Style(typeof(Button));
        //       style.Setters.Add(new Setter(Button.WidthProperty, baseWidth));
        //       var trigger = new DataTrigger { Binding = new Binding(nameof(Ex027_TaskItem.IsUrgent)), Value = true };
        //       trigger.Setters.Add(new Setter(Button.WidthProperty, triggerWidth));
        //       style.Triggers.Add(trigger);
        //       return style;
        => throw new NotImplementedException("TODO: Ex027 - build a Style with a base Setter (Button.WidthProperty = baseWidth) and a DataTrigger (Binding = new Binding(nameof(Ex027_TaskItem.IsUrgent)), Value = true) whose own Setter sets Button.WidthProperty = triggerWidth; add the trigger to style.Triggers");

    /// <summary>
    /// Builds a Style targeting typeof(Button): a base Setter for Width = <paramref name="baseWidth"/>,
    /// plus a MultiDataTrigger that sets Width = <paramref name="triggerWidth"/> only while
    /// BOTH Ex027_TaskItem.IsUrgent and Ex027_TaskItem.IsAssigned equal true.
    /// </summary>
    public static Style BuildStyleWithMultiDataTrigger(double baseWidth, double triggerWidth)
        // TODO: var style = new Style(typeof(Button));
        //       style.Setters.Add(new Setter(Button.WidthProperty, baseWidth));
        //       var trigger = new MultiDataTrigger();
        //       trigger.Conditions.Add(new Condition(new Binding(nameof(Ex027_TaskItem.IsUrgent)), true));
        //       trigger.Conditions.Add(new Condition(new Binding(nameof(Ex027_TaskItem.IsAssigned)), true));
        //       trigger.Setters.Add(new Setter(Button.WidthProperty, triggerWidth));
        //       style.Triggers.Add(trigger);
        //       return style;
        => throw new NotImplementedException("TODO: Ex027 - build a Style with a base Setter (Button.WidthProperty = baseWidth) and a MultiDataTrigger with two Conditions (Ex027_TaskItem.IsUrgent == true, Ex027_TaskItem.IsAssigned == true) whose own Setter sets Button.WidthProperty = triggerWidth only when both hold; add it to style.Triggers");
}
