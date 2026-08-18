// Exercise 033 - == and hashCode contract (beginner).
//
// Goal:   Make Point value-equal by coordinates instead of by identity, and
//         keep its hashCode consistent with that equality.
// Drills: operator==, hashCode, the equal-objects-have-equal-hashCodes rule.
// Passes: when two Points with the same coordinates are == and share a
//         hashCode, different coordinates are not ==, and a Set<Point>
//         correctly de-duplicates equal points.

class Point {
  final int x;
  final int y;
  const Point(this.x, this.y);

  @override
  bool operator ==(Object other) {
    throw UnimplementedError('TODO');
  }

  @override
  int get hashCode {
    throw UnimplementedError('TODO');
  }
}
