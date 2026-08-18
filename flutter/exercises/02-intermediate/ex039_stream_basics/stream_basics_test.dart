import 'package:test/test.dart';

import 'stream_basics.dart';

void main() {
  test('countUpTo emits 1..n in order', () async {
    expect(await collect(countUpTo(4)), [1, 2, 3, 4]);
  });

  test('countUpTo(0) emits nothing', () async {
    expect(await collect(countUpTo(0)), <int>[]);
  });

  test('await-for gathers the same values as collect', () async {
    final result = <int>[];
    await for (final value in countUpTo(3)) {
      result.add(value);
    }
    expect(result, [1, 2, 3]);
  });
}
