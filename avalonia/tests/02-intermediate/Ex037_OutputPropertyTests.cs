using System.ComponentModel;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex037_OutputPropertyTests
{
    [Fact]
    public void Initial_Fahrenheit_Matches_The_Default_Celsius()
    {
        var vm = new Ex037_OutputPropertyViewModel();

        Assert.Equal(32.0, vm.Fahrenheit, 3);
    }

    [Fact]
    public void Setting_Celsius_Recomputes_Fahrenheit()
    {
        var vm = new Ex037_OutputPropertyViewModel();

        vm.Celsius = 100;
        Assert.Equal(212.0, vm.Fahrenheit, 3);

        vm.Celsius = 37;
        Assert.Equal(98.6, vm.Fahrenheit, 3);
    }

    // The discriminator: a plain computed getter (public double Fahrenheit =>
    // Celsius * 9 / 5 + 32;) reproduces every value read above but is not itself
    // an ObservableAsPropertyHelper, so it never raises PropertyChanged for
    // Fahrenheit - only for Celsius.
    [Fact]
    public void Setting_Celsius_Raises_PropertyChanged_For_Fahrenheit_Itself()
    {
        var vm = new Ex037_OutputPropertyViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.Celsius = 20;

        Assert.Contains(nameof(Ex037_OutputPropertyViewModel.Fahrenheit), raised);
    }

    [Fact]
    public void A_Second_Distinct_Celsius_Value_Also_Raises_Fahrenheit()
    {
        var vm = new Ex037_OutputPropertyViewModel();
        vm.Celsius = 20;
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.Celsius = -10;

        Assert.Equal(14.0, vm.Fahrenheit, 3);
        Assert.Contains(nameof(Ex037_OutputPropertyViewModel.Fahrenheit), raised);
    }

    [Fact]
    public void Assigning_The_Same_Celsius_Again_Raises_Nothing_For_Fahrenheit()
    {
        var vm = new Ex037_OutputPropertyViewModel();
        vm.Celsius = 15;
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.Celsius = 15;

        Assert.DoesNotContain(nameof(Ex037_OutputPropertyViewModel.Fahrenheit), raised);
    }
}
