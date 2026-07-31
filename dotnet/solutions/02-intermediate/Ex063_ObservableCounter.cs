namespace FeWoLearning.Exercises.Intermediate;

// Exercise 063 — Observable Counter (reference solution).
public class ObservableCounter : IObservable<int>
{
    private readonly List<IObserver<int>> _observers = new();
    private bool _completed;

    public int Value { get; private set; }

    public IDisposable Subscribe(IObserver<int> observer)
    {
        if (_completed)
        {
            observer.OnCompleted();
            return new Unsubscriber(_observers, observer);
        }

        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void Increment()
    {
        if (_completed)
        {
            return;
        }

        Value++;

        // Snapshot so an observer that unsubscribes during OnNext doesn't
        // mutate the collection we're iterating over.
        foreach (var observer in _observers.ToArray())
        {
            observer.OnNext(Value);
        }
    }

    public void Complete()
    {
        if (_completed)
        {
            return;
        }

        _completed = true;

        foreach (var observer in _observers.ToArray())
        {
            observer.OnCompleted();
        }

        _observers.Clear();
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<int>> _observers;
        private readonly IObserver<int> _observer;
        private bool _disposed;

        public Unsubscriber(List<IObserver<int>> observers, IObserver<int> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _observers.Remove(_observer);
        }
    }
}
