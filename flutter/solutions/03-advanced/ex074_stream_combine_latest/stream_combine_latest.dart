// Exercise 074 - combining latest stream values (reference solution).

import 'dart:async';

Stream<R> combineLatest2<A, B, R>(
  Stream<A> a,
  Stream<B> b,
  R Function(A, B) combiner,
) {
  late final StreamController<R> controller;
  A? latestA;
  B? latestB;
  var hasA = false;
  var hasB = false;
  StreamSubscription<A>? subA;
  StreamSubscription<B>? subB;

  void maybeEmit() {
    if (hasA && hasB) {
      controller.add(combiner(latestA as A, latestB as B));
    }
  }

  controller = StreamController<R>(
    onListen: () {
      subA = a.listen(
        (value) {
          latestA = value;
          hasA = true;
          maybeEmit();
        },
        onError: controller.addError,
      );
      subB = b.listen(
        (value) {
          latestB = value;
          hasB = true;
          maybeEmit();
        },
        onError: controller.addError,
      );
    },
    onCancel: () async {
      await subA?.cancel();
      await subB?.cancel();
    },
  );

  return controller.stream;
}
