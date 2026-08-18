// Exercise 089 - AsyncNotifier with retry (reference solution).

import 'package:flutter_riverpod/flutter_riverpod.dart';

final fetcherProvider = Provider<Future<int> Function()>((ref) {
  throw UnimplementedError('override this provider in tests/app setup');
});

class NumberNotifier extends AsyncNotifier<int> {
  @override
  Future<int> build() {
    final fetch = ref.watch(fetcherProvider);
    return fetch();
  }

  Future<void> retry() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() {
      final fetch = ref.read(fetcherProvider);
      return fetch();
    });
  }
}

final numberProvider =
    AsyncNotifierProvider<NumberNotifier, int>(NumberNotifier.new);
