// Exercise 055 - StatefulWidget basics (intermediate).
//
// Goal:   Build a button that increments and displays a counter each time
//         it is tapped.
// Drills: StatefulWidget, State, setState.
// Passes: when tapping the button increments the displayed count by one
//         each time, starting from zero.

import 'package:flutter/material.dart';

class CounterButton extends StatefulWidget {
  const CounterButton({super.key});

  @override
  State<CounterButton> createState() => _CounterButtonState();
}

class _CounterButtonState extends State<CounterButton> {
  int _count = 0;

  void _increment() {
    throw UnimplementedError('TODO');
  }

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
