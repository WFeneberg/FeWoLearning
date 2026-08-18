// Exercise 050 - Completer basics (reference solution).

import 'dart:async';

typedef Callback = void Function(int result);

void legacyFetch(int input, Callback onDone) {
  onDone(input * 2);
}

Future<int> fetchAsFuture(int input) {
  final completer = Completer<int>();
  legacyFetch(input, (result) {
    completer.complete(result);
  });
  return completer.future;
}
