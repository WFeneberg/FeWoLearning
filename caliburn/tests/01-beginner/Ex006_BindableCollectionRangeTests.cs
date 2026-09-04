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
        var propertyNames = new List<string?>();
        ((INotifyPropertyChanged)vm.Items).PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

        vm.AddRange(new[] { "a", "b", "c" });

        Assert.Equal(new[] { "a", "b", "c" }, vm.Items);
        // Not one Add per item - a naive foreach-Add loop would be three events here.
        Assert.Equal(new[] { NotifyCollectionChangedAction.Reset }, actions);
        // Alongside the Reset, Count is announced too - the silent case is covered by
        // AddRangeIfAny's empty-sequence test below.
        Assert.Contains("Count", propertyNames);
    }

    [Fact]
    public void AddRange_With_An_Empty_Sequence_Still_Raises_A_Reset()
    {
        var vm = new Ex006_BindableCollectionRange();
        var actions = RecordCollectionChanges(vm);

        // Nothing to add - but plain AddRange must not special-case this away itself.
        // That guard is AddRangeIfAny's job, not AddRange's: a learner who makes the two
        // identical (skip the call whenever the sequence is empty) has broken the very
        // distinction this exercise exists to draw.
        vm.AddRange(Array.Empty<string>());

        Assert.Empty(vm.Items);
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

    /// <summary>An IEnumerable that counts how many times it has actually been walked.</summary>
    sealed class CountingSequence(params string[] items) : IEnumerable<string>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<string> GetEnumerator()
        {
            EnumerationCount++;
            return ((IEnumerable<string>)items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public void AddRangeIfAny_Enumerates_Its_Source_Exactly_Once()
    {
        var vm = new Ex006_BindableCollectionRange();
        var sequence = new CountingSequence("a", "b");

        vm.AddRangeIfAny(sequence);

        Assert.Equal(new[] { "a", "b" }, vm.Items);
        // A guard that checks "is it empty?" and then hands the same lazy sequence to
        // AddRange would walk it twice. The check must consume it once, not peek and redo.
        Assert.Equal(1, sequence.EnumerationCount);
    }
}
