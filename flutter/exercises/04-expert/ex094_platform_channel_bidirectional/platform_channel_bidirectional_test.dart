import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

import 'platform_channel_bidirectional.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  const methodChannel = MethodChannel('battery/methods');
  const eventChannel = EventChannel('battery/events');
  final eventHandshakeChannel = MethodChannel(eventChannel.name);
  final messenger =
      TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger;

  setUp(() {
    // EventChannel.receiveBroadcastStream() internally sends a "listen"
    // (and later "cancel") MethodCall over the same channel name to set up
    // the platform-side stream; without a handler these would throw.
    messenger.setMockMethodCallHandler(
      eventHandshakeChannel,
      (call) async => null,
    );
  });

  tearDown(() {
    messenger.setMockMethodCallHandler(methodChannel, null);
    messenger.setMockMethodCallHandler(eventHandshakeChannel, null);
  });

  test('getBatteryLevel returns the platform-reported level', () async {
    messenger.setMockMethodCallHandler(methodChannel, (call) async {
      expect(call.method, 'getBatteryLevel');
      return 76;
    });

    final service =
        BatteryService(methodChannel: methodChannel, eventChannel: eventChannel);

    expect(await service.getBatteryLevel(), 76);
  });

  test('getBatteryLevel maps a PlatformException to a BatteryException',
      () async {
    messenger.setMockMethodCallHandler(methodChannel, (call) async {
      throw PlatformException(code: 'UNAVAILABLE', message: 'no battery');
    });

    final service =
        BatteryService(methodChannel: methodChannel, eventChannel: eventChannel);

    expect(service.getBatteryLevel(), throwsA(isA<BatteryException>()));
  });

  test('onStateChanged re-emits every platform event', () async {
    final service =
        BatteryService(methodChannel: methodChannel, eventChannel: eventChannel);

    final received = <String>[];
    final subscription = service.onStateChanged().listen(received.add);
    await Future<void>.delayed(Duration.zero);

    final codec = eventChannel.codec;
    for (final value in ['charging', 'discharging']) {
      await messenger.handlePlatformMessage(
        eventChannel.name,
        codec.encodeSuccessEnvelope(value),
        (_) {},
      );
    }
    await Future<void>.delayed(Duration.zero);

    expect(received, ['charging', 'discharging']);
    await subscription.cancel();
  });
}
