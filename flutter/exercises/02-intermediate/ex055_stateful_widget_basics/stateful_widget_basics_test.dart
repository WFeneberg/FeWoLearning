import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'stateful_widget_basics.dart';

void main() {
  testWidgets('CounterButton starts at zero', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: CounterButton())));

    expect(find.text('0'), findsOneWidget);
  });

  testWidgets('tapping the button increments the count', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: CounterButton())));

    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('1'), findsOneWidget);
    expect(find.text('0'), findsNothing);
  });

  testWidgets('tapping twice increments twice', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: CounterButton())));

    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();
    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('2'), findsOneWidget);
  });
}
