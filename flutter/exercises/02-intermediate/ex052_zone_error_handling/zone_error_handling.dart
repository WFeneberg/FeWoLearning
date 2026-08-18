// Exercise 052 - runZonedGuarded (intermediate).
//
// Goal:   Run a body function inside a guarded zone so that errors thrown
//         asynchronously (e.g. inside a microtask) are reported instead of
//         crashing.
// Drills: dart:async, runZonedGuarded, zone error handlers.
// Passes: when an error thrown inside a microtask scheduled by body() is
//         delivered to onError() instead of becoming an uncaught error.

import 'dart:async';

void runGuarded(void Function() body, void Function(Object error) onError) {
  throw UnimplementedError('TODO');
}
