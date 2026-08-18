import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'theme_data_basics.dart';

void main() {
  testWidgets('label picks up the color scheme primary color', (tester) async {
    const seedColor = Colors.deepPurple;

    await tester.pumpWidget(
      MaterialApp(
        theme: ThemeData(colorScheme: ColorScheme.fromSeed(seedColor: seedColor)),
        home: const Scaffold(body: PrimaryColorLabel()),
      ),
    );

    final text = tester.widget<Text>(find.text('Themed'));
    final expectedPrimary = ColorScheme.fromSeed(seedColor: seedColor).primary;

    expect(text.style?.color, expectedPrimary);
  });

  testWidgets('label re-themes when the ancestor Theme changes', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        theme: ThemeData(colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue)),
        home: const Scaffold(body: PrimaryColorLabel()),
      ),
    );
    final firstColor = tester.widget<Text>(find.text('Themed')).style?.color;

    await tester.pumpWidget(
      MaterialApp(
        theme: ThemeData(colorScheme: ColorScheme.fromSeed(seedColor: Colors.green)),
        home: const Scaffold(body: PrimaryColorLabel()),
      ),
    );
    await tester.pump();
    final secondColor = tester.widget<Text>(find.text('Themed')).style?.color;

    expect(firstColor, isNot(equals(secondColor)));
  });
}
