// Exercise 057 - InheritedWidget basics (reference solution).

import 'package:flutter/material.dart';

class CartInfo extends InheritedWidget {
  const CartInfo({super.key, required this.itemCount, required super.child});

  final int itemCount;

  static CartInfo? of(BuildContext context) {
    return context.dependOnInheritedWidgetOfExactType<CartInfo>();
  }

  @override
  bool updateShouldNotify(CartInfo oldWidget) {
    return itemCount != oldWidget.itemCount;
  }
}

class CartBadge extends StatelessWidget {
  const CartBadge({super.key});

  @override
  Widget build(BuildContext context) {
    final cart = CartInfo.of(context);
    return Text('${cart?.itemCount ?? 0} items');
  }
}
