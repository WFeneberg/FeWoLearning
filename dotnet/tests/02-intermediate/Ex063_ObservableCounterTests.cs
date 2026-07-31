using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex063_ObservableCounterTests
{
    private sealed class RecordingObserver : IObserver<int>
    {
        public List<int> Values { get; } = new();
        public bool Completed { get; private set; }

        public void OnNext(int value) => Values.Add(value);
        public void OnError(Exception error) => throw error;
        public void OnCompleted() => Completed = true;
    }

    [Fact]
    public void Subscribe_ReceivesFullSequenceOfEmittedValues()
    {
        var counter = new ObservableCounter();
        var observer = new RecordingObserver();

        using var subscription = counter.Subscribe(observer);

        counter.Increment();
        counter.Increment();
        counter.Increment();

        Assert.Equal(new List<int> { 1, 2, 3 }, observer.Values);
    }

    [Fact]
    public void Increment_UpdatesPublicValue()
    {
        var counter = new ObservableCounter();

        counter.Increment();
        counter.Increment();

        Assert.Equal(2, counter.Value);
    }

    [Fact]
    public void MultipleObservers_AllReceiveTheSameNotifications()
    {
        var counter = new ObservableCounter();
        var first = new RecordingObserver();
        var second = new RecordingObserver();

        using var subscription1 = counter.Subscribe(first);
        using var subscription2 = counter.Subscribe(second);

        counter.Increment();
        counter.Increment();

        Assert.Equal(new List<int> { 1, 2 }, first.Values);
        Assert.Equal(new List<int> { 1, 2 }, second.Values);
    }

    [Fact]
    public void DisposingSubscription_StopsFurtherNotifications()
    {
        var counter = new ObservableCounter();
        var observer = new RecordingObserver();
        var subscription = counter.Subscribe(observer);

        counter.Increment();
        subscription.Dispose();
        counter.Increment();
        counter.Increment();

        Assert.Equal(new List<int> { 1 }, observer.Values);
    }

    [Fact]
    public void Complete_NotifiesObserversAndStopsEmitting()
    {
        var counter = new ObservableCounter();
        var observer = new RecordingObserver();
        using var subscription = counter.Subscribe(observer);

        counter.Increment();
        counter.Complete();
        counter.Increment();

        Assert.Equal(new List<int> { 1 }, observer.Values);
        Assert.True(observer.Completed);
    }
}
