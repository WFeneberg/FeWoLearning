// Exercise 071 - package:provider basics (reference solution).

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class CounterModel extends ChangeNotifier {
  int _count = 0;
  int get count => _count;

  void increment() {
    _count++;
    notifyListeners();
  }
}

class CounterView extends StatelessWidget {
  const CounterView({super.key});

  @override
  Widget build(BuildContext context) {
    final count = context.watch<CounterModel>().count;
    return Scaffold(
      body: Column(
        children: [
          Text('Count: $count'),
          ElevatedButton(
            onPressed: () => context.read<CounterModel>().increment(),
            child: const Text('Increment'),
          ),
        ],
      ),
    );
  }
}

Widget buildCounterApp() {
  return ChangeNotifierProvider(
    create: (_) => CounterModel(),
    child: const MaterialApp(home: CounterView()),
  );
}
