import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'hero_animation_basics.dart';

void main() {
  testWidgets('home screen has a Hero with the shared tag', (tester) async {
    await tester.pumpWidget(buildHeroDemoApp());

    final hero = tester.widget<Hero>(find.byType(Hero));
    expect(hero.tag, heroTag);
  });

  testWidgets('tapping navigates to a detail screen with the same tag', (tester) async {
    await tester.pumpWidget(buildHeroDemoApp());

    await tester.tap(find.byType(Hero));
    await tester.pumpAndSettle();

    expect(find.byType(DetailPage), findsOneWidget);
    final heroes = tester.widgetList<Hero>(find.byType(Hero));
    expect(heroes.every((h) => h.tag == heroTag), isTrue);
  });
}
