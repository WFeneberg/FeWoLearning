// Exercise 069 - widget snapshot (golden) testing basics (intermediate).
//
// Goal:   Build a small circular badge icon showing a numeric count.
// Drills: matchesGoldenFile, widget snapshot testing.
// Passes: when BadgeIcon renders the count as white text over a red
//         CircleAvatar, and its render matches the golden reference image
//         once one has been generated on a real machine.

import 'package:flutter/material.dart';

class BadgeIcon extends StatelessWidget {
  const BadgeIcon({super.key, required this.count});

  final int count;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
