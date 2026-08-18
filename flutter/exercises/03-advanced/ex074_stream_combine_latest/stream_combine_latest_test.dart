import 'dart:async';

import 'package:test/test.dart';

import 'stream_combine_latest.dart';

void main() {
  test('emits nothing until both sources have emitted once', () async {
    final a = StreamController<int>();
    final b = StreamController<String>();
    final combined = combineLatest2(a.stream, b.stream, (x, y) => '$x-$y');

    final results = <String>[];
    final sub = combined.listen(results.add);

    a.add(1);
    await Future.delayed(Duration.zero);
    expect(results, isEmpty);

    b.add('a');
    await Future.delayed(Duration.zero);
    expect(results, ['1-a']);

    await sub.cancel();
    await a.close();
    await b.close();
  });

  test('re-emits on every subsequent event from either source', () async {
    final a = StreamController<int>();
    final b = StreamController<String>();
    final combined = combineLatest2(a.stream, b.stream, (x, y) => '$x-$y');

    final results = <String>[];
    final sub = combined.listen(results.add);

    a.add(1);
    b.add('a');
    await Future.delayed(Duration.zero);
    a.add(2);
    await Future.delayed(Duration.zero);
    b.add('b');
    await Future.delayed(Duration.zero);

    expect(results, ['1-a', '2-a', '2-b']);

    await sub.cancel();
    await a.close();
    await b.close();
  });
}
