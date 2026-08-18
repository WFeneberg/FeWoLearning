// Exercise 072 - flutter_riverpod basics (advanced).
//
// Goal:   Expose a counter via a Riverpod StateProvider and read/update it
//         from a ConsumerWidget.
// Drills: StateProvider, ProviderScope, ConsumerWidget, ref.watch, ref.read.
// Passes: when tapping the increment button updates the displayed count via
//         Riverpod's rebuild mechanism.

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

final counterProvider = StateProvider<int>((ref) => 0);

class CounterView extends ConsumerWidget {
  const CounterView({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    throw UnimplementedError('TODO');
  }
}

Widget buildCounterApp() {
  throw UnimplementedError('TODO');
}
