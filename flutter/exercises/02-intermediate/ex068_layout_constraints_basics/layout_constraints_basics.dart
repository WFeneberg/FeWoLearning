// Exercise 068 - layout constraints basics (intermediate).
//
// Goal:   Build a two-pane layout: a fixed-width sidebar and a content
//         area that expands to fill the remaining horizontal space.
// Drills: Row, SizedBox, Expanded, constraint flow.
// Passes: when the sidebar's rendered width equals sidebarWidth exactly,
//         and the content area's rendered width equals the available width
//         minus sidebarWidth.

import 'package:flutter/material.dart';

class SidebarLayout extends StatelessWidget {
  const SidebarLayout({super.key, required this.sidebarWidth});

  final double sidebarWidth;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
