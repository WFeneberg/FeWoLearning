import 'package:test/test.dart';

import 'custom_state_management_arch.dart';

sealed class CounterAction {}

class Increment extends CounterAction {}

class Decrement extends CounterAction {}

int counterReducer(int state, CounterAction action) => switch (action) {
      Increment() => state + 1,
      Decrement() => state - 1,
    };

void main() {
  test('dispatch runs the reducer and updates state', () {
    final store = Store<int, CounterAction>(counterReducer, 0);
    store.dispatch(Increment());
    expect(store.state, 1);
  });

  test('dispatch notifies subscribed listeners with the new state', () {
    final store = Store<int, CounterAction>(counterReducer, 0);
    final seen = <int>[];
    store.subscribe(seen.add);
    store.dispatch(Increment());
    store.dispatch(Increment());
    expect(seen, [1, 2]);
  });

  test('multiple listeners are each notified once per dispatch', () {
    final store = Store<int, CounterAction>(counterReducer, 0);
    var aCalls = 0;
    var bCalls = 0;
    store.subscribe((_) => aCalls++);
    store.subscribe((_) => bCalls++);
    store.dispatch(Increment());
    expect(aCalls, 1);
    expect(bCalls, 1);
  });

  test('unsubscribe stops further notifications', () {
    final store = Store<int, CounterAction>(counterReducer, 0);
    final seen = <int>[];
    final unsubscribe = store.subscribe(seen.add);
    store.dispatch(Increment());
    unsubscribe();
    store.dispatch(Increment());
    expect(seen, [1]);
  });
}
