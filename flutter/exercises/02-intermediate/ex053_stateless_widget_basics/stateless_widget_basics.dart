// Exercise 053 - StatelessWidget basics (intermediate).
//
// Goal:   Build a small widget that greets a user by name and always shows
//         a fixed welcome subtitle underneath.
// Drills: StatelessWidget, build(), widget tree composition.
// Passes: when GreetingCard renders "Hello, <name>!" plus the welcome
//         subtitle, and re-renders correctly when the name prop changes.

import 'package:flutter/material.dart';

class GreetingCard extends StatelessWidget {
  const GreetingCard({super.key, required this.name});

  final String name;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
