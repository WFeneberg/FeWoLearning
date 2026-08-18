// Exercise 070 - mocking a service with mockito (reference solution).

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
    return FutureBuilder<String>(
      future: service.fetchGreeting(name),
      builder: (context, snapshot) {
        if (!snapshot.hasData) {
          return const CircularProgressIndicator();
        }
        return Text(snapshot.data!);
      },
    );
  }
}
