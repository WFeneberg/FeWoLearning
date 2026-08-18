// Exercise 054 - flutter_test / pumpWidget basics (reference solution).

import 'package:flutter/material.dart';

class CounterDisplay extends StatelessWidget {
  const CounterDisplay({super.key, required this.count});

  final int count;

  @override
  Widget build(BuildContext context) {
    return Center(child: Text('Count: $count'));
  }
}
