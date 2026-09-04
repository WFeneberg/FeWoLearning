using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex002_PropertyChangedBaseBasicsTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex002_PropertyChangedBaseBasics vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Uses_The_Caliburn_Base_Class()
    {
        // Folded together with a behavioural assertion: on its own, the base-type check
        // would be green against the untouched stub.
        Assert.True(typeof(Ex002_PropertyChangedBaseBasics).IsSubclassOf(typeof(PropertyChangedBase)));

        var vm = new Ex002_PropertyChangedBaseBasics { FirstName = "Ada" };

        Assert.Equal("Ada", vm.FirstName);
    }

    [Fact]
    public void Setting_A_Property_Announces_It_By_Name()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.FirstName = "Ada";

        Assert.Contains(nameof(Ex002_PropertyChangedBaseBasics.FirstName), names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex002_PropertyChangedBaseBasics { FirstName = "Ada", LastName = "Lovelace" };
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // Set already does this comparison for you - that is half of what it is for.
        Assert.Empty(names);
    }

    [Fact]
    public void Computed_FullName_Is_Announced_When_Its_Inputs_Move()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        Assert.Equal(2, names.Count(n => n == nameof(Ex002_PropertyChangedBaseBasics.FullName)));
        Assert.Equal("Ada Lovelace", vm.FullName);
    }

    [Fact]
    public void RefreshAll_Announces_Everything_In_One_Event()
    {
        var vm = new Ex002_PropertyChangedBaseBasics();
        var names = Record(vm);

        vm.RefreshAll();

        // null or empty is the INotifyPropertyChanged convention for "all of them";
        // Caliburn's Refresh() uses empty. It still raises exactly one such event.
        Assert.Single(names);
        Assert.Equal(string.Empty, names[0]);
    }
}
