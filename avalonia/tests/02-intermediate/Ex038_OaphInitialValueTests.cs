using System.Reflection;
using ReactiveUI;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex038_OaphInitialValueTests
{
    // A minimal IObservable<int> that records how many times it has been
    // subscribed and lets the test control emission by hand. This track has no
    // System.Reactive reference (see README/design doc), so this stands in for
    // System.Reactive.Subjects.Subject<T>.
    private sealed class CountingSource : IObservable<int>
    {
        private readonly List<IObserver<int>> _observers = [];
        public int SubscribeCount { get; private set; }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            SubscribeCount++;
            _observers.Add(observer);
            return new Unsubscriber(() => _observers.Remove(observer));
        }

        public void Emit(int value)
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnNext(value);
            }
        }

        private sealed class Unsubscriber(Action dispose) : IDisposable
        {
            public void Dispose() => dispose();
        }
    }

    [Fact]
    public void Constructing_The_View_Model_Does_Not_Subscribe_To_The_Source()
    {
        var source = new CountingSource();

        _ = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        Assert.Equal(0, source.SubscribeCount);
    }

    [Fact]
    public void Reading_Value_The_First_Time_Triggers_The_Deferred_Subscription_And_Returns_The_Initial_Value()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        var first = vm.Value;

        Assert.Equal(7, first);
        Assert.Equal(1, source.SubscribeCount);
    }

    [Fact]
    public void Reading_Value_Again_Does_Not_Resubscribe()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        _ = vm.Value;
        _ = vm.Value;
        _ = vm.Value;

        Assert.Equal(1, source.SubscribeCount);
    }

    [Fact]
    public void Emitting_After_The_Deferred_Subscription_Updates_Value()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        _ = vm.Value; // triggers the deferred subscription

        source.Emit(99);

        Assert.Equal(99, vm.Value);
    }

    // The discriminator against deferSubscription: false (the ToProperty
    // default): if the source were subscribed eagerly at construction, this
    // emission - which happens BEFORE any read - would already have replaced the
    // initial value by the time anyone reads Value.
    [Fact]
    public void An_Emission_Before_The_First_Read_Is_Missed_Because_Subscription_Is_Deferred()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        source.Emit(123); // nobody is subscribed yet - this goes nowhere

        Assert.Equal(7, vm.Value);
    }

    // Guards against a hard-coded initial value: a different constructor
    // argument must come back unchanged too.
    [Fact]
    public void A_Different_Initial_Value_Is_Forwarded_Exactly()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: -42);

        Assert.Equal(-42, vm.Value);
    }

    // Structural check: a lazy `bool _subscribed` flag guarding a manual
    // source.Subscribe(...) on first read, seeded from the initial value, can
    // reproduce every assertion above (subscribe-count gating, the missed
    // pre-read emission, the forwarded initial value) without ever
    // constructing an ObservableAsPropertyHelper<T>. Reflect by FIELD TYPE, not
    // by name, so a learner who renames or restructures the field is still
    // free to pass, as long as a real ObservableAsPropertyHelper<int> backs
    // Value.
    [Fact]
    public void Value_Is_Backed_By_A_Real_ObservableAsPropertyHelper()
    {
        var source = new CountingSource();
        var vm = new Ex038_OaphInitialValueViewModel(source, initialValue: 7);

        var hasOaph = vm.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(f => f.FieldType == typeof(ObservableAsPropertyHelper<int>) && f.GetValue(vm) is not null);

        Assert.True(hasOaph,
            "Value must be backed by a real ObservableAsPropertyHelper<int> field - a hand-rolled " +
            "bool flag plus a manual Subscribe call is not the mechanism (ToProperty / " +
            "ObservableAsPropertyHelper) this exercise teaches.");
    }
}
