import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'future_builder_widget.dart';

void main() {
  testWidgets('shows a loading indicator before the future completes', (tester) async {
    final completer = Completer<String>();

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: UserNameLoader(nameFuture: completer.future))),
    );

    expect(find.text('Loading...'), findsOneWidget);

    completer.complete('Ada');
    await tester.pumpAndSettle();
  });

  testWidgets('shows the resolved name once the future completes', (tester) async {
    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: UserNameLoader(nameFuture: Future.value('Ada')))),
    );

    await tester.pumpAndSettle();

    expect(find.text('Ada'), findsOneWidget);
    expect(find.text('Loading...'), findsNothing);
  });

  testWidgets('shows an error message when the future fails', (tester) async {
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(body: UserNameLoader(nameFuture: Future.error('boom'))),
      ),
    );

    await tester.pumpAndSettle();

    expect(find.text('Error'), findsOneWidget);
  });
}
