// Exercise 066 - GestureDetector basics (intermediate).
//
// Goal:   Build a box that accumulates horizontal drag distance and resets
//         to zero on tap.
// Drills: GestureDetector, onHorizontalDragUpdate, onTap.
// Passes: when dragging horizontally adds the drag delta's dx to the
//         running total, shown rounded to the nearest integer, and tapping
//         resets that total back to 0.

import 'package:flutter/material.dart';

class DragCounter extends StatefulWidget {
  const DragCounter({super.key});

  @override
  State<DragCounter> createState() => _DragCounterState();
}

class _DragCounterState extends State<DragCounter> {
  double _distance = 0;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
