// Exercise 076 - CustomPainter basics (reference solution).

import 'package:flutter/material.dart';

class BarPainter extends CustomPainter {
  BarPainter(this.values);

  final List<double> values;

  @override
  void paint(Canvas canvas, Size size) {
    if (values.isEmpty) return;
    final barWidth = size.width / values.length;
    final paint = Paint()..color = Colors.blue;
    for (var i = 0; i < values.length; i++) {
      final barHeight = values[i] * size.height;
      canvas.drawRect(
        Rect.fromLTWH(
          i * barWidth,
          size.height - barHeight,
          barWidth,
          barHeight,
        ),
        paint,
      );
    }
  }

  @override
  bool shouldRepaint(covariant BarPainter oldDelegate) {
    if (oldDelegate.values.length != values.length) return true;
    for (var i = 0; i < values.length; i++) {
      if (oldDelegate.values[i] != values[i]) return true;
    }
    return false;
  }
}
