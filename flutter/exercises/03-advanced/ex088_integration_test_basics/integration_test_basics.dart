// Exercise 088 - integration_test basics (advanced).
//
// Goal:   Build a minimal counter app and drive it end-to-end with the
//         integration_test package's testWidgets.
// Drills: IntegrationTestWidgetsFlutterBinding, end-to-end widget testing.
// Passes: when the app increments visibly on tap, verified through a full
//         pump-tap-pump integration test rather than a unit test.

import 'package:flutter/material.dart';

class CounterApp extends StatefulWidget {
  const CounterApp({super.key});

  @override
  State<CounterApp> createState() => _CounterAppState();
}

class _CounterAppState extends State<CounterApp> {
  int _count = 0;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
