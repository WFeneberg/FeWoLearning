import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'key_widget_identity.dart';

void main() {
  testWidgets('buildList renders one CheckableItem per label', (tester) async {
    await tester.pumpWidget(MaterialApp(home: Scaffold(body: buildList(['A', 'B']))));

    expect(find.text('A'), findsOneWidget);
    expect(find.text('B'), findsOneWidget);
  });

  testWidgets('checking an item keeps its state after the list reorders', (tester) async {
    await tester.pumpWidget(MaterialApp(home: Scaffold(body: buildList(['A', 'B']))));

    await tester.tap(find.widgetWithText(CheckboxListTile, 'A'));
    await tester.pump();

    await tester.pumpWidget(MaterialApp(home: Scaffold(body: buildList(['B', 'A']))));
    await tester.pump();

    final tileA = tester.widget<CheckboxListTile>(find.widgetWithText(CheckboxListTile, 'A'));
    expect(tileA.value, isTrue);
  });

  testWidgets('the other item stays unchecked after the reorder', (tester) async {
    await tester.pumpWidget(MaterialApp(home: Scaffold(body: buildList(['A', 'B']))));

    await tester.tap(find.widgetWithText(CheckboxListTile, 'A'));
    await tester.pump();

    await tester.pumpWidget(MaterialApp(home: Scaffold(body: buildList(['B', 'A']))));
    await tester.pump();

    final tileB = tester.widget<CheckboxListTile>(find.widgetWithText(CheckboxListTile, 'B'));
    expect(tileB.value, isFalse);
  });
}
