// Exercise 075 - debounced search stream (reference solution).

import 'dart:async';

Stream<T> debounce<T>(Stream<T> source, Duration duration) {
  late final StreamController<T> controller;
  Timer? timer;
  StreamSubscription<T>? subscription;

  controller = StreamController<T>(
    onListen: () {
      subscription = source.listen(
        (value) {
          timer?.cancel();
          timer = Timer(duration, () => controller.add(value));
        },
        onError: controller.addError,
        onDone: () {
          timer?.cancel();
          controller.close();
        },
      );
    },
    onCancel: () async {
      timer?.cancel();
      await subscription?.cancel();
    },
  );

  return controller.stream;
}
