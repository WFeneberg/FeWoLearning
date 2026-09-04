using System.ComponentModel;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex008_ObservableViewModelTests
{
    private static (Ex008_ObservableViewModel Vm, List<string?> Raised) Arrange()
    {
        var vm = new Ex008_ObservableViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);
        return (vm, raised);
    }

    [Fact]
    public void Starts_At_Zero_And_Round_Trips_The_Value()
    {
        var (vm, _) = Arrange();

        Assert.Equal(0, vm.Count);

        vm.Count = 5;

        Assert.Equal(5, vm.Count);
    }

    [Fact]
    public void Raises_PropertyChanged_With_The_Property_Name()
    {
        var (vm, raised) = Arrange();

        vm.Count = 5;

        Assert.Equal(new[] { nameof(Ex008_ObservableViewModel.Count) }, raised);
    }

    // The discriminator: a setter that raises unconditionally passes the test above
    // but fails here.
    [Fact]
    public void Assigning_The_Same_Value_Raises_Nothing()
    {
        var (vm, raised) = Arrange();

        vm.Count = 5;
        vm.Count = 5;
        vm.Count = 5;

        Assert.Single(raised);
    }

    [Fact]
    public void Each_Real_Change_Raises_Once()
    {
        var (vm, raised) = Arrange();

        vm.Count = 1;
        vm.Count = 2;
        vm.Count = 2;
        vm.Count = 3;

        Assert.Equal(3, raised.Count);
        Assert.All(raised, name => Assert.Equal("Count", name));
    }
}
