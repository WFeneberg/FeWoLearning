// Exercise 078 - implicit animations (reference solution).

import 'package:flutter/material.dart';

class AnimatedBox extends StatelessWidget {
  const AnimatedBox({super.key, required this.expanded});

  final bool expanded;

  static const Duration animationDuration = Duration(milliseconds: 200);

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
      duration: animationDuration,
      width: expanded ? 200 : 100,
      height: expanded ? 200 : 100,
      color: expanded ? Colors.blue : Colors.grey,
    );
  }
}
