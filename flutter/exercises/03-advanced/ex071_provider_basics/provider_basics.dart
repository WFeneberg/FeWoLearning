// Exercise 071 - package:provider basics (advanced).
//
// Goal:   Wire up a ChangeNotifier-backed counter using package:provider and
//         a widget that reads/watches it.
// Drills: ChangeNotifier, ChangeNotifierProvider, context.watch, context.read.
// Passes: when tapping the increment button updates the displayed count via
//         provider's rebuild mechanism (no manual setState in the view).

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class CounterModel extends ChangeNotifier {
  int _count = 0;
  int get count => _count;

  void increment() {
    throw UnimplementedError('TODO');
  }
}

class CounterView extends StatelessWidget {
  const CounterView({super.key});

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}

Widget buildCounterApp() {
  throw UnimplementedError('TODO');
}
