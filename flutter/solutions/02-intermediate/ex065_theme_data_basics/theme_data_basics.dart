// Exercise 065 - ThemeData basics (reference solution).

import 'package:flutter/material.dart';

class PrimaryColorLabel extends StatelessWidget {
  const PrimaryColorLabel({super.key});

  @override
  Widget build(BuildContext context) {
    final primary = Theme.of(context).colorScheme.primary;
    return Text('Themed', style: TextStyle(color: primary));
  }
}
