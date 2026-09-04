using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex004_DependentPropertiesTests : CaliburnCoreContext
{
    private static List<string?> Record(Ex004_DependentProperties vm)
    {
        var names = new List<string?>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Computes_The_Whole_Chain()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m, DiscountPercent = 10m };

        Assert.Equal(100m, vm.Subtotal);
        Assert.Equal(10m, vm.Discount);
        Assert.Equal(90m, vm.Total);
    }

    [Fact]
    public void Quantity_Announces_Itself_And_Every_Dependent()
    {
        var vm = new Ex004_DependentProperties { UnitPrice = 25m, DiscountPercent = 10m };
        var names = Record(vm);

        vm.Quantity = 4;

        Assert.Contains(nameof(Ex004_DependentProperties.Quantity), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Subtotal), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);
        Assert.Equal(4, names.Count);
    }

    [Fact]
    public void UnitPrice_Announces_Itself_And_Every_Dependent()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, DiscountPercent = 10m };
        var names = Record(vm);

        vm.UnitPrice = 25m;

        Assert.Contains(nameof(Ex004_DependentProperties.UnitPrice), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Subtotal), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);
        Assert.Equal(4, names.Count);
    }

    [Fact]
    public void DiscountPercent_Does_Not_Announce_Subtotal()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m };
        var names = Record(vm);

        vm.DiscountPercent = 10m;

        Assert.Contains(nameof(Ex004_DependentProperties.DiscountPercent), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Discount), names);
        Assert.Contains(nameof(Ex004_DependentProperties.Total), names);

        // Subtotal is Quantity * UnitPrice. A discount cannot move it, and announcing it
        // anyway would re-evaluate every binding on Subtotal for nothing.
        Assert.DoesNotContain(nameof(Ex004_DependentProperties.Subtotal), names);
        Assert.Equal(3, names.Count);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var vm = new Ex004_DependentProperties { Quantity = 4, UnitPrice = 25m, DiscountPercent = 10m };
        var names = Record(vm);

        vm.Quantity = 4;
        vm.UnitPrice = 25m;
        vm.DiscountPercent = 10m;

        // Set(...) suppresses the property itself; the dependents must be suppressed too,
        // which is why the announcements belong inside the `if (Set(...))`.
        Assert.Empty(names);
    }
}
