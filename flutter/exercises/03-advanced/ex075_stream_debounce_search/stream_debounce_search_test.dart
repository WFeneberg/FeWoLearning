import 'dart:async';

import 'package:test/test.dart';

import 'stream_debounce_search.dart';

void main() {
  test('suppresses events superseded within the debounce window', () async {
    final controller = StreamController<String>();
    final debounced =
        debounce(controller.stream, const Duration(milliseconds: 30));

    final results = <String>[];
    final sub = debounced.listen(results.add);

    controller.add('f');
    await Future.delayed(const Duration(milliseconds: 10));
    controller.add('fl');
    await Future.delayed(const Duration(milliseconds: 10));
    controller.add('flu');
    await Future.delayed(const Duration(milliseconds: 60));

    expect(results, ['flu']);

    await sub.cancel();
    await controller.close();
  });

  test('emits each value once the source goes quiet between bursts', () async {
    final controller = StreamController<String>();
    final debounced =
        debounce(controller.stream, const Duration(milliseconds: 30));

    final results = <String>[];
    final sub = debounced.listen(results.add);

    controller.add('a');
    await Future.delayed(const Duration(milliseconds: 60));
    controller.add('b');
    await Future.delayed(const Duration(milliseconds: 60));

    expect(results, ['a', 'b']);

    await sub.cancel();
    await controller.close();
  });
}
