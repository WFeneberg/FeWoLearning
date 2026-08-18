// Exercise 051 - Timer & Timer.periodic (reference solution).

import 'dart:async';

class Ticker {
  Timer? _timer;
  int ticks = 0;

  void start(Duration interval) {
    _timer = Timer.periodic(interval, (_) => ticks++);
  }

  void stop() {
    _timer?.cancel();
    _timer = null;
  }
}
