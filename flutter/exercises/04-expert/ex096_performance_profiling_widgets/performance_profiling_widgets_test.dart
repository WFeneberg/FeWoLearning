import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'performance_profiling_widgets.dart';

void main() {
  testWidgets('tapping increment repaints the counter text', (tester) async {
    var paintCount = 0;
    await tester.pumpWidget(MaterialApp(
      home: ProfiledCounterView(onPaint: () => paintCount++),
    ));

    expect(find.text('0'), findsOneWidget);

    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('1'), findsOneWidget);
  });

  testWidgets('tapping increment does not repaint the expensive painter',
      (tester) async {
    var paintCount = 0;
    await tester.pumpWidget(MaterialApp(
      home: ProfiledCounterView(onPaint: () => paintCount++),
    ));

    final paintsAfterInitialFrame = paintCount;
    expect(paintsAfterInitialFrame, greaterThan(0));

    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(paintCount, paintsAfterInitialFrame);
  });

  testWidgets('the painter sits inside a RepaintBoundary', (tester) async {
    await tester.pumpWidget(MaterialApp(
      home: ProfiledCounterView(onPaint: () {}),
    ));

    expect(
      find.ancestor(
        of: find.byType(CustomPaint),
        matching: find.byType(RepaintBoundary),
      ),
      findsWidgets,
    );
  });
}
