// Exercise 091 - hand-rolled Redux-like state container (expert).
//
// Goal:   Implement a minimal Redux-style Store<S, A> that holds a single
//         state value, updates it through a pure reducer on dispatch(), and
//         notifies subscribed listeners after every change.
// Drills: generics, typedefs, listener management, unsubscribe callbacks.
// Passes: when dispatch() runs the reducer, updates state, and calls every
//         subscribed listener exactly once with the new state; unsubscribe
//         stops further notifications.

typedef Reducer<S, A> = S Function(S state, A action);
typedef Listener<S> = void Function(S state);

class Store<S, A> {
  Store(this._reducer, S initialState) : _state = initialState;

  final Reducer<S, A> _reducer;
  S _state;
  final List<Listener<S>> _listeners = [];

  S get state => _state;

  void dispatch(A action) {
    throw UnimplementedError('TODO');
  }

  void Function() subscribe(Listener<S> listener) {
    throw UnimplementedError('TODO');
  }
}
