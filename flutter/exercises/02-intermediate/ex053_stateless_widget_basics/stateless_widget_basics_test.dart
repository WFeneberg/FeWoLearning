import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'stateless_widget_basics.dart';

void main() {
  testWidgets('GreetingCard shows a personalized hello', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: GreetingCard(name: 'Ada'))),
    );

    expect(find.text('Hello, Ada!'), findsOneWidget);
  });

  testWidgets('GreetingCard always shows the welcome subtitle', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: GreetingCard(name: 'Grace'))),
    );

    expect(find.text('Welcome to Flutter/Dart exercises.'), findsOneWidget);
  });

  testWidgets('GreetingCard rebuilds with a new name', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: GreetingCard(name: 'Ada'))),
    );
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: GreetingCard(name: 'Grace'))),
    );
    await tester.pump();

    expect(find.text('Hello, Ada!'), findsNothing);
    expect(find.text('Hello, Grace!'), findsOneWidget);
  });
}
