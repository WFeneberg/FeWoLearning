// Exercise 050 - Completer basics (intermediate).
//
// Goal:   Bridge a callback-style API into a Future using a Completer.
// Drills: dart:async, Completer, callback-to-Future adaptation.
// Passes: when fetchAsFuture() resolves with whatever value the legacy
//         callback API produces.

import 'dart:async';

typedef Callback = void Function(int result);

void legacyFetch(int input, Callback onDone) {
  onDone(input * 2);
}

Future<int> fetchAsFuture(int input) {
  throw UnimplementedError('TODO');
}
