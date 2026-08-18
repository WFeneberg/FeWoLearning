// Exercise 083 - a basic error boundary (advanced).
//
// Goal:   Make build errors from a subtree render a friendly fallback
//         message instead of Flutter's default red error screen.
// Drills: ErrorWidget.builder, graceful degradation, global error hooks.
// Passes: when a descendant that throws during build() is replaced by the
//         given fallback message rather than crashing the test.

import 'package:flutter/material.dart';

void installErrorBoundary(String fallbackMessage) {
  throw UnimplementedError('TODO');
}

class Faulty extends StatelessWidget {
  const Faulty({super.key});

  @override
  Widget build(BuildContext context) {
    throw StateError('boom');
  }
}
