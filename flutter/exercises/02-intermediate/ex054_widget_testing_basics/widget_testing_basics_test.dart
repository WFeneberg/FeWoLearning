import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'widget_testing_basics.dart';

void main() {
  testWidgets('pumpWidget renders the widget tree', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: CounterDisplay(count: 0))),
    );

    expect(find.byType(CounterDisplay), findsOneWidget);
  });

  testWidgets('CounterDisplay shows the given count as text', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: CounterDisplay(count: 7))),
    );

    expect(find.text('Count: 7'), findsOneWidget);
  });

  testWidgets('CounterDisplay is centered on screen', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: CounterDisplay(count: 1))),
    );

    expect(find.byType(Center), findsOneWidget);
  });
}
