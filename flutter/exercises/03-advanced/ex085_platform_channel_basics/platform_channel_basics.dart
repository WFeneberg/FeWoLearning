// Exercise 085 - platform channel basics (advanced).
//
// Goal:   Fetch the device battery level through a MethodChannel.
// Drills: MethodChannel, invokeMethod, platform interop.
// Passes: when getBatteryLevel() returns whatever the platform side reports
//         via the "battery" channel's "getBatteryLevel" method.

import 'package:flutter/services.dart';

class BatteryService {
  BatteryService([MethodChannel? channel])
      : _channel = channel ?? const MethodChannel('battery');

  final MethodChannel _channel;

  Future<int> getBatteryLevel() {
    throw UnimplementedError('TODO');
  }
}
