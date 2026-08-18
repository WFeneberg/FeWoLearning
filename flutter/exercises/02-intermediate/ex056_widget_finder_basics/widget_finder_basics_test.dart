import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'widget_finder_basics.dart';

void main() {
  testWidgets('LikeButton starts unliked', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: LikeButton())));

    expect(find.byIcon(Icons.favorite_border), findsOneWidget);
    expect(find.text('0 likes'), findsOneWidget);
  });

  testWidgets('tapping the icon button likes it', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: LikeButton())));

    await tester.tap(find.byType(IconButton));
    await tester.pump();

    expect(find.byIcon(Icons.favorite), findsOneWidget);
    expect(find.text('1 like'), findsOneWidget);
  });

  testWidgets('tapping twice unlikes it again', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: LikeButton())));

    await tester.tap(find.byType(IconButton));
    await tester.pump();
    await tester.tap(find.byType(IconButton));
    await tester.pump();

    expect(find.byIcon(Icons.favorite_border), findsOneWidget);
    expect(find.text('0 likes'), findsOneWidget);
  });
}
