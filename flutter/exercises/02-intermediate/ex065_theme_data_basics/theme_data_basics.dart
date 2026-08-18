// Exercise 065 - ThemeData basics (intermediate).
//
// Goal:   Build a label whose text color comes from the ambient theme's
//         color scheme, not a hard-coded color.
// Drills: ThemeData, Theme.of(context), ColorScheme.
// Passes: when PrimaryColorLabel's text color equals
//         Theme.of(context).colorScheme.primary for whatever theme it is
//         built under, and updates when the ancestor Theme changes.

import 'package:flutter/material.dart';

class PrimaryColorLabel extends StatelessWidget {
  const PrimaryColorLabel({super.key});

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
