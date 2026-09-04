using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex003_NotifyOfPropertyChangeTests : CaliburnCoreContext
{
    private static (Ex003_NotifyOfPropertyChange Vm, Dictionary<string, string> Store, List<string?> Names) Make()
    {
        var store = new Dictionary<string, string>();
        var vm = new Ex003_NotifyOfPropertyChange(store);
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return (vm, store, names);
    }

    [Fact]
    public void Setting_Writes_Through_To_The_Store()
    {
        var (vm, store, _) = Make();

        // Nothing in the store yet, so the getter's fallback is what a view would show.
        Assert.Equal("light", vm.Theme);
        Assert.False(vm.IsDark);

        vm.Theme = "dark";

        // The store is the single source of truth - there is no field shadowing it.
        Assert.Equal("dark", store["Theme"]);
        Assert.Equal("dark", vm.Theme);
    }

    [Fact]
    public void Reads_Through_To_The_Store_Rather_Than_A_Field()
    {
        var store = new Dictionary<string, string> { ["Theme"] = "dark" };
        var vm = new Ex003_NotifyOfPropertyChange(store);

        // Constructed over a populated store, the getter already reflects it. Then prove
        // the setter writes back into that same store rather than into a field.
        Assert.Equal("dark", vm.Theme);
        Assert.True(vm.IsDark);

        vm.Theme = "light";

        Assert.Equal("light", store["Theme"]);
    }

    [Fact]
    public void Setting_Announces_Theme_And_IsDark()
    {
        var (vm, _, names) = Make();

        vm.Theme = "dark";

        Assert.Contains(nameof(Ex003_NotifyOfPropertyChange.Theme), names);
        Assert.Contains(nameof(Ex003_NotifyOfPropertyChange.IsDark), names);
        Assert.True(vm.IsDark);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var (vm, _, names) = Make();
        vm.Theme = "dark";
        names.Clear();

        vm.Theme = "dark";

        // No Set(...) to lean on: this comparison is yours to write.
        Assert.Empty(names);
    }

    [Fact]
    public void Writing_The_Default_Over_An_Empty_Store_Announces_Nothing()
    {
        var (vm, _, names) = Make();

        vm.Theme = "light";

        // Theme already reads "light", so nothing moved - even though the store was empty.
        Assert.Empty(names);
    }
}
