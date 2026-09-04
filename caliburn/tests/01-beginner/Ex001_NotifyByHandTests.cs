using System.ComponentModel;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex001_NotifyByHandTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex001_NotifyByHand vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Setting_A_Property_Stores_The_Value()
    {
        var vm = new Ex001_NotifyByHand { FirstName = "Ada", LastName = "Lovelace" };

        Assert.Equal("Ada", vm.FirstName);
        Assert.Equal("Lovelace", vm.LastName);
        Assert.Equal("Ada Lovelace", vm.FullName);
    }

    [Fact]
    public void Setting_A_Property_Announces_It_By_Name()
    {
        // Asserted here rather than in a [Fact] of its own: a standalone structural
        // assertion would be green against the untouched stub, which the track forbids.
        Assert.False(
            typeof(Ex001_NotifyByHand).IsSubclassOf(typeof(PropertyChangedBase)),
            "ex001 is the hand-written version on purpose - ex002 is the PropertyChangedBase one.");

        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";

        Assert.Contains(nameof(Ex001_NotifyByHand.FirstName), names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex001_NotifyByHand { FirstName = "Ada", LastName = "Lovelace" };
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // Every redundant notification re-evaluates each binding on the property and
        // re-runs its converters, for nothing.
        Assert.Empty(names);
    }

    [Fact]
    public void Computed_FullName_Is_Announced_When_Its_Inputs_Move()
    {
        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        Assert.Equal(2, names.Count(n => n == nameof(Ex001_NotifyByHand.FullName)));
    }

    [Fact]
    public void Announced_Names_Exist_On_The_Type()
    {
        var vm = new Ex001_NotifyByHand();
        var names = Record(vm);

        vm.FirstName = "Ada";
        vm.LastName = "Lovelace";

        // A typo in a hand-written string literal fails silently at runtime; the compiler
        // never sees it. [CallerMemberName] is how you stop writing them.
        Assert.NotEmpty(names);
        Assert.All(names, name =>
        {
            Assert.False(string.IsNullOrEmpty(name));
            Assert.NotNull(typeof(Ex001_NotifyByHand).GetProperty(name!));
        });
    }
}
