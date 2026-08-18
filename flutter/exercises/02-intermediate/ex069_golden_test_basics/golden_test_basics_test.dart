import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'golden_test_basics.dart';

void main() {
  testWidgets('BadgeIcon shows the count text', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: BadgeIcon(count: 9))),
    );

    expect(find.text('9'), findsOneWidget);
  });

  // Golden test: compares the rendered widget against a reference PNG.
  // The reference file does not exist yet on this unverified track — the
  // first `flutter test --update-goldens` run on a real machine generates
  // it; afterwards this test only passes if the render matches it exactly.
  testWidgets('BadgeIcon matches its golden reference image', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: Center(child: BadgeIcon(count: 9)))),
    );

    await expectLater(
      find.byType(BadgeIcon),
      matchesGoldenFile('goldens/badge_icon_9.png'),
    );
  });
}
