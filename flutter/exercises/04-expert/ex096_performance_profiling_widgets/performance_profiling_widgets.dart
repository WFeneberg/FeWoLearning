// Exercise 096 - RepaintBoundary avoids unnecessary repaints (expert).
//
// Goal:   Build ProfiledCounterView so that tapping the increment button
//         rebuilds the counter text but does NOT trigger a repaint of the
//         expensive CustomPainter below it.
// Drills: RepaintBoundary, CustomPainter.shouldRepaint, isolating repaint
//         regions from unrelated setState() calls.
// Passes: when tapping the button repaints the counter but the painter's
//         paint() call count stays unchanged.

import 'package:flutter/material.dart';

class ExpensivePainter extends CustomPainter {
  ExpensivePainter(this.onPaint);
  final VoidCallback onPaint;

  @override
  void paint(Canvas canvas, Size size) {
    onPaint();
    canvas.drawRect(Offset.zero & size, Paint()..color = Colors.blue);
  }

  @override
  bool shouldRepaint(covariant ExpensivePainter oldDelegate) => false;
}

class ProfiledCounterView extends StatefulWidget {
  const ProfiledCounterView({super.key, required this.onPaint});
  final VoidCallback onPaint;

  @override
  State<ProfiledCounterView> createState() => _ProfiledCounterViewState();
}

class _ProfiledCounterViewState extends State<ProfiledCounterView> {
  int _count = 0;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
