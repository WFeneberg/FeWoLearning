// Exercise 069 - widget snapshot (golden) testing basics (reference solution).

import 'package:flutter/material.dart';

class BadgeIcon extends StatelessWidget {
  const BadgeIcon({super.key, required this.count});

  final int count;

  @override
  Widget build(BuildContext context) {
    return CircleAvatar(
      radius: 12,
      backgroundColor: Colors.red,
      child: Text('$count', style: const TextStyle(color: Colors.white, fontSize: 12)),
    );
  }
}
