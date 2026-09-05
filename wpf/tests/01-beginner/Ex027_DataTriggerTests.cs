using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex027_DataTriggerTests : WpfTestContext
{
    [WpfFact]
    public void BuildStyleWithDataTrigger_Has_The_Base_Setter_And_One_Trigger()
    {
        var style = Ex027_DataTrigger.BuildStyleWithDataTrigger(80.0, 220.0);

        Assert.Equal(typeof(Button), style.TargetType);
        var setters = style.Setters.Cast<Setter>().ToList();
        Assert.Single(setters);
        Assert.Contains(setters, s => s.Property == Button.WidthProperty && Equals(s.Value, 80.0));

        Assert.Single(style.Triggers);
        var trigger = Assert.IsType<DataTrigger>(style.Triggers[0]);
        Assert.Equal(true, trigger.Value);
        Assert.Equal(nameof(Ex027_TaskItem.IsUrgent), ((Binding)trigger.Binding!).Path.Path);
        var triggerSetters = trigger.Setters.Cast<Setter>().ToList();
        Assert.Contains(triggerSetters, s => s.Property == Button.WidthProperty && Equals(s.Value, 220.0));
    }

    [WpfFact]
    public void The_DataTrigger_Setter_Applies_Only_While_The_Bound_Value_Matches_And_Reports_StyleTrigger()
    {
        var item = new Ex027_TaskItem { IsUrgent = false };
        var button = new Button
        {
            Style = Ex027_DataTrigger.BuildStyleWithDataTrigger(baseWidth: 90.0, triggerWidth: 240.0),
            DataContext = item,
        };
        Layout(button);

        Assert.Equal(90.0, button.Width);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);

        item.IsUrgent = true;
        Pump();

        // The distinguishing check: a value the trigger supplied reports a BaseValueSource
        // distinct from a plain style Setter - StyleTrigger, not Style - which is what tells
        // "the trigger fired" apart from "a setter happened to already hold this value".
        Assert.Equal(240.0, button.Width);
        Assert.Equal(BaseValueSource.StyleTrigger, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);

        item.IsUrgent = false;
        Pump();

        Assert.Equal(90.0, button.Width);
        Assert.Equal(BaseValueSource.Style, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);
    }

    [WpfFact]
    public void A_Different_Bound_Item_And_Different_Widths_Behave_The_Same_Way()
    {
        // Different item, different base/trigger widths than the test above - a hard-coded
        // 240.0 (or a hard-coded starting IsUrgent state) cannot satisfy both.
        var item = new Ex027_TaskItem { IsUrgent = true };
        var button = new Button
        {
            Style = Ex027_DataTrigger.BuildStyleWithDataTrigger(baseWidth: 55.0, triggerWidth: 310.0),
            DataContext = item,
        };
        Layout(button);
        Pump();

        Assert.Equal(310.0, button.Width);

        item.IsUrgent = false;
        Pump();
        Assert.Equal(55.0, button.Width);
    }

    [WpfFact]
    public void BuildStyleWithMultiDataTrigger_Has_Two_Conditions()
    {
        var style = Ex027_DataTrigger.BuildStyleWithMultiDataTrigger(70.0, 260.0);

        Assert.Single(style.Triggers);
        var trigger = Assert.IsType<MultiDataTrigger>(style.Triggers[0]);
        Assert.Equal(2, trigger.Conditions.Count);
        Assert.All(trigger.Conditions, c => Assert.Equal(true, c.Value));
        var paths = trigger.Conditions.Select(c => ((Binding)c.Binding!).Path.Path).ToList();
        Assert.Contains(nameof(Ex027_TaskItem.IsUrgent), paths);
        Assert.Contains(nameof(Ex027_TaskItem.IsAssigned), paths);
    }

    [WpfFact]
    public void MultiDataTrigger_Applies_Only_When_Every_Condition_Holds()
    {
        var item = new Ex027_TaskItem { IsUrgent = false, IsAssigned = false };
        var button = new Button
        {
            Style = Ex027_DataTrigger.BuildStyleWithMultiDataTrigger(baseWidth: 65.0, triggerWidth: 275.0),
            DataContext = item,
        };
        Layout(button);
        Pump();
        Assert.Equal(65.0, button.Width);

        item.IsUrgent = true;
        Pump();
        Assert.Equal(65.0, button.Width); // only one of two conditions holds - must not fire yet

        item.IsAssigned = true;
        Pump();
        Assert.Equal(275.0, button.Width); // both hold now
        Assert.Equal(BaseValueSource.StyleTrigger, DependencyPropertyHelper.GetValueSource(button, Button.WidthProperty).BaseValueSource);

        item.IsUrgent = false;
        Pump();
        Assert.Equal(65.0, button.Width); // dropping one condition again turns it back off
    }
}
