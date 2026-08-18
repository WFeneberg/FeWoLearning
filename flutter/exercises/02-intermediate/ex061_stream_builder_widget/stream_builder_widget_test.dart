import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'stream_builder_widget.dart';

void main() {
  testWidgets('shows the initial score before any event', (tester) async {
    final controller = StreamController<int>();
    addTearDown(controller.close);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: LiveScoreDisplay(scoreStream: controller.stream))),
    );

    expect(find.text('Score: 0'), findsOneWidget);
  });

  testWidgets('updates when the stream emits a new score', (tester) async {
    final controller = StreamController<int>();
    addTearDown(controller.close);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: LiveScoreDisplay(scoreStream: controller.stream))),
    );

    controller.add(7);
    await tester.pump();

    expect(find.text('Score: 7'), findsOneWidget);
  });

  testWidgets('keeps showing the latest score after multiple events', (tester) async {
    final controller = StreamController<int>();
    addTearDown(controller.close);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: LiveScoreDisplay(scoreStream: controller.stream))),
    );

    controller.add(1);
    await tester.pump();
    controller.add(2);
    await tester.pump();

    expect(find.text('Score: 2'), findsOneWidget);
  });
}
