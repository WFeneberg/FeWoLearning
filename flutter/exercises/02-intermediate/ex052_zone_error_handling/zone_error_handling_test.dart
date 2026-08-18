import 'dart:async';

import 'package:test/test.dart';

import 'zone_error_handling.dart';

void main() {
  test('runGuarded reports an uncaught error thrown inside a microtask',
      () async {
    final errors = <Object>[];
    runGuarded(() {
      scheduleMicrotask(() => throw StateError('boom'));
    }, errors.add);

    await Future<void>.delayed(Duration.zero);
    expect(errors, hasLength(1));
    expect(errors.first, isA<StateError>());
  });

  test('runGuarded does not report anything when nothing throws', () async {
    final errors = <Object>[];
    runGuarded(() {}, errors.add);
    await Future<void>.delayed(Duration.zero);
    expect(errors, isEmpty);
  });
}
