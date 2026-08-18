import 'package:test/test.dart';

import 'async_await_basics.dart';

void main() {
  test('fetchDouble doubles after an async delay', () async {
    expect(await fetchDouble(3), 6);
  });

  test('fetchSquareOfDouble awaits fetchDouble before squaring', () async {
    expect(await fetchSquareOfDouble(3), 36);
  });

  test('fetchSquareOfDouble handles zero', () async {
    expect(await fetchSquareOfDouble(0), 0);
  });
}
