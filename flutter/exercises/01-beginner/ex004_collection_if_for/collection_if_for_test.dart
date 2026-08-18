import 'package:test/test.dart';

import 'collection_if_for.dart';

void main() {
  test('evensBelow(10) returns 0,2,4,6,8', () {
    expect(evensBelow(10), [0, 2, 4, 6, 8]);
  });

  test('evensBelow(1) returns just 0', () {
    expect(evensBelow(1), [0]);
  });

  test('evensBelow(0) returns an empty list', () {
    expect(evensBelow(0), <int>[]);
  });
}
