import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'error_boundary_widget.dart';

void main() {
  final originalBuilder = ErrorWidget.builder;

  tearDown(() {
    ErrorWidget.builder = originalBuilder;
  });

  testWidgets(
      'shows the fallback message instead of the default error screen',
      (tester) async {
    installErrorBoundary('Something went wrong');

    await tester.pumpWidget(
      const MaterialApp(home: Faulty()),
    );

    expect(tester.takeException(), isA<StateError>());
    expect(find.text('Something went wrong'), findsOneWidget);
  });
}
