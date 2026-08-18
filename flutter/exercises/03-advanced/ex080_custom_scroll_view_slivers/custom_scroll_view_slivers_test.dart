import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'custom_scroll_view_slivers.dart';

void main() {
  final items = List.generate(40, (i) => 'Item $i');

  testWidgets('first item is visible without scrolling', (tester) async {
    await tester.pumpWidget(buildItemListApp(items));

    expect(find.text('Item 0'), findsOneWidget);
    expect(find.text('Item 39'), findsNothing);
  });

  testWidgets('scrolling reveals a later item', (tester) async {
    await tester.pumpWidget(buildItemListApp(items));

    await tester.fling(find.byType(CustomScrollView), const Offset(0, -3000), 3000);
    await tester.pumpAndSettle();

    expect(find.text('Item 39'), findsOneWidget);
  });
}
