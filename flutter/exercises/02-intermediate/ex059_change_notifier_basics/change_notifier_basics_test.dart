import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'change_notifier_basics.dart';

void main() {
  testWidgets('CartCounterText shows zero items initially', (tester) async {
    final cart = CartModel();

    await tester.pumpWidget(MaterialApp(home: Scaffold(body: CartCounterText(cart: cart))));

    expect(find.text('0'), findsOneWidget);
  });

  testWidgets('CartCounterText updates after addItem notifies listeners', (tester) async {
    final cart = CartModel();

    await tester.pumpWidget(MaterialApp(home: Scaffold(body: CartCounterText(cart: cart))));

    cart.addItem();
    await tester.pump();

    expect(find.text('1'), findsOneWidget);
  });

  test('addItem increments itemCount and notifies listeners', () {
    final cart = CartModel();
    var notified = false;
    cart.addListener(() => notified = true);

    cart.addItem();

    expect(cart.itemCount, 1);
    expect(notified, isTrue);
  });
}
