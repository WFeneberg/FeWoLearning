// Exercise 052 - runZonedGuarded (reference solution).

import 'dart:async';

void runGuarded(void Function() body, void Function(Object error) onError) {
  runZonedGuarded(body, (error, stackTrace) => onError(error));
}
