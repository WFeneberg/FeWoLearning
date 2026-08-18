// Exercise 083 - a basic error boundary (reference solution).

import 'package:flutter/material.dart';

void installErrorBoundary(String fallbackMessage) {
  ErrorWidget.builder = (FlutterErrorDetails details) {
    return Center(child: Text(fallbackMessage));
  };
}

class Faulty extends StatelessWidget {
  const Faulty({super.key});

  @override
  Widget build(BuildContext context) {
    throw StateError('boom');
  }
}
