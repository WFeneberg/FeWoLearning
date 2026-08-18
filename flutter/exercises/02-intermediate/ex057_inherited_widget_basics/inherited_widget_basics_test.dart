import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'inherited_widget_basics.dart';

void main() {
  testWidgets('CartBadge reads the item count from CartInfo', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(body: CartInfo(itemCount: 3, child: CartBadge())),
      ),
    );

    expect(find.text('3 items'), findsOneWidget);
  });

  testWidgets('CartBadge falls back to 0 with no ancestor CartInfo', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: CartBadge())),
    );

    expect(find.text('0 items'), findsOneWidget);
  });

  testWidgets('CartBadge updates when CartInfo rebuilds with a new count', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(body: CartInfo(itemCount: 1, child: CartBadge())),
      ),
    );
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(body: CartInfo(itemCount: 5, child: CartBadge())),
      ),
    );
    await tester.pump();

    expect(find.text('5 items'), findsOneWidget);
    expect(find.text('1 items'), findsNothing);
  });
}
