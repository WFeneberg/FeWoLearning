// Exercise 061 - StreamBuilder basics (intermediate).
//
// Goal:   Build a widget that shows a live score fed by a Stream<int>,
//         starting from 0 before any event arrives.
// Drills: StreamBuilder, AsyncSnapshot, initialData.
// Passes: when LiveScoreDisplay shows "Score: 0" before any stream event,
//         and "Score: <n>" for the most recently emitted value afterwards.

import 'package:flutter/material.dart';

class LiveScoreDisplay extends StatelessWidget {
  const LiveScoreDisplay({super.key, required this.scoreStream});

  final Stream<int> scoreStream;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
