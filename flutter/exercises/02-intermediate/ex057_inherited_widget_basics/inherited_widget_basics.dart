// Exercise 057 - InheritedWidget basics (intermediate).
//
// Goal:   Build a CartInfo InheritedWidget carrying an item count, and a
//         CartBadge that reads that count from the nearest ancestor.
// Drills: InheritedWidget, dependOnInheritedWidgetOfExactType,
//         updateShouldNotify.
// Passes: when CartBadge reads the count through CartInfo.of(context),
//         rebuilds when the ancestor's count changes, and falls back to 0
//         when there is no ancestor CartInfo.

import 'package:flutter/material.dart';

class CartInfo extends InheritedWidget {
  const CartInfo({super.key, required this.itemCount, required super.child});

  final int itemCount;

  static CartInfo? of(BuildContext context) {
    throw UnimplementedError('TODO');
  }

  @override
  bool updateShouldNotify(CartInfo oldWidget) {
    throw UnimplementedError('TODO');
  }
}

class CartBadge extends StatelessWidget {
  const CartBadge({super.key});

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
