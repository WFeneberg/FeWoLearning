import 'package:test/test.dart';

import 'future_basics.dart';

void main() {
  test('doubleAsync doubles a non-negative value', () async {
    expect(await doubleAsync(5), 10);
  });

  test('doubleAsync fails for negative values', () {
    expect(doubleAsync(-1), throwsArgumentError);
  });

  test('doubleWithFallback doubles on success', () async {
    expect(await doubleWithFallback(Future.value(3), -1), 6);
  });

  test('doubleWithFallback returns the fallback on failure', () async {
    expect(await doubleWithFallback(Future.error('boom'), -1), -1);
  });
}
