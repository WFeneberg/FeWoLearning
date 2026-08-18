import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'gesture_detector_basics.dart';

void main() {
  testWidgets('DragCounter starts at zero', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: DragCounter())));

    expect(find.text('0'), findsOneWidget);
  });

  testWidgets('dragging horizontally accumulates the distance', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: DragCounter())));

    await tester.drag(find.byType(GestureDetector), const Offset(50, 0));
    await tester.pump();

    expect(find.text('50'), findsOneWidget);
  });

  testWidgets('tapping resets the accumulated distance', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: DragCounter())));

    await tester.drag(find.byType(GestureDetector), const Offset(50, 0));
    await tester.pump();
    await tester.tap(find.byType(GestureDetector));
    await tester.pump();

    expect(find.text('0'), findsOneWidget);
  });
}
