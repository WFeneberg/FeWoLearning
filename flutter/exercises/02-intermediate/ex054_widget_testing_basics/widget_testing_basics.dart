// Exercise 054 - flutter_test / pumpWidget basics (intermediate).
//
// Goal:   Build a widget that centers a "Count: <n>" label on screen.
// Drills: flutter_test, WidgetTester.pumpWidget, find.byType/find.text.
// Passes: when CounterDisplay renders the given count centered in a Text
//         widget wrapped by a Center widget.

import 'package:flutter/material.dart';

class CounterDisplay extends StatelessWidget {
  const CounterDisplay({super.key, required this.count});

  final int count;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
