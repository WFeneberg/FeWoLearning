import 'package:flutter/material.dart';
import 'package:golden_toolkit/golden_toolkit.dart';

import 'golden_toolkit_multi_device.dart';

// NOTE: like any golden test, the first run against a real Flutter SDK must
// generate the baseline image (`flutter test --update-goldens`) before this
// can compare against it — that is expected, not a bug in this exercise.
void main() {
  testGoldens('GreetingCard renders across phone and tablet', (tester) async {
    final builder = DeviceBuilder()
      ..overrideDevicesForAllScenarios(
        devices: [Device.phone, Device.tabletLandscape],
      )
      ..addScenario(
        widget: const Material(child: GreetingCard(name: 'Ada')),
        name: 'greeting_card',
      );

    await tester.pumpDeviceBuilder(builder);

    await screenMatchesGolden(tester, 'greeting_card_multi_device');
  });
}
