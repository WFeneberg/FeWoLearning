import 'package:flutter_test/flutter_test.dart';

import 'bloc_basics.dart';

void main() {
  testWidgets('tapping increment dispatches an event and rebuilds', (tester) async {
    await tester.pumpWidget(buildCounterApp());

    expect(find.text('Count: 0'), findsOneWidget);

    await tester.tap(find.text('Increment'));
    await tester.pump();

    expect(find.text('Count: 1'), findsOneWidget);
  });

  testWidgets('multiple taps accumulate', (tester) async {
    await tester.pumpWidget(buildCounterApp());

    await tester.tap(find.text('Increment'));
    await tester.pump();
    await tester.tap(find.text('Increment'));
    await tester.pump();

    expect(find.text('Count: 2'), findsOneWidget);
  });
}
