// Exercise 072 - flutter_riverpod basics (reference solution).

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

final counterProvider = StateProvider<int>((ref) => 0);

class CounterView extends ConsumerWidget {
  const CounterView({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final count = ref.watch(counterProvider);
    return Scaffold(
      body: Column(
        children: [
          Text('Count: $count'),
          ElevatedButton(
            onPressed: () => ref.read(counterProvider.notifier).state++,
            child: const Text('Increment'),
          ),
        ],
      ),
    );
  }
}

Widget buildCounterApp() {
  return const ProviderScope(child: MaterialApp(home: CounterView()));
}
