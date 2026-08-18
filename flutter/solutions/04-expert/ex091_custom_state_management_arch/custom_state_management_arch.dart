// Exercise 091 - hand-rolled Redux-like state container (reference solution).

typedef Reducer<S, A> = S Function(S state, A action);
typedef Listener<S> = void Function(S state);

class Store<S, A> {
  Store(this._reducer, S initialState) : _state = initialState;

  final Reducer<S, A> _reducer;
  S _state;
  final List<Listener<S>> _listeners = [];

  S get state => _state;

  void dispatch(A action) {
    _state = _reducer(_state, action);
    for (final listener in List<Listener<S>>.of(_listeners)) {
      listener(_state);
    }
  }

  void Function() subscribe(Listener<S> listener) {
    _listeners.add(listener);
    return () => _listeners.remove(listener);
  }
}
