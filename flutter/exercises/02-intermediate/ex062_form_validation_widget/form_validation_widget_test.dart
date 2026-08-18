import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'form_validation_widget.dart';

void main() {
  test('emailValidator rejects a value without @', () {
    expect(emailValidator('not-an-email'), isNotNull);
  });

  test('emailValidator accepts a value with @', () {
    expect(emailValidator('ada@example.com'), isNull);
  });

  testWidgets('submitting an invalid email shows an error message', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: EmailForm())));

    await tester.enterText(find.byType(TextFormField), 'nope');
    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('Invalid email'), findsOneWidget);
  });

  testWidgets('submitting a valid email shows a success message', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: EmailForm())));

    await tester.enterText(find.byType(TextFormField), 'ada@example.com');
    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('Valid'), findsOneWidget);
  });
}
