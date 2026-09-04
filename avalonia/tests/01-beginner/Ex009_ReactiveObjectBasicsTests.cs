using System.ComponentModel;
using FeWoLearning.Avalonia.Exercises.Beginner;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex009_ReactiveObjectBasicsTests
{
    [Fact]
    public void Round_Trips_The_Value_And_Raises_Change_Only()
    {
        var vm = new Ex009_ReactiveObjectBasics();
        var changed = new List<string?>();
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Count = 5;
        vm.Count = 5;

        Assert.Equal(5, vm.Count);
        Assert.Equal(new[] { nameof(Ex009_ReactiveObjectBasics.Count) }, changed);
    }

    // The discriminator against re-hand-rolling Ex008 inside a ReactiveObject:
    // PropertyChanging must fire, and must fire while the old value is still in
    // place. Only RaiseAndSetIfChanged (or an explicit RaisePropertyChanging before
    // the assignment) satisfies both.
    [Fact]
    public void Raises_PropertyChanging_Before_The_Value_Is_Updated()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 1 };
        var valueSeenWhileChanging = new List<int>();
        var changingNames = new List<string?>();

        ((INotifyPropertyChanging)vm).PropertyChanging += (_, e) =>
        {
            changingNames.Add(e.PropertyName);
            valueSeenWhileChanging.Add(vm.Count);
        };

        vm.Count = 2;

        Assert.Equal(new[] { nameof(Ex009_ReactiveObjectBasics.Count) }, changingNames);
        Assert.Equal(new[] { 1 }, valueSeenWhileChanging);
        Assert.Equal(2, vm.Count);
    }

    [Fact]
    public void Assigning_The_Same_Value_Raises_Neither_Event()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 7 };
        var events = 0;
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, _) => events++;
        ((INotifyPropertyChanging)vm).PropertyChanging += (_, _) => events++;

        vm.Count = 7;

        Assert.Equal(0, events);
    }

    // Proves the property participates in ReactiveUI's observable pipeline, which is
    // what the rest of the track builds on.
    [Fact]
    public void Property_Is_Observable_Through_WhenAnyValue()
    {
        var vm = new Ex009_ReactiveObjectBasics { Count = 1 };
        var seen = new List<int>();
        using var sub = vm.WhenAnyValue(x => x.Count).Subscribe(seen.Add);

        vm.Count = 2;
        vm.Count = 3;

        Assert.Equal(new[] { 1, 2, 3 }, seen);
    }
}
