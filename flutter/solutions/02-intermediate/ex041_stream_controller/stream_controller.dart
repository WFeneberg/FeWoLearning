// Exercise 041 - StreamController & broadcast streams (reference solution).

import 'dart:async';

class EventBus {
  final StreamController<String> _controller =
      StreamController<String>.broadcast();

  Stream<String> get events => _controller.stream;

  void publish(String event) => _controller.add(event);

  Future<void> close() => _controller.close();
}
