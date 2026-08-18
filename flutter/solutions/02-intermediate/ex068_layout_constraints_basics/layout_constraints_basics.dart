// Exercise 068 - layout constraints basics (reference solution).

import 'package:flutter/material.dart';

class SidebarLayout extends StatelessWidget {
  const SidebarLayout({super.key, required this.sidebarWidth});

  final double sidebarWidth;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        SizedBox(
          width: sidebarWidth,
          child: Container(color: Colors.grey.shade300, key: const ValueKey('sidebar')),
        ),
        Expanded(
          child: Container(color: Colors.white, key: const ValueKey('content')),
        ),
      ],
    );
  }
}
