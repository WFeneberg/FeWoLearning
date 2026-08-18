// Exercise 066 - GestureDetector basics (reference solution).

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
    return GestureDetector(
      onHorizontalDragUpdate: (details) {
        setState(() {
          _distance += details.delta.dx;
        });
      },
      onTap: () {
        setState(() {
          _distance = 0;
        });
      },
      child: Container(
        color: Colors.grey.shade200,
        padding: const EdgeInsets.all(16),
        child: Text('${_distance.round()}'),
      ),
    );
  }
}
