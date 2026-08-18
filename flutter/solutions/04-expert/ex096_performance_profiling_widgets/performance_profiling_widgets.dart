// Exercise 096 - RepaintBoundary avoids unnecessary repaints (reference
// solution).

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
    return Column(
      children: [
        Text('$_count'),
        ElevatedButton(
          onPressed: () => setState(() => _count++),
          child: const Text('+'),
        ),
        RepaintBoundary(
          child: CustomPaint(
            painter: ExpensivePainter(widget.onPaint),
            size: const Size(50, 50),
          ),
        ),
      ],
    );
  }
}
