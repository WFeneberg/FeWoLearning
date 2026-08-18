// Exercise 067 - AnimationController & Tween basics (reference solution).

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
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 300),
    );
    _opacity = Tween<double>(begin: 0, end: 1).animate(_controller);
    _controller.forward();
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
