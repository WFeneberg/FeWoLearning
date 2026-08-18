// Exercise 051 - Timer & Timer.periodic (intermediate).
//
// Goal:   Build a Ticker that increments a counter on every tick of a
//         periodic timer, until stopped.
// Drills: dart:async, Timer.periodic, Timer.cancel.
// Passes: when start() increments ticks on each interval, and stop() cancels
//         the timer so ticks no longer changes afterward.

import 'dart:async';

class Ticker {
  Timer? _timer;
  int ticks = 0;

  void start(Duration interval) {
    throw UnimplementedError('TODO');
  }

  void stop() {
    throw UnimplementedError('TODO');
  }
}
