// Exercise 058 - ValueNotifier basics (reference solution).

import 'package:flutter/material.dart';

class TemperatureDisplay extends StatelessWidget {
  const TemperatureDisplay({super.key, required this.temperature});

  final ValueNotifier<double> temperature;

  @override
  Widget build(BuildContext context) {
    return ValueListenableBuilder<double>(
      valueListenable: temperature,
      builder: (context, value, child) {
        return Text('${value.toStringAsFixed(1)}°C');
      },
    );
  }
}
