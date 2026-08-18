// Exercise 059 - ChangeNotifier basics (intermediate).
//
// Goal:   Build a CartModel that tracks an item count and notifies its
//         listeners whenever an item is added, plus a widget that displays
//         that count live.
// Drills: ChangeNotifier, notifyListeners, ListenableBuilder.
// Passes: when addItem() increments itemCount and calls notifyListeners(),
//         and CartCounterText re-renders to show the new count.

import 'package:flutter/material.dart';

class CartModel extends ChangeNotifier {
  int _itemCount = 0;

  int get itemCount => _itemCount;

  void addItem() {
    throw UnimplementedError('TODO');
  }
}

class CartCounterText extends StatelessWidget {
  const CartCounterText({super.key, required this.cart});

  final CartModel cart;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
