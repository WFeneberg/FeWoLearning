using System.Collections.Specialized;
using System.ComponentModel;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex006_BindableCollectionRangeTests : CaliburnCoreContext
{
    private static List<NotifyCollectionChangedAction> RecordCollectionChanges(Ex006_BindableCollectionRange vm)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        vm.Items.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }

    [Fact]
    public void AddRange_Of_Many_Items_Raises_Exactly_One_Reset()
    {
        var vm = new Ex006_BindableCollectionRange();
        var actions = RecordCollectionChanges(vm);

        vm.AddRange(new[] { "a", "b", "c" });

        Assert.Equal(new[] { "a", "b", "c" }, vm.Items);
        // Not one Add per item - a naive foreach-Add loop would be three events here.
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }

    [Fact]
    public void RemoveRange_Of_Many_Items_Raises_Exactly_One_Reset()
    {
        var vm = new Ex006_BindableCollectionRange();
        vm.AddRange(new[] { "a", "b", "c", "d" });
        var actions = RecordCollectionChanges(vm);

        vm.RemoveRange(new[] { "b", "d" });

        Assert.Equal(new[] { "a", "c" }, vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }

    [Fact]
    public void RemoveRange_With_Items_Not_Present_Still_Raises_A_Reset()
    {
        var vm = new Ex006_BindableCollectionRange();
        vm.AddRange(new[] { "a", "b" });
        var actions = RecordCollectionChanges(vm);

        // Nothing here is actually in the collection - a bound view still re-reads it.
        vm.RemoveRange(new[] { "x", "y" });

        Assert.Equal(new[] { "a", "b" }, vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }

    [Fact]
    public void AddRangeIfAny_With_An_Empty_Sequence_Stays_Completely_Silent()
    {
        var vm = new Ex006_BindableCollectionRange();
        vm.AddRange(new[] { "a" });
        var collectionActions = RecordCollectionChanges(vm);
        var propertyNames = new List<string?>();
        ((INotifyPropertyChanged)vm.Items).PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        vm.AddRangeIfAny(Array.Empty<string>());

        // Not "one Reset instead of many" - genuinely nothing, because there was nothing
        // to do. A guard that only ever calls Items.AddRange would still Reset here.
        Assert.Empty(collectionActions);
        Assert.Empty(propertyNames);
        Assert.Equal(new[] { "a" }, vm.Items);
    }

    [Fact]
    public void AddRangeIfAny_With_Items_Still_Adds_Them_In_One_Reset()
    {
        var vm = new Ex006_BindableCollectionRange();
        var actions = RecordCollectionChanges(vm);

        vm.AddRangeIfAny(new[] { "a", "b" });

        Assert.Equal(new[] { "a", "b" }, vm.Items);
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
    }
}
