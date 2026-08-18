// Exercise 078 - implicit animations (advanced).
//
// Goal:   Build a widget that animates its size and color between a
//         "collapsed" and "expanded" state using AnimatedContainer.
// Drills: AnimatedContainer, implicit animations, Duration-driven pump.
// Passes: when toggling expanded rebuilds with the new target size/color,
//         and the animation is still mid-flight at a partial duration.

import 'package:flutter/material.dart';

class AnimatedBox extends StatelessWidget {
  const AnimatedBox({super.key, required this.expanded});

  final bool expanded;

  static const Duration animationDuration = Duration(milliseconds: 200);

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
