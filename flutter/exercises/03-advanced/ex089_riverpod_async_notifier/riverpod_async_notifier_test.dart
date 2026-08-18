import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:test/test.dart';

import 'riverpod_async_notifier.dart';

void main() {
  test(
      'a failing fetch surfaces as AsyncValue.error, retry resolves to data',
      () async {
    var shouldFail = true;
    final container = ProviderContainer(
      overrides: [
        fetcherProvider.overrideWithValue(() async {
          if (shouldFail) throw Exception('network down');
          return 42;
        }),
      ],
    );
    addTearDown(container.dispose);

    await expectLater(
      container.read(numberProvider.future),
      throwsException,
    );
    expect(container.read(numberProvider).hasError, isTrue);

    shouldFail = false;
    await container.read(numberProvider.notifier).retry();

    expect(container.read(numberProvider).value, 42);
  });
}
