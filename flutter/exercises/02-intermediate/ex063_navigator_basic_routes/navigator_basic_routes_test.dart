import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'navigator_basic_routes.dart';

void main() {
  Widget buildApp() {
    return MaterialApp(
      initialRoute: '/',
      routes: {
        '/': (context) => const HomeScreen(),
        '/details': (context) => const DetailsScreen(),
      },
    );
  }

  testWidgets('starts on the home screen', (tester) async {
    await tester.pumpWidget(buildApp());

    expect(find.text('Go to details'), findsOneWidget);
  });

  testWidgets('tapping the button navigates to the details screen', (tester) async {
    await tester.pumpWidget(buildApp());

    await tester.tap(find.text('Go to details'));
    await tester.pumpAndSettle();

    expect(find.text('Go back'), findsOneWidget);
    expect(find.text('Go to details'), findsNothing);
  });

  testWidgets('popping the details screen returns home', (tester) async {
    await tester.pumpWidget(buildApp());

    await tester.tap(find.text('Go to details'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Go back'));
    await tester.pumpAndSettle();

    expect(find.text('Go to details'), findsOneWidget);
  });
}
