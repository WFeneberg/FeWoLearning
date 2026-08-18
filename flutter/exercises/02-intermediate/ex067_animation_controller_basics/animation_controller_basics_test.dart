import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'animation_controller_basics.dart';

void main() {
  testWidgets('FadeInBox starts fully transparent', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: FadeInBox())));

    final transition = tester.widget<FadeTransition>(find.byType(FadeTransition));
    expect(transition.opacity.value, 0.0);
  });

  testWidgets('FadeInBox is fully opaque once the animation settles', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: FadeInBox())));

    await tester.pumpAndSettle();

    final transition = tester.widget<FadeTransition>(find.byType(FadeTransition));
    expect(transition.opacity.value, 1.0);
  });

  testWidgets('FadeInBox is partially visible mid-animation', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: FadeInBox())));

    await tester.pump(const Duration(milliseconds: 150));

    final transition = tester.widget<FadeTransition>(find.byType(FadeTransition));
    expect(transition.opacity.value, greaterThan(0.0));
    expect(transition.opacity.value, lessThan(1.0));
  });
}
