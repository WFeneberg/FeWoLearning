// Exercise 053 - StatelessWidget basics (reference solution).

import 'package:flutter/material.dart';

class GreetingCard extends StatelessWidget {
  const GreetingCard({super.key, required this.name});

  final String name;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text('Hello, $name!'),
        const Text('Welcome to Flutter/Dart exercises.'),
      ],
    );
  }
}
