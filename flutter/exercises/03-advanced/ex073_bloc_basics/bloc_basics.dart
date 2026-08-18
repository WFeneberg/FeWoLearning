// Exercise 073 - flutter_bloc basics (advanced).
//
// Goal:   Model a counter as a Bloc with explicit events and states, and
//         drive it from a BlocBuilder.
// Drills: Bloc, events, states, BlocProvider, BlocBuilder.
// Passes: when tapping the increment button dispatches a CounterIncremented
//         event and the BlocBuilder rebuilds with the new state.

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

sealed class CounterEvent {}

class CounterIncremented extends CounterEvent {}

class CounterBloc extends Bloc<CounterEvent, int> {
  CounterBloc() : super(0) {
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
