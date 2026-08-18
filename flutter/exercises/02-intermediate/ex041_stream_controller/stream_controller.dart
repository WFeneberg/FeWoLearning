// Exercise 041 - StreamController & broadcast streams (intermediate).
//
// Goal:   Build a tiny event bus where every current listener receives every
//         published event.
// Drills: StreamController.broadcast, Stream getter, closing a controller.
// Passes: when two independent listeners on `events` each receive every
//         event passed to publish(), in order.

import 'dart:async';

class EventBus {
  final StreamController<String> _controller =
      StreamController<String>.broadcast();

  Stream<String> get events => throw UnimplementedError('TODO');

  void publish(String event) {
    throw UnimplementedError('TODO');
  }

  Future<void> close() {
    throw UnimplementedError('TODO');
  }
}
