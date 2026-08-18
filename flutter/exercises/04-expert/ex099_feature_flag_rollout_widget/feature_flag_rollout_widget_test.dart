import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';

import 'feature_flag_rollout_widget.dart';

void main() {
  test('isEnabledFor is deterministic for the same user id', () {
    const flag = FeatureFlag('new_ui', 50);
    final first = flag.isEnabledFor('user-42');
    final second = flag.isEnabledFor('user-42');
    expect(first, second);
  });

  test('0% rollout disables the flag for everyone', () {
    const flag = FeatureFlag('new_ui', 0);
    for (var i = 0; i < 50; i++) {
      expect(flag.isEnabledFor('user-$i'), isFalse);
    }
  });

  test('100% rollout enables the flag for everyone', () {
    const flag = FeatureFlag('new_ui', 100);
    for (var i = 0; i < 50; i++) {
      expect(flag.isEnabledFor('user-$i'), isTrue);
    }
  });

  test('a partial rollout enables the flag for only some users', () {
    const flag = FeatureFlag('new_ui', 50);
    final results = [for (var i = 0; i < 200; i++) flag.isEnabledFor('user-$i')];
    final enabledCount = results.where((enabled) => enabled).length;
    expect(enabledCount, greaterThan(50));
    expect(enabledCount, lessThan(150));
  });

  testWidgets('FeatureFlagView renders the on child when enabled',
      (tester) async {
    const flag = FeatureFlag('new_ui', 100);
    await tester.pumpWidget(Directionality(
      textDirection: TextDirection.ltr,
      child: FeatureFlagView(
        flag: flag,
        userId: 'user-1',
        onChild: const Text('on'),
        offChild: const Text('off'),
      ),
    ));

    expect(find.text('on'), findsOneWidget);
    expect(find.text('off'), findsNothing);
  });

  testWidgets('FeatureFlagView renders the off child when disabled',
      (tester) async {
    const flag = FeatureFlag('new_ui', 0);
    await tester.pumpWidget(Directionality(
      textDirection: TextDirection.ltr,
      child: FeatureFlagView(
        flag: flag,
        userId: 'user-1',
        onChild: const Text('on'),
        offChild: const Text('off'),
      ),
    ));

    expect(find.text('off'), findsOneWidget);
    expect(find.text('on'), findsNothing);
  });
}
