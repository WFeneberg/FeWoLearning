// Exercise 058 - ValueNotifier basics (intermediate).
//
// Goal:   Build a widget that displays a temperature from a ValueNotifier
//         and re-renders whenever the notifier's value changes.
// Drills: ValueNotifier, ValueListenableBuilder.
// Passes: when TemperatureDisplay shows the current value formatted to one
//         decimal place with a °C suffix, and updates after
//         `temperature.value = ...` without rebuilding the whole widget.

import 'package:flutter/material.dart';

class TemperatureDisplay extends StatelessWidget {
  const TemperatureDisplay({super.key, required this.temperature});

  final ValueNotifier<double> temperature;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
