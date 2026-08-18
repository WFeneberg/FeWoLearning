import 'package:flutter/material.dart';
import 'package:flutter/semantics.dart';
import 'package:flutter_test/flutter_test.dart';

import 'accessibility_semantics.dart';

void main() {
  testWidgets('exposes a semantic label and a tap action', (tester) async {
    final handle = tester.ensureSemantics();
    var pressed = false;

    await tester.pumpWidget(
      MaterialApp(
        home: LabeledIconButton(
          icon: Icons.favorite,
          label: 'Add to favorites',
          onPressed: () => pressed = true,
        ),
      ),
    );

    final finder = find.bySemanticsLabel('Add to favorites');
    expect(finder, findsOneWidget);

    final node = tester.getSemantics(finder);
    expect(node.label, 'Add to favorites');
    expect(node.hasAction(SemanticsAction.tap), isTrue);

    await tester.tap(finder);
    expect(pressed, isTrue);

    handle.dispose();
  });
}
