// Exercise 089 - AsyncNotifier with retry (advanced).
//
// Goal:   Fetch a number through an AsyncNotifier and expose a retry() that
//         re-runs the fetch after a failure.
// Drills: AsyncNotifier, AsyncValue, AsyncValue.guard, provider overrides.
// Passes: when a failing fetch surfaces as AsyncValue.error and calling
//         retry() re-fetches, resolving to AsyncValue.data on success.

import 'package:flutter_riverpod/flutter_riverpod.dart';

final fetcherProvider = Provider<Future<int> Function()>((ref) {
  throw UnimplementedError('override this provider in tests/app setup');
});

class NumberNotifier extends AsyncNotifier<int> {
  @override
  Future<int> build() {
    throw UnimplementedError('TODO');
  }

  Future<void> retry() {
    throw UnimplementedError('TODO');
  }
}

final numberProvider =
    AsyncNotifierProvider<NumberNotifier, int>(NumberNotifier.new);
