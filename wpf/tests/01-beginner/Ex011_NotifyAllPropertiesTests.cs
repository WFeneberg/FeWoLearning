using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex011_NotifyAllPropertiesTests : WpfTestContext
{
    // There is no separate "ready to use" wrapper standing between these tests and the
    // subject: LoadFrom is the only place the all-properties-changed signal can come
    // from, so every test below already exercises it directly.

    [WpfFact]
    public void LoadFrom_Assigns_Both_Fields()
    {
        var model = new Ex011_ProfileViewModel();

        model.LoadFrom("Ada", 100);

        Assert.Equal("Ada", model.Name);
        Assert.Equal(100, model.Score);
    }

    [WpfFact]
    public void LoadFrom_Raises_Exactly_One_Event_With_An_Empty_Or_Null_Property_Name()
    {
        var model = new Ex011_ProfileViewModel();
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        model.LoadFrom("Ada", 100);

        // Exactly one event: a learner who instead raised Name and Score separately
        // (defeating the whole point of the empty-name signal) would show up here as
        // two events, not one. WPF treats null the same as string.Empty, so either is
        // accepted - the literal text is not the contract, the "refresh everything"
        // behavior below is.
        var propertyName = Assert.Single(names);
        Assert.True(
            propertyName is null or "",
            $"Expected PropertyChangedEventArgs.PropertyName to be null or \"\" (both mean " +
            $"\"everything changed\" to WPF), but it was \"{propertyName}\".");
    }

    [WpfFact]
    public void A_Live_Binding_To_Name_And_A_Live_Binding_To_Score_Both_Refresh_From_One_Call()
    {
        var model = new Ex011_ProfileViewModel();
        model.LoadFrom("Ada", 1);

        var nameTarget = new TextBlock { DataContext = model };
        nameTarget.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex011_ProfileViewModel.Name)));

        var scoreTarget = new TextBlock { DataContext = model };
        scoreTarget.SetBinding(TextBlock.TextProperty, new Binding(nameof(Ex011_ProfileViewModel.Score)));

        Layout(nameTarget);
        Layout(scoreTarget);
        Pump();

        Assert.Equal("Ada", nameTarget.Text);
        Assert.Equal("1", scoreTarget.Text);

        model.LoadFrom("Grace", 42);
        Pump();

        // Neither binding's own property was named individually by LoadFrom - only a
        // real "everything changed" signal refreshes both targets from a single call.
        // A learner who hard-coded RaisePropertyChanged(nameof(Name)) only (or any one
        // specific name) would leave the other target stale here.
        Assert.Equal("Grace", nameTarget.Text);
        Assert.Equal("42", scoreTarget.Text);
    }
}
