import 'package:flutter_test/flutter_test.dart';

import 'provider_basics.dart';

void main() {
  testWidgets('tapping increment updates the displayed count', (tester) async {
    await tester.pumpWidget(buildCounterApp());

    expect(find.text('Count: 0'), findsOneWidget);

    await tester.tap(find.text('Increment'));
    await tester.pump();

    expect(find.text('Count: 1'), findsOneWidget);
    expect(find.text('Count: 0'), findsNothing);
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
