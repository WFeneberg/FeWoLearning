// Exercise 059 - ChangeNotifier basics (reference solution).

import 'package:flutter/material.dart';

class CartModel extends ChangeNotifier {
  int _itemCount = 0;

  int get itemCount => _itemCount;

  void addItem() {
    _itemCount++;
    notifyListeners();
  }
}

class CartCounterText extends StatelessWidget {
  const CartCounterText({super.key, required this.cart});

  final CartModel cart;

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: cart,
      builder: (context, child) => Text('${cart.itemCount}'),
    );
  }
}
