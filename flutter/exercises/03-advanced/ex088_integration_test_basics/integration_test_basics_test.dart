import 'package:flutter/material.dart';
import 'package:integration_test/integration_test.dart';

import 'integration_test_basics.dart';

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('tapping the FAB increments the visible count end-to-end',
      (tester) async {
    await tester.pumpWidget(const CounterApp());

    expect(find.text('Count: 0'), findsOneWidget);

    await tester.tap(find.byType(FloatingActionButton));
    await tester.pumpAndSettle();

    expect(find.text('Count: 1'), findsOneWidget);
  });
}
