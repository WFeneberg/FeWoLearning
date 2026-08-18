// Exercise 092 - CQRS-style command/query separation in a Flutter feature
// module (reference solution).

import 'package:flutter/material.dart';

class CounterStore {
  int _value = 0;
  final List<VoidCallback> _listeners = [];

  int get value => _value;

  void addListener(VoidCallback listener) => _listeners.add(listener);

  void removeListener(VoidCallback listener) => _listeners.remove(listener);

  void setValue(int next) {
    _value = next;
    for (final listener in List<VoidCallback>.of(_listeners)) {
      listener();
    }
  }
}

class CounterCommands {
  CounterCommands(this._store);
  final CounterStore _store;

  void increment() => _store.setValue(_store.value + 1);

  void reset() => _store.setValue(0);
}

class CounterQueries {
  CounterQueries(this._store);
  final CounterStore _store;

  int currentValue() => _store.value;
}

class CounterView extends StatefulWidget {
  const CounterView({super.key, required this.store});
  final CounterStore store;

  @override
  State<CounterView> createState() => _CounterViewState();
}

class _CounterViewState extends State<CounterView> {
  late final CounterCommands _commands = CounterCommands(widget.store);
  late final CounterQueries _queries = CounterQueries(widget.store);

  @override
  void initState() {
    super.initState();
    widget.store.addListener(_onStoreChanged);
  }

  @override
  void dispose() {
    widget.store.removeListener(_onStoreChanged);
    super.dispose();
  }

  void _onStoreChanged() => setState(() {});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text('${_queries.currentValue()}'),
        ElevatedButton(
          onPressed: _commands.increment,
          child: const Text('+'),
        ),
      ],
    );
  }
}
