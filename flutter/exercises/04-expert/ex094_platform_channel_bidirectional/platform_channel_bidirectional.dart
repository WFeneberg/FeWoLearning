// Exercise 094 - bidirectional platform channels (expert).
//
// Goal:   Wrap a MethodChannel call ("getBatteryLevel") and an EventChannel
//         stream ("batteryState") behind a small BatteryService facade.
// Drills: MethodChannel.invokeMethod, EventChannel.receiveBroadcastStream,
//         platform interop error mapping.
// Passes: when getBatteryLevel() returns the platform's reported level (and
//         throws a BatteryException on a PlatformException), and
//         onStateChanged() re-emits every event from the platform's event
//         stream.

import 'package:flutter/services.dart';

class BatteryException implements Exception {
  BatteryException(this.message);
  final String message;

  @override
  String toString() => 'BatteryException: $message';
}

class BatteryService {
  BatteryService({
    MethodChannel methodChannel = const MethodChannel('battery/methods'),
    EventChannel eventChannel = const EventChannel('battery/events'),
  })  : _methodChannel = methodChannel,
        _eventChannel = eventChannel;

  final MethodChannel _methodChannel;
  final EventChannel _eventChannel;

  Future<int> getBatteryLevel() async {
    throw UnimplementedError('TODO');
  }

  Stream<String> onStateChanged() {
    throw UnimplementedError('TODO');
  }
}
