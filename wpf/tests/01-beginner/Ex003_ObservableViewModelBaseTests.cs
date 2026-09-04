using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex003_ObservableViewModelBaseTests : WpfTestContext
{
    private static List<string?> Record(Ex003_MeterViewModel model)
    {
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    // SetProperty is protected: this is the only way a test can reach it directly instead
    // of through Ex003_MeterViewModel, which is "ready to use" and therefore editable by
    // the learner. Without this probe, a learner could inline compare/assign/RaisePropertyChanged
    // straight into Reading's and Label's setters, leave SetProperty throwing, and still pass
    // every test above.
    private sealed class SetPropertyProbe : Ex003_ObservableViewModelBase
    {
        private int _amount;

        public int Amount => _amount;

        /// <summary>Calls SetProperty directly, bypassing any view model.</summary>
        public bool SetAmount(int value) => SetProperty(ref _amount, value);
    }

    private static List<string?> Record(SetPropertyProbe probe)
    {
        var names = new List<string?>();
        probe.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [WpfFact]
    public void SetProperty_Reports_True_And_Raises_The_Event_On_A_Real_Change()
    {
        var probe = new SetPropertyProbe();
        var names = Record(probe);

        var changed = probe.SetAmount(7);

        Assert.True(changed, "SetProperty must return true when the value actually changed.");
        Assert.Equal(7, probe.Amount);
        Assert.Equal(new string?[] { nameof(SetPropertyProbe.SetAmount) }, names);
    }

    [WpfFact]
    public void SetProperty_Reports_False_And_Raises_Nothing_On_An_Equal_Value()
    {
        var probe = new SetPropertyProbe();
        probe.SetAmount(7);
        var names = Record(probe);

        var changed = probe.SetAmount(7);

        Assert.False(changed, "SetProperty must return false when the value did not change.");
        Assert.Equal(7, probe.Amount);
        Assert.Empty(names);
    }

    [WpfFact]
    public void Assigning_A_New_Value_Stores_It()
    {
        var model = new Ex003_MeterViewModel();

        model.Reading = 12.5;

        Assert.Equal(12.5, model.Reading);
    }

    [WpfFact]
    public void Assigning_A_New_Value_Raises_Property_Changed()
    {
        var model = new Ex003_MeterViewModel();
        var names = Record(model);

        model.Reading = 12.5;

        Assert.Equal(new string?[] { "Reading" }, names);
    }

    [WpfFact]
    public void The_Property_Name_Comes_From_CallerMemberName()
    {
        var model = new Ex003_MeterViewModel();
        var names = Record(model);

        model.Label = "inlet";

        // Not "value", not null, not string.Empty: the compiler substitutes the calling
        // property's name, which is why the setter passes no name at all.
        Assert.Equal(new string?[] { "Label" }, names);
    }

    [WpfFact]
    public void Assigning_An_Equal_Value_Raises_Nothing()
    {
        var model = new Ex003_MeterViewModel { Reading = 12.5 };
        var names = Record(model);

        model.Reading = 12.5;

        Assert.Empty(names);
    }

    [WpfFact]
    public void Equal_Reference_Values_Are_Compared_By_Value_Not_By_Reference()
    {
        var model = new Ex003_MeterViewModel { Label = "inlet" };
        var names = Record(model);

        // A fresh string with the same content. Reference equality would call this a
        // change; EqualityComparer<string>.Default does not.
        model.Label = new string("inlet".ToCharArray());

        Assert.Empty(names);
    }

    [WpfFact]
    public void Null_Is_A_Legal_Value_On_Both_Sides()
    {
        var model = new Ex003_MeterViewModel { Label = "inlet" };
        var names = Record(model);

        model.Label = null!;
        Assert.Null(model.Label);
        Assert.Equal(new string?[] { "Label" }, names);

        names.Clear();
        model.Label = null!;
        Assert.Empty(names);
    }
}
