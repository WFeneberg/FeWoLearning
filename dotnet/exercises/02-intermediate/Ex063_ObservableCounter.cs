namespace FeWoLearning.Exercises.Intermediate;

// Exercise 063 — Observable Counter (intermediate).
// Goal:   Implement an IObservable<int> counter. Each call to Increment()
//         pushes the new counter value to every currently subscribed
//         IObserver<int>. Subscribe(...) must return an IDisposable that,
//         when disposed, unsubscribes the observer (no further OnNext calls
//         after that). Complete() must call OnCompleted() on all observers
//         and stop delivering any further notifications.
// Drills: IObservable<T>/IObserver<T>, the subscription/unsubscription
//         pattern, IDisposable, push-based notification.
public class ObservableCounter : IObservable<int>
{
    public int Value { get; private set; }

    public IDisposable Subscribe(IObserver<int> observer) => throw new NotImplementedException();

    public void Increment() => throw new NotImplementedException();

    public void Complete() => throw new NotImplementedException();
}
