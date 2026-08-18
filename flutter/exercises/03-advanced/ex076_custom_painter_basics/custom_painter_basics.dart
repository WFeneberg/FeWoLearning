// Exercise 076 - CustomPainter basics (advanced).
//
// Goal:   Implement a CustomPainter that draws a bar for each value, and a
//         shouldRepaint() that only requests a repaint when the values
//         actually changed.
// Drills: CustomPainter, Canvas, Paint, shouldRepaint.
// Passes: when shouldRepaint() returns false for an identical values list
//         and true when the values differ (including differing lengths).

import 'package:flutter/material.dart';

class BarPainter extends CustomPainter {
  BarPainter(this.values);

  final List<double> values;

  @override
  void paint(Canvas canvas, Size size) {
    throw UnimplementedError('TODO');
  }

  @override
  bool shouldRepaint(covariant BarPainter oldDelegate) {
    throw UnimplementedError('TODO');
  }
}
