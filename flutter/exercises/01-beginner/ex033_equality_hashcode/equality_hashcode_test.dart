import 'package:test/test.dart';

import 'equality_hashcode.dart';

void main() {
  test('points with the same coordinates are equal', () {
    expect(const Point(1, 2), const Point(1, 2));
  });

  test('points with different coordinates are not equal', () {
    expect(const Point(1, 2) == const Point(2, 1), isFalse);
  });

  test('equal points share a hashCode', () {
    expect(const Point(3, 4).hashCode, const Point(3, 4).hashCode);
  });

  test('a Set de-duplicates equal points', () {
    final points = {const Point(1, 1), const Point(1, 1), const Point(2, 2)};
    expect(points.length, 2);
  });
}
