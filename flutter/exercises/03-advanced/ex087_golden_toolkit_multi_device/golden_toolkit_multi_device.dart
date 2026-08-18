// Exercise 087 - golden tests across multiple devices (advanced).
//
// Goal:   Build a simple greeting card widget that golden_toolkit can
//         render across several device sizes in one golden test.
// Drills: golden_toolkit, DeviceBuilder, multi-device golden matrices.
// Passes: when GreetingCard renders the given name inside a Card, so the
//         accompanying golden test can register it for phone/tablet devices.

import 'package:flutter/material.dart';

class GreetingCard extends StatelessWidget {
  const GreetingCard({super.key, required this.name});

  final String name;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
