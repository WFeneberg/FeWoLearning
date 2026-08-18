import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

import 'platform_channel_basics.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  const channel = MethodChannel('battery');

  tearDown(() {
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, null);
  });

  test('returns the level reported by the platform side', () async {
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, (call) async {
      if (call.method == 'getBatteryLevel') return 77;
      return null;
    });

    final service = BatteryService(channel);

    expect(await service.getBatteryLevel(), 77);
  });
}
