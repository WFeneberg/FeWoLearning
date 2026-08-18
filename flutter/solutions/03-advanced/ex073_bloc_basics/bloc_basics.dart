// Exercise 073 - flutter_bloc basics (reference solution).

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

sealed class CounterEvent {}

class CounterIncremented extends CounterEvent {}

class CounterBloc extends Bloc<CounterEvent, int> {
  CounterBloc() : super(0) {
    on<CounterIncremented>((event, emit) => emit(state + 1));
  }
}

class CounterView extends StatelessWidget {
  const CounterView({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          BlocBuilder<CounterBloc, int>(
            builder: (context, count) => Text('Count: $count'),
          ),
          ElevatedButton(
            onPressed: () =>
                context.read<CounterBloc>().add(CounterIncremented()),
            child: const Text('Increment'),
          ),
        ],
      ),
    );
  }
}

Widget buildCounterApp() {
  return BlocProvider(
    create: (_) => CounterBloc(),
    child: const MaterialApp(home: CounterView()),
  );
}
