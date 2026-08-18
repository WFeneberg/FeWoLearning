// Exercise 094 - bidirectional platform channels (reference solution).

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
    try {
      final level = await _methodChannel.invokeMethod<int>('getBatteryLevel');
      return level!;
    } on PlatformException catch (e) {
      throw BatteryException(e.message ?? 'unknown platform error');
    }
  }

  Stream<String> onStateChanged() {
    return _eventChannel
        .receiveBroadcastStream()
        .map((event) => event as String);
  }
}
