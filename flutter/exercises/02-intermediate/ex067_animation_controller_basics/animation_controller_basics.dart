// Exercise 067 - AnimationController & Tween basics (intermediate).
//
// Goal:   Build a box that fades in from fully transparent to fully opaque
//         over 300ms as soon as it is built.
// Drills: AnimationController, Tween, SingleTickerProviderStateMixin,
//         FadeTransition.
// Passes: when the opacity animation starts at 0.0, ends at 1.0 once
//         settled, and sits strictly between them mid-animation.

import 'package:flutter/material.dart';

class FadeInBox extends StatefulWidget {
  const FadeInBox({super.key});

  @override
  State<FadeInBox> createState() => _FadeInBoxState();
}

class _FadeInBoxState extends State<FadeInBox> with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<double> _opacity;

  @override
  void initState() {
    super.initState();
    throw UnimplementedError('TODO');
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: _opacity,
      child: Container(width: 100, height: 100, color: Colors.blue),
    );
  }
}
