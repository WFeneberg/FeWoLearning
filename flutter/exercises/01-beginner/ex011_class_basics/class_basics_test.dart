import 'package:test/test.dart';

import 'class_basics.dart';

void main() {
  test('Point stores the coordinates passed to its constructor', () {
    final p = Point(3, 4);
    expect(p.x, 3);
    expect(p.y, 4);
  });

  test('distanceTo computes the Euclidean distance between two points', () {
    expect(Point(0, 0).distanceTo(Point(3, 4)), 5);
  });

  test('distanceTo is symmetric', () {
    final a = Point(1, 1);
    final b = Point(4, 5);
    expect(a.distanceTo(b), closeTo(b.distanceTo(a), 0.0001));
  });
}
