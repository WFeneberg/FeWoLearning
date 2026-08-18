// Exercise 085 - platform channel basics (reference solution).

import 'package:flutter/services.dart';

class BatteryService {
  BatteryService([MethodChannel? channel])
      : _channel = channel ?? const MethodChannel('battery');

  final MethodChannel _channel;

  Future<int> getBatteryLevel() async {
    final level = await _channel.invokeMethod<int>('getBatteryLevel');
    return level ?? -1;
  }
}
