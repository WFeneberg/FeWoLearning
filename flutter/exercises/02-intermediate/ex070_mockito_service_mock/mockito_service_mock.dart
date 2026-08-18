// Exercise 070 - mocking a service with mockito (intermediate).
//
// Goal:   Build a widget that loads a greeting from an injected service and
//         shows a progress indicator until it resolves.
// Drills: mockito (Mock, when/thenAnswer, verify), dependency injection via
//         constructor, FutureBuilder.
// Passes: when GreetingLoader shows a CircularProgressIndicator before the
//         service's future resolves, and the resolved greeting text
//         afterwards.

import 'package:flutter/material.dart';

abstract class GreetingService {
  Future<String> fetchGreeting(String name);
}

class GreetingLoader extends StatelessWidget {
  const GreetingLoader({super.key, required this.service, required this.name});

  final GreetingService service;
  final String name;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
