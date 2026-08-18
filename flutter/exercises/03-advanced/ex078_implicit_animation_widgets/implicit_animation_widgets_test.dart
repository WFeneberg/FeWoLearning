import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'implicit_animation_widgets.dart';

void main() {
  testWidgets('renders the collapsed size immediately', (tester) async {
    await tester.pumpWidget(
      const Directionality(
        textDirection: TextDirection.ltr,
        child: AnimatedBox(expanded: false),
      ),
    );

    expect(tester.getSize(find.byType(AnimatedContainer)), const Size(100, 100));
  });

  testWidgets('is mid-flight partway through the animation', (tester) async {
    await tester.pumpWidget(
      const Directionality(
        textDirection: TextDirection.ltr,
        child: AnimatedBox(expanded: false),
      ),
    );

    await tester.pumpWidget(
      const Directionality(
        textDirection: TextDirection.ltr,
        child: AnimatedBox(expanded: true),
      ),
    );
    await tester.pump(AnimatedBox.animationDuration ~/ 2);

    final size = tester.getSize(find.byType(AnimatedContainer));
    expect(size.width, greaterThan(100));
    expect(size.width, lessThan(200));

    await tester.pumpAndSettle();
    expect(tester.getSize(find.byType(AnimatedContainer)), const Size(200, 200));
  });
}
